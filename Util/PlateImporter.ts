import GeometryUtil from "./GeometryUtil";
import Polygon from "../Model/Polygon";
import Plate from "../Model/Plate";
import Joint from "../Model/Joint";
import * as THREE from 'three';
import Util from "./Util";

const fs = require('fs');

export function importPlates(fileName: string) {
  // parse imported model and regenerate the mockups by re-creating all three objects
  const data = fs.readFileSync(fileName, 'utf8');
  const mockups: PlateMockup[] = JSON.parse(data).map(m => recreateMockup(m));

  // create the plates
  const plateMap = new Map<string, Plate>();

  for (const m of mockups) {
    plateMap.set(m.id, createJointLessPlate(m));
  }

  // parse joints
  for (const m of mockups) {
    setJoints(m, plateMap);
  }

  return Array.from(plateMap.values());
}


function recreateMockup(mockup: PlateMockup): PlateMockup {
  const points = mockup.points.map(p => clone3(p));

  const poly1 = mockup.polygon[0].map(p => clone2(p));
  const poly2 = mockup.polygon[1].map(arr => arr.map(p => clone2(p)));
  const polygon = [poly1, poly2];

  const joints = mockup.joints.map(joint => {
    const p1 = joint[1].map(p => clone2(p));
    const p2 = joint[2].map(p => clone2(p));
    const p3 = joint[3].map(p => clone3(p));
    const p4 = joint[4].map(p => clone3(p));
    return [joint[0], p1, p2, p3, p4];
  });

  return new PlateMockup(mockup.id, points, polygon, joints);
}

function clone2(origin: THREE.Vector2): THREE.Vector2 {
  return new THREE.Vector2(origin.x, origin.y);
}

function clone3(origin: THREE.Vector3): THREE.Vector3 {
  return new THREE.Vector3(origin.x, origin.y, origin.z);
}

function createJointLessPlate(mockup: PlateMockup): Plate {
  const poly = new Polygon(mockup.polygon[0], mockup.polygon[1]);
  return new Plate(mockup.id, mockup.points, poly, []);
}

function setJoints(mockup: PlateMockup, plateMap: Map<string, Plate>): void {

  const joints: Joint[] = [];
  const plate = plateMap.get(mockup.id);

  for (const joint of mockup.joints) {
    const otherPlate = plateMap.get(joint[0]);
    const otherJoint = otherPlate.getJoint(plate);

    if (otherJoint != undefined) { // don't create the same joint twice
      joints.push(otherJoint);
      continue;
    }

    const points1 = findPoints(plate, joint[1]);
    const points2 = findPoints(otherPlate, joint[2]);

    // not using actual references here, because they are not needed
    const points1_3d = joint[3];
    const points2_3d = joint[4];

    const normal1 = getNormal(points1);
    const normal2 = getNormal(points2);

    joints.push(new Joint(plate, otherPlate, points1, points2, points1_3d, points2_3d, normal1, normal2));
  }

  plate.setJoints(joints);
}

// try to find the original points (Vector2) (by reference) that are used in the polygon
function findPoints(plate: Plate, copyPoints: THREE.Vector2[]) {
  // collect all points (outline / hole) from the plates 2d geometry (polygon)
  const allPoints: THREE.Vector2[] = [];
  const polygon = plate.get2DShape();
  allPoints.push(...polygon.getPoints());

  for (const hole of polygon.getHoles()) {
    allPoints.push(...hole);
  }


  // now try to find the respective points
  const points: THREE.Vector2[] = [];

  for (const target of copyPoints) {
    let closest = allPoints[0];
    let bestDist = GeometryUtil.distanceOf2d(closest, target);

    for (const point of allPoints) {
      const newDist = GeometryUtil.distanceOf2d(point, target);
      if (newDist < bestDist) {
        closest = point;
        bestDist = newDist;
      }
    }

    if (!Util.eq(bestDist, 0)) throw new Error("Failed to import joint - could not find actual polygon point that is close enough (found best margin of " + bestDist + ")");

    points.push(closest);
  }

  return points;
}

function getNormal(points: THREE.Vector2[]): THREE.Vector2 {
  const points3 = points.map(v => new THREE.Vector3(v.x, v.y, 0));
  const normal = points3[1].sub(points3[0]).cross(new THREE.Vector3(0, 0, 1));
  normal.normalize();
  return new THREE.Vector2(normal.x, normal.y);
}

class PlateMockup {
  // imported attributes from json, the joints contain ids of other mockups to reconstruct the object references
  public id: string;
  public points: THREE.Vector3[];
  public polygon: [THREE.Vector2[], THREE.Vector2[][]];
  public joints: Array<[string, THREE.Vector2[], THREE.Vector2[], THREE.Vector3[], THREE.Vector3[]]>;

  constructor(id: string, points, polygon, joints) {
    this.id = id;
    this.points = points;
    this.polygon = polygon;
    this.joints = joints;
  }
}