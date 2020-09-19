import Plate from "../../Model/Plate";
import * as THREE from 'three';
import GeometryUtil from "../../Util/GeometryUtil";

export default class PlateInfo {
  public plate: Plate;
  public facing = [false, false, false]; // for x, y, z
  public ratio: number; // convex / concave connection ratio

  public outside: boolean;
  public inside: boolean;

  public partOfCycle: boolean;
  public cGrouped: boolean;
  public partOfCShape: number;
  public partOfHole: number;

  public allConnections: Array<[Plate, "convex" | "concave"]> = [];
  public xCons: Array<[Plate, "convex" | "concave"]> = [];
  public yCons: Array<[Plate, "convex" | "concave"]> = [];
  public zCons: Array<[Plate, "convex" | "concave"]> = [];

  private readonly xEn: boolean;
  private readonly yEn: boolean;
  private readonly zEn: boolean;

  private readonly otherDim: number; // how many non-facing dirs are enabled

  constructor(plate: Plate) {
    this.plate = plate;

    // compute facing:
    const plateNormal: THREE.Vector3 = plate.getNormal();

    this.facing[0] = Math.abs(plateNormal.x) > 0.5;
    this.facing[1] = Math.abs(plateNormal.y) > 0.5;
    this.facing[2] = Math.abs(plateNormal.z) > 0.5;

    // build connections
    this.allConnections = plate
      .getJoints()
      .map(joint => {
        const otherPlate: Plate = joint.getOtherPlate(plate);
        const type = GeometryUtil.isJointConvex(joint) ? "convex" : "concave";
        return [otherPlate, type];
      });

    // remove duplicates
    this.removeDuplicatePlates();

    // categorize connections and set convex-concave ratio
    let convex = 0;
    let concave = 0;

    this.allConnections.forEach(con => {
      const otherPlate = con[0];
      const normal = otherPlate.getNormal();

      if (Math.abs(normal.x) > 0.5) this.xCons.push(con);
      if (Math.abs(normal.y) > 0.5) this.yCons.push(con);
      if (Math.abs(normal.z) > 0.5) this.zCons.push(con);

      if (con[1] === "convex") convex++;
      else concave++;
    });
    this.ratio = convex / (convex + concave);

    // compute if the plate is enabled for forbidden directions
    this.xEn = this.shouldBeEnabled(this.xCons);
    this.yEn = this.shouldBeEnabled(this.yCons);
    this.zEn = this.shouldBeEnabled(this.zCons);

    // compute the enabled, non-facing dims
    this.otherDim = 0;
    if (this.xEn && !this.facing[0]) this.otherDim++;
    if (this.yEn && !this.facing[1]) this.otherDim++;
    if (this.zEn && !this.facing[2]) this.otherDim++;
  }

  // returns if a plate can be used with forbidden direction dir
  public isEnabled(dir: "x" | "y" | "z"): boolean {
    switch (dir) {
      case "x":
        return this.xEn;
      case "y":
        return this.yEn;
      case "z":
        return this.zEn;
      default:
        return false;
    }
  }

  // returns if a plate is facing
  public isFacing(dir: "x" | "y" | "z"): boolean {
    switch (dir) {
      case "x":
        return this.facing[0];
      case "y":
        return this.facing[1];
      case "z":
        return this.facing[2];
      default:
        return false;
    }
  }

  // trigger resorting of allConnections and the directional connections
  public sortConnections(info: Map<Plate, PlateInfo>): void {
    const comp = this._getComparator(info);

    this.allConnections.sort(comp);
    this.xCons.sort(comp);
    this.yCons.sort(comp);
    this.zCons.sort(comp);
  }

  // counts the convex connections in the list, returns true if >= 2
  private shouldBeEnabled(cons: Array<[Plate, "convex" | "concave"]>): boolean {

    let convCounter = 0;
    for (let k = 0; convCounter < 2 && k < cons.length; k++) {
      convCounter += cons[k][1] === "convex" ? 1 : 0;
    }

    return convCounter >= 2;
  }

  // rules:
  // Enabled in none, one or both of the other dimensions?
  // At least 1 connection in all directions?
  // Convex-Concave ratio, DESC

  // Count of connections, ASC (holes significantly contribute to connection count)
  private _getComparator(info: Map<Plate, PlateInfo>) {
    return (con1: [Plate, "convex" | "concave"], con2: [Plate, "convex" | "concave"]): number => {
      const info1: PlateInfo = info.get(con1[0]);
      const info2: PlateInfo = info.get(con2[0]);

      // step 1
      if (info1.otherDim > info2.otherDim) return -1;
      if (info1.otherDim < info2.otherDim) return 1;

      // step 2
      let con1Sides = 0;
      con1Sides += info1.xCons.length > 0 ? 1 : 0;
      con1Sides += info1.yCons.length > 0 ? 1 : 0;
      con1Sides += info1.zCons.length > 0 ? 1 : 0;

      let con2Sides = 0;
      con2Sides += info2.xCons.length > 0 ? 1 : 0;
      con2Sides += info2.yCons.length > 0 ? 1 : 0;
      con2Sides += info2.zCons.length > 0 ? 1 : 0;

      if (con1Sides > con2Sides) return -1;
      if (con1Sides < con2Sides) return 1;

      // step 3
      if (info1.ratio > info2.ratio) return -1;
      if (info1.ratio < info2.ratio) return 1;

      // step 4;
      const totalSides1 = info1.xCons.length + info1.yCons.length + info1.zCons.length;
      const totalSides2 = info2.xCons.length + info2.yCons.length + info2.zCons.length;

      if (totalSides1 < totalSides2) return -1;
      if (totalSides1 > totalSides2) return 1;

      // equal
      return 0;
    };
  }

  private removeDuplicatePlates(): void {
    // sort the connections, so duplicate connections would be immediately adjacent in the array
    this.allConnections.sort((a, b) => (a[0].getId() > b[0].getId() ? 1 : a[0].getId() < b[0].getId() ? -1 : 0));

    let j = 0;
    while (j + 1 < this.allConnections.length) {
      if (this.allConnections[j][0] === this.allConnections[j + 1][0]) {
        // remove the duplicate plate
        this.allConnections.splice(j + 1, 1);
      } else j++;
    }
  }
}