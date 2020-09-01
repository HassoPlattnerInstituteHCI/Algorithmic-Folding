import GeometryUtil from "../Util/GeometryUtil";
import SvgCreator from "../Util/SvgCreator";
import Polygon from "../Model/Polygon";
import Plate from "../Model/Plate";
import Joint from "../Model/Joint";
import * as THREE from 'three';
import * as fs from 'fs';

// this class encapsulates an unfolding, i.e. one single 2d layout consisting of adjacently placed plates
export default class Unfolding {
  
  private placements: Map<Plate, THREE.Matrix4> = new Map();
  private placementPolygons: Map<Plate, Polygon> = new Map();
  private joints: Map<Plate, Joint> = new Map();

  // add the first plate
  constructor(startPlate: Plate) {
    this.placements.set(startPlate, new THREE.Matrix4());
    this.placementPolygons.set(startPlate, GeometryUtil.applyMatrix4ToPolygon(startPlate.get2DShape(), this.placements.get(startPlate)));
  }

  public getPlateCount(): number {
    return this.placements.size;
  }

  public deletePlate(plate: Plate): void {
    this.placements.delete(plate);
    this.placementPolygons.delete(plate);
  }

  // returns true if successful and false otherwise (if the new plate was already added before, if the other plate is not part of the unfolding yet or if there would be overlaps)
  public addPlateOnJoint(plate: Plate, joint: Joint): boolean {
    
    const otherPlate = joint.getOtherPlate(plate);
    if (!this.placements.has(otherPlate)) return false;
    if (this.placements.has(plate)) return false;

    const otherMatrix = this.placements.get(otherPlate);
    const otherPolygon = this.placementPolygons.get(otherPlate);

    const otherCenter = GeometryUtil.applyMatrix4(joint.getCenter(otherPlate).clone(), otherMatrix);
    const otherNormal = GeometryUtil.transformDirection(joint.getNormal(otherPlate).clone(), otherMatrix);


    const matrix = new THREE.Matrix4();
    const center = joint.getCenter(plate).clone();
    const normal = joint.getNormal(plate).clone();


    // 1. translate section center to zero
    GeometryUtil.makeTranslation(center.negate(), matrix);

    // 2. rotate to match the other section's normal
    const rotationAxis = new THREE.Vector3(0, 0, 1);
    GeometryUtil.makeRotationFromVectorsAroundNormal(normal, otherNormal.negate(), rotationAxis, matrix);


    // 3. translate to target (section center matches other section center)
    GeometryUtil.makeTranslation(otherCenter, matrix);
    
    
    // generate the new Polygon
    const newPolygon = GeometryUtil.applyMatrix4ToPolygon(plate.get2DShape(), matrix);
    


    // check for overlap with all existing
    /* for (const poly of this.placementPolygons.values()) {
      const overlap = poly.overlappingArea(newPolygon);
      if (overlap > 0.001) return false;
    } */

    // add plate to unfolding
    this.placements.set(plate, matrix);
    this.placementPolygons.set(plate, newPolygon);

    return true;
  }

  public saveSvg(fileName: string) {

    const polygons = Array.from(this.placementPolygons.values());
    const svgText = SvgCreator.getSvg(polygons);

    fs.writeFileSync(fileName, svgText);
  }
}