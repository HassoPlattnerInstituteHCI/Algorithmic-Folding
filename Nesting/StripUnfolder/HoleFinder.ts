import Plate from "../../Model/Plate";
import PlateInfo from "./PlateInfo";

export default class HoleFinder {

  private readonly infoMap: Map<Plate, PlateInfo>;
  private readonly plates: Plate[];

  constructor(plates: Plate[]) {
    this.plates = plates;

    // prepare the data by filling info and sorting plates and connections for optimal results
    this.infoMap = this.generatePlateInfoMap();
  }

  public getInfoMap(): Map<Plate, PlateInfo> {
    return this.infoMap;
  }

  // returns holes and cShapes
  public getGroups(): [Plate[][], Plate[][]] {

    // tag all plates on the outside
    for (const plate of this.plates) {
      if (!this.infoMap.get(plate).outside) this.tagIfOutside(plate);
    }

    // group all remaining plates as holes
    const groups: Plate[][] = [];

    for (let i = 0; i < this.plates.length; i++) {
      if (this.infoMap.get(this.plates[i]).outside) continue; // not a hole
      if (this.infoMap.get(this.plates[i]).inside) continue; // already processed

      const group: Plate[] = [];

      this.recHoleGroup(this.plates[i], i + 5, group);
      groups.push(group);
    }

    // find c-shapes and edges:
    const cGroups: Plate[][] = [];

    for (let i = 0; i < this.plates.length; i++) {
      const cGroup: Plate[] = [];

      this.recCGroup(this.plates[i], i + 5, cGroup);
      if (cGroup.length === 0) continue;

      cGroups.push(cGroup);
    }

    return [groups, cGroups];
  }

  private generatePlateInfoMap(): Map<Plate, PlateInfo> {
    const infoMap = new Map<Plate, PlateInfo>();

    // fill the info map for each plate
    for (const plate of this.plates) {
      infoMap.set(plate, new PlateInfo(plate));
    }

    // sort the connections of each info
    for (const plate of this.plates) {
      infoMap.get(plate).sortConnections(infoMap);
    }

    return infoMap;
  }

  // tries all 3 dimensions as forbidden to find a cycle
  private tagIfOutside(plate: Plate) {
    return (
      this.recTag(plate, plate, null, 0, "x") ||
      this.recTag(plate, plate, null, 0, "y") ||
      this.recTag(plate, plate, null, 0, "z")
    );
  }

  // tries to find a convex-ish cycle by recursively setting the next plate,
  // no plate can be used twice and the current and next plate have to share
  // two convex side-plates, while the next plate cannot touch the last plate
  private recTag(
    origin: Plate,
    plate: Plate,
    lastPlate: Plate,
    convexProportion: number,
    forbiddenDirection: "x" | "y" | "z",
  ): boolean {
    const plateInfo: PlateInfo = this.infoMap.get(plate);

    // base case: cycle closed - only cycles with convexEdges - concaveEdges = 4 are valid
    // (exactly 4 direction changes that were not reversed)
    if (lastPlate && plate.getId() === origin.getId()) {
      return convexProportion === 4;
    }

    // return if the plate is not enabled in that direction
    switch (forbiddenDirection) {
      case "x":
        if (!plateInfo.isEnabled("x")) return false;
        break;
      case "y":
        if (!plateInfo.isEnabled("y")) return false;
        break;
      case "z":
        if (!plateInfo.isEnabled("z")) return false;
        break;
      default:
        break;
    }

    // find the next plate
    let list1: Array<[Plate, "concave" | "convex"]>;
    let list2: Array<[Plate, "concave" | "convex"]>;

    // set list1 and list2 according to the forbidden direction
    switch (forbiddenDirection) {
      case "x":
        list1 = plateInfo.yCons;
        list2 = plateInfo.zCons;
        break;
      case "y":
        list1 = plateInfo.xCons;
        list2 = plateInfo.zCons;
        break;
      case "z":
        list1 = plateInfo.xCons;
        list2 = plateInfo.yCons;
        break;
      default:
        break;
    }

    let i1 = 0;
    let i2 = 0;
    for (let i = 0; i < list1.length + list2.length; i++) {
      // try to balance the 2 lists from which new candidates are picked
      let candidate: Plate;
      let candidateInfo: PlateInfo;
      let convex: boolean;

      if (i2 >= list2.length || (i1 === i2 && i1 < list1.length)) {
        candidate = list1[i1][0];
        candidateInfo = this.infoMap.get(candidate);

        convex = list1[i1++][1] === "convex";
      } else {
        candidate = list2[i2][0];
        candidateInfo = this.infoMap.get(candidate);

        convex = list2[i2++][1] === "convex";
      }

      if (candidateInfo.partOfCycle) continue; // sides can't be in the cycle twice
      if (candidateInfo.inside) continue; // if a plate was declared as "inside" for some reason, don't try to use it for the outside

      // else found potential next plate
      candidateInfo.partOfCycle = true;
      const result = this.recTag(
        origin,
        candidate,
        plate,
        convexProportion + (convex ? 1 : -1),
        forbiddenDirection,
      );
      candidateInfo.partOfCycle = false;

      if (candidateInfo.outside === false || !result) continue;

      // else found "solution" - this plate is part of the outside
      candidateInfo.outside = true;
      return true;
    }

    return false;
  }


  // recursively groups plates in a hole that are adjacent
  private recHoleGroup(plate: Plate, id: number, group: Plate[]) {
    const plateInfo: PlateInfo = this.infoMap.get(plate);

    // skip all categorized plates (outside / inside) and all plates that are already part of this hole
    if (plateInfo.outside || plateInfo.inside || plateInfo.partOfHole) return;

    // add plate to hole
    group.push(plate);
    plateInfo.inside = true;
    plateInfo.partOfHole = id;

    for (const entry of plateInfo.allConnections) {
      const candidate = entry[0];
      this.recHoleGroup(candidate, id, group);
    }
  }

  // recursively groups plates on the surface that are adjacent and have a concave edge
  private recCGroup(plate: Plate, id: number, group: Plate[]) {
    const plateInfo: PlateInfo = this.infoMap.get(plate);

    // skip all processed plates and all plates on the inside
    if (plateInfo.inside || plateInfo.cGrouped || plateInfo.partOfCShape === id) return;
    plateInfo.partOfCShape = id;

    for (const entry of plateInfo.allConnections) {
      if (entry[1] === "concave") {
        const candidate: Plate = entry[0];
        if (this.infoMap.get(candidate).inside) continue;

        plateInfo.cGrouped = true;
        this.recCGroup(candidate, id, group);
      }
    }

    if (plateInfo.cGrouped) group.push(plate);
  }
}