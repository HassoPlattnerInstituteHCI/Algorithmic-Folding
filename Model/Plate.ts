import * as THREE from 'three';
import Polygon from "./Polygon";
import Joint from "./Joint";

// two plates can have any number of joints between them
export default class Plate {
  private id: string;
  private points: THREE.Vector3[];
  private normal: THREE.Vector3;
  private polygon: Polygon;
  private joints: Map<Plate, Joint>;

  constructor(id: string, points: THREE.Vector3[], polygon: Polygon, joints: Joint[]) {
    this.id = id;
    this.points = points;
    this.polygon = polygon;
    this.joints = new Map();
    this.setJoints(joints);

    const side1 = this.points[2].clone().sub(this.points[1]);
    const side2 = this.points[1].clone().sub(this.points[0]);
    this.normal = side1.cross(side2).normalize();
  }

  public setJoints(joints: Joint[]): void {
    this.joints = new Map();
    for (const j of joints) {
      const other = j.getOtherPlate(this);
      this.joints.set(other, j);
    }
  }

  public getJoints(): Joint[] {
    return Array.from(this.joints.values());
  }

  public getJoint(other: Plate): Joint {
    return this.joints.get(other);
  }

  public get2DShape(): Polygon {
    return this.polygon;
  }

  public getNormal(): THREE.Vector3 {
    return this.normal;
  }
}