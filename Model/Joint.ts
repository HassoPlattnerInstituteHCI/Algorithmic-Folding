import Util from "../Util/Util";
import * as THREE from 'three';
import Plate from "./Plate";

// the THREE.Vector2-objects supplied in the constructor should be reference variables to the actual Vector2-Points of the polygons used by plates to represent their outlines in 2D space
export default class Joint {
  private plate1: Plate;
  private plate2: Plate;
  private pointsOfPlate1: THREE.Vector2[];
  private pointsOfPlate2: THREE.Vector2[];
  private pointsOfPlate1_3d: THREE.Vector3[];
  private pointsOfPlate2_3d: THREE.Vector3[];
  private normal1: THREE.Vector2;
  private normal2: THREE.Vector2;
  private center1: THREE.Vector2;
  private center2: THREE.Vector2;

  constructor(plate1: Plate, plate2: Plate, pointsOfPlate1: THREE.Vector2[], pointsOfPlate2: THREE.Vector2[], pointsOfPlate1_3d: THREE.Vector3[], pointsOfPlate2_3d: THREE.Vector3[], normal1: THREE.Vector2, normal2: THREE.Vector2) {
    this.plate1 = plate1;
    this.plate2 = plate2;
    this.pointsOfPlate1 = pointsOfPlate1;
    this.pointsOfPlate2 = pointsOfPlate2;
    this.pointsOfPlate1_3d = pointsOfPlate1_3d;
    this.pointsOfPlate2_3d = pointsOfPlate2_3d;
    this.normal1 = normal1;
    this.normal2 = normal2;

    // compute centers
    if (this.pointsOfPlate1.length !== 2 || this.pointsOfPlate2.length !== 2) throw new Error("Joint not initialized with exactly 2 points (found " + this.pointsOfPlate1.length + " | " + this.pointsOfPlate2.length + ")");

    this.center1 = this.pointsOfPlate1[0].clone().add(this.pointsOfPlate1[1].clone()).divideScalar(2);
    this.center2 = this.pointsOfPlate2[0].clone().add(this.pointsOfPlate2[1].clone()).divideScalar(2);
  }

  public getPlates(): [Plate, Plate] {
    return [this.plate1, this.plate2];
  }

  public getOtherPlate(plate: Plate = this.plate1): Plate {
    if (plate === this.plate1) return this.plate2;
    if (plate === this.plate2) return this.plate1;
    throw new Error("Plate not part of this joint");
  }

  public getPoints(plate: Plate = this.plate1): THREE.Vector2[] {
    if (plate === this.plate1) return this.pointsOfPlate1;
    if (plate === this.plate2) return this.pointsOfPlate2;
    throw new Error("Plate not part of this joint");
  }

  public getPoints3d(plate: Plate = this.plate1): THREE.Vector3[] {
    if (plate === this.plate1) return this.pointsOfPlate1_3d;
    if (plate === this.plate2) return this.pointsOfPlate2_3d;
    throw new Error("Plate not part of this joint");
  }

  public getDirectionVector(plate: Plate = this.plate1): THREE.Vector3 {
    return this.getPoints3d(plate)[0].clone().sub(this.getPoints3d(plate)[1].clone());
  }

  public getOtherCorner3d(corner: THREE.Vector3): THREE.Vector3 {
    if (Util.eq(corner.distanceTo(this.pointsOfPlate1_3d[0]), 0)) return this.pointsOfPlate1_3d[1];
    
    if (Util.eq(corner.distanceTo(this.pointsOfPlate1_3d[1]), 0)) return this.pointsOfPlate1_3d[0];

    if (Util.eq(corner.distanceTo(this.pointsOfPlate2_3d[0]), 0)) return this.pointsOfPlate2_3d[1];

    if (Util.eq(corner.distanceTo(this.pointsOfPlate2_3d[1]), 0)) return this.pointsOfPlate2_3d[0];

    throw new Error("Point not part of this joint");
  }

  public getNormal(plate: Plate = this.plate1) {
    if (plate === this.plate1) return this.normal1;
    if (plate === this.plate2) return this.normal2;
    throw new Error("Plate not part of this joint");
  }

  public getCenter(plate: Plate = this.plate1) {
    if (plate === this.plate1) return this.center1;
    if (plate === this.plate2) return this.center2;
    throw new Error("Plate not part of this joint");
  }
}