import * as THREE from 'three';
import Polygon from "./Polygon";
import Joint from "./Joint";

// two plates can have any number of joints between them
export default class Plate {
  private id: string;
  private points: THREE.Vector3[];
  private normal: THREE.Vector3;
  private polygon: Polygon;
  private joints: Map<Plate, Joint[]>;

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

      if (!(this.joints.has(other))) this.joints.set(other, []);
      this.joints.get(other).push(j);
    }
  }

  public getJoints(): Joint[] {
    // flatten the joint map
    const joints: Joint[] = [];
    for (const jointArr of this.joints.values()){
      joints.push(...jointArr);
    }
    return joints;
  }

  public getJoint(other: Plate): Joint {
    // just return the first joint
    return (!(this.joints.has(other)))? undefined : this.joints.get(other)[0];
  }

  public get2DShape(): Polygon {
    return this.polygon;
  }

  public getNormal(): THREE.Vector3 {
    return this.normal;
  }
}