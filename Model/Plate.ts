import Polygon from "./Polygon";
import * as THREE from 'three';
import * as mathjs from "mathjs";
import Joint from "./Joint";
import GeometryUtil from "../Util/GeometryUtil";

// two plates can have any number of joints between them
export default class Plate {
  private readonly id: string;
  private readonly points: THREE.Vector3[];
  private readonly center: THREE.Vector3;
  private readonly normal: THREE.Vector3;
  private readonly polygon: Polygon;
  private joints: Map<Plate, Joint[]>;
  private conversion: [THREE.Vector3[], THREE.Vector2[]];

  constructor(id: string, points: THREE.Vector3[], polygon: Polygon, joints: Joint[], conversion: [THREE.Vector3[], THREE.Vector2[]]) {
    this.id = id;
    this.points = points;
    this.polygon = polygon;
    this.joints = new Map();
    this.setJoints(joints);
    this.normal = this.computeNormal();
    this.conversion = conversion;
    this.center = this.points.reduce((prev, curr) => prev.add(curr), new THREE.Vector3()).divideScalar(this.points.length);
  }

  public setJoints(joints: Joint[]): void {
    this.joints = new Map();
    for (const j of joints) {
      const other = j.getOtherPlate(this);

      if (!(this.joints.has(other))) this.joints.set(other, []);
      this.joints.get(other).push(j);
    }
  }

  public getJoints(): Joint[] {
    // flatten the joint map
    const joints: Joint[] = [];
    for (const jointArr of this.joints.values()) {
      joints.push(...jointArr);
    }
    return joints;
  }

  public getJoint(other: Plate): Joint {
    // just return the first joint
    return (!(this.joints.has(other))) ? undefined : this.joints.get(other)[0];
  }

  public getId(): string {
    return this.id;
  }

  public get2DShape(): Polygon {
    return this.polygon;
  }

  public getPoints(): THREE.Vector3[] {
    return this.points;
  }

  public getNormal(): THREE.Vector3 {
    return this.normal;
  }

  // returns the 3d center of the plate points, which is not necessarily on the plate
  public getCenter(): THREE.Vector3 {
    return this.center;
  }

  public getConversion(): [THREE.Vector3[], THREE.Vector2[]] {
    return this.conversion;
  }

  // maps a point on the 3d coordinates of the plate to its 2d counterpart
  public map3dTo2d(point: THREE.Vector3): THREE.Vector2 {

    const p = point.clone().sub(this.conversion[0][0]);
    const v1 = this.conversion[0][1];
    const v2 = this.conversion[0][2];

    const m = [[v1.x, v2.x, 0], [v1.y, v2.y, 0], [v1.z, v2.z, 0]];
    const n = [p.x, p.y, p.z];
    // ToDo: this appears to not work with slanted edges...
    const solution = mathjs.lusolve(m, n);

    const newPoint = this.conversion[1][0].clone();
    newPoint.add(this.conversion[1][1].clone().multiplyScalar(solution[0][0]));
    newPoint.add(this.conversion[1][2].clone().multiplyScalar(solution[1][0]));

    return newPoint;
  }

  // tries to find the outward-pointing normal, based on the order of the points (CW / CCW)
  private computeNormal(): THREE.Vector3 {
    // Finds an outer edge by choosing any random vector on the plate-plane and then choosing the outermost corner point
    // in that direction. The two adjacent joints of that point are then used to create the plate normals.
    const base = this.points[1].clone().sub(this.points[0]).normalize();

    let maxScalar: number;
    let maxI: number;

    const len = this.points.length;
    for (let i = 0; i < len; i++) {
      const projection = this.points[i].clone().projectOnVector(base);
      const scalar = GeometryUtil.getNeededScalar(projection, base);

      if (maxScalar === undefined || scalar > maxScalar) {
        maxScalar = scalar;
        maxI = i;
      }
    }
    if (maxI === undefined) throw new Error("Failed to create plate normal");

    // generate normal
    const side1 = this.points[(maxI + 1) % len].clone().sub(this.points[maxI]);
    const side2 = this.points[maxI].clone().sub(this.points[(maxI + len - 1) % len]);

    return side1.clone().cross(side2).normalize();
  }
}