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
  private readonly globalToLocalMatrix: THREE.Matrix4;
  private joints: Map<Plate, Joint[]>;

  constructor(id: string, points: THREE.Vector3[], polygon: Polygon, joints: Joint[], globalToLocalMatrix: THREE.Matrix4) {
    this.id = id;
    this.points = points;
    this.polygon = polygon;
    this.joints = new Map();
    this.setJoints(joints);
    this.normal = this.computeNormal();
    this.globalToLocalMatrix = globalToLocalMatrix;
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

  public getGlobalToLocalMatrix(): THREE.Matrix4 {
    return this.globalToLocalMatrix;
  }

  // maps a point on the 3d coordinates of the plate to its 2d counterpart
  public map3dTo2d(point: THREE.Vector3): THREE.Vector2 {

    const p = point.clone().applyMatrix4(this.globalToLocalMatrix);
    return new THREE.Vector2(p.x, p.y);
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