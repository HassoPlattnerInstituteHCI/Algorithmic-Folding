import Polygon from "./Polygon";
import * as THREE from 'three';
import Joint from "./Joint";
import GeometryUtil from "../Util/GeometryUtil";

// two plates can have any number of joints between them
export default class Plate {
  private readonly id: string;
  private readonly points: THREE.Vector3[];
  private readonly normal: THREE.Vector3;
  private readonly polygon: Polygon;
  private joints: Map<Plate, Joint[]>;

  constructor(id: string, points: THREE.Vector3[], polygon: Polygon, joints: Joint[]) {
    this.id = id;
    this.points = points;
    this.polygon = polygon;
    this.joints = new Map();
    this.setJoints(joints);
    this.normal = this.computeNormal();
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

  public getNormal(): THREE.Vector3 {
    return this.normal;
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

    return side2.clone().cross(side1).normalize();
  }
}