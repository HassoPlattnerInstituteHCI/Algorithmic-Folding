// - find shortest paths
// - cut and regenerate geometry
// - do some random EdgeUnfolding


// Explores joint possibilities by DFS, but does not backtrack in the sense that already placed plates are removed from the unfolding. Hence, it does not always arrive at a solution, even if one exists.
import * as THREE from 'three';
import * as mathjs from "mathjs";
import Unfolder from "./Unfolder";
import Plate from "../Model/Plate";
import Unfolding from "./Unfolding";
import Joint from "../Model/Joint";
import CornerFinder from "../Util/CornerFinder";
import Util from "../Util/Util";
import Polygon from "../Model/Polygon";

export type JointCut = [THREE.Vector3, Joint];

export interface CutPath {
  start: THREE.Vector3;
  jointCuts: JointCut[];
  end: THREE.Vector3;
}

export class StarUnfolder extends Unfolder {

  public nest(plates: Plate[]): Unfolding {

    // choose the center of any plate as source point and add it to 3d corners of the centerPlate
    const centerPlate = plates[0];
    const center = centerPlate.getCenter();


    // determine the shortest path between each corners and the center and remember the cut-paths
    const corners = CornerFinder.getCorners(plates);
    const cutPaths: CutPath[] = [];

    for (const corner of corners.keys()) {
      const [cornerPlate1, cornerPlate2] = corners.get(corner)[0].getPlates();
      const plateSet1 = new Set<Plate>(plates);
      const plateSet2 = new Set<Plate>(plates);
      plateSet1.delete(cornerPlate1);
      plateSet2.delete(cornerPlate2);

      const pathJoints1 = this.findShortestPath(cornerPlate1, centerPlate, plateSet1);
      const pathJoints2 = this.findShortestPath(cornerPlate2, centerPlate, plateSet2);

      const cornerPlate = (pathJoints1.length < pathJoints2.length)? cornerPlate1 : cornerPlate2;
      const pathJoints = (pathJoints1.length < pathJoints2.length)? pathJoints1 : pathJoints2;

      const cuts = this.findCutsOnPath(center, corner, centerPlate, cornerPlate, new Set(pathJoints), plates);
      cutPaths.push(cuts);
    }

    // split the model
    const cutPlates = this.executeAllCutsOnModel(cutPaths, plates);
    const allowedJoints = new Set<Joint>();
    cutPlates.forEach(p => p.getJoints().forEach(j => allowedJoints.add(j)));

    const unfolding = new Unfolding(cutPlates[0]);
    const plateSet = new Set<Plate>(cutPlates);
    plateSet.delete(cutPlates[0]);

    this.finishNesting(unfolding, cutPlates[0], plateSet, allowedJoints);
    return unfolding;
  }


  // assumes that there are no intersecting cutlines
  private executeAllCutsOnModel(cutPaths: CutPath[], initialPlates: Plate[]): Plate[] {

    // each cutPath will produce a valid state, hence the cutPaths also have to be updated
    const cutMap = new Map<Joint, Set<JointCut>>();

    for (const cutPath of cutPaths) {
      for (const cut of cutPath.jointCuts) {
        if (!cutMap.has(cut[1])) cutMap.set(cut[1], new Set<JointCut>());
        cutMap.get(cut[1]).add(cut);
      }
    }

    // do the cuts
    let plates = initialPlates;
    for (const cutPath of cutPaths) {
      plates = this.cutPlates(cutPath, cutMap, plates);
    }

    return plates;
  }


  private cutPlates(cutPath: CutPath, cutMap: Map<Joint, Set<JointCut>>, plates: Plate[]): Plate[] {
    const cuts = new Array<[THREE.Vector3, THREE.Vector3, Plate]>();

    // for all JointCuts, create two joint instances and update the joints of the two related plates
    for (const jointCut of cutPath.jointCuts) {
      const newJoints = this.splitJoint(jointCut, cutMap);
      const [plate1, plate2] = jointCut[1].getPlates();

      const plate1Joints = plate1.getJoints().filter(j => j !== jointCut[1]);
      const plate2Joints = plate2.getJoints().filter(j => j !== jointCut[1]);

      plate1Joints.push(...newJoints);
      plate2Joints.push(...newJoints);

      plate1.setJoints(plate1Joints);
      plate2.setJoints(plate2Joints);
    }


    // add all cuts
    if (cutPath.jointCuts.length > 0) {
      cuts.push([cutPath.start, cutPath.jointCuts[0][0], Util.findCommonPlate(cutPath.start, cutPath.jointCuts[0])]);
      for (let i = 1; i < cutPath.jointCuts.length; i++) {
        const jointPlates = new Set(cutPath.jointCuts[i-1][1].getPlates());
        const joint2Plates = cutPath.jointCuts[i][1].getPlates();
        const commonPlate = (jointPlates.has(joint2Plates[0]))? joint2Plates[0] : joint2Plates[1];
        cuts.push([cutPath.jointCuts[i-1][0], cutPath.jointCuts[i][0], commonPlate]);
      }
      cuts.push([cutPath.jointCuts[cutPath.jointCuts.length-1][0], cutPath.end, Util.findCommonPlate(cutPath.end, cutPath.jointCuts[cutPath.jointCuts.length-1])]);
    }
    else {
      try {
        cuts.push([cutPath.start, cutPath.end, Util.findPlateWithPoints(cutPath.start, cutPath.end, plates)]);
      }
      catch (e) { // this might be the very first plate, i.e. the start is not a cornerpoint of any plate yet
        const commonPlate = plates.find(p => Util.eq(p.getCenter().distanceTo(cutPath.start), 0));
        if (commonPlate === undefined) throw e;
        if (commonPlate.getPoints().findIndex(p => Util.eq(p.distanceTo(cutPath.end), 0)) === -1) throw e;

        cuts.push([cutPath.start, cutPath.end, commonPlate]);
      }
    }


    // now all the cutPaths can be done between "Corners" of plates
    const plateSet = new Set<Plate>(plates);
    for (const cut of cuts) {
      const [start, end, plate] = cut;

      if (plate.get2DShape().getHoles().length !== 0) throw new Error("Trying to split a plate with holes");

      const platePoints = plate.getPoints();
      const startIndex = platePoints.findIndex(p => Util.eq(p.distanceTo(start), 0));
      const endIndex = platePoints.findIndex(p => Util.eq(p.distanceTo(end), 0));

      // if start is not found, this is the very first cut (i.e. start is not a corner of the plate yet)
      // hence: create a new joint within the plate itself
      if (startIndex === -1) {
        // add points to 3d plate outline
        platePoints.splice(endIndex + 1, 0, start);
        platePoints.splice(endIndex + 2, 0, end);

        // add joint to plate
        const points = [end, start];
        const points2d = points.map(p => plate.map3dTo2d(p));
        /* const newJoint = new Joint(plate, plate, points2d, points2d.concat([]).reverse(), points, points.concat([]).reverse(), getNormal(points2d), getNormal(points2d).negate());
        plate.setJoints(plate.getJoints().concat([newJoint])); */

        // add points to 2d plate outline
        const startIndex2d = plate.get2DShape().getPoints().findIndex(p => Util.eq(p.distanceTo(points2d[1]), 0));
        plate.get2DShape().getPoints().splice(startIndex2d + 1, 0, points2d[1]);
        plate.get2DShape().getPoints().splice(startIndex2d + 2, 0, points2d[0]);

        continue;
      }


      const points1: THREE.Vector3[] = [platePoints[endIndex], platePoints[startIndex]];
      const points2: THREE.Vector3[] = [platePoints[startIndex], platePoints[endIndex]];

      let i = (startIndex+1) % platePoints.length;
      while (i !== endIndex) {
        points1.push(platePoints[i]);
        i = (i+1) % platePoints.length;
      }
      i = (i+1) % platePoints.length;
      while (i !== startIndex) {
        points2.push(platePoints[i]);
        i = (i+1) % platePoints.length;
      }

      const points1_2d = points1.map(p => plate.map3dTo2d(p));
      const points2_2d = points2.map(p => plate.map3dTo2d(p));

      // create the plates
      const plate1 = new Plate(plate.getId() + "1", points1, new Polygon(points1_2d), [], plate.getGlobalToLocalMatrix());
      const plate2 = new Plate(plate.getId() + "2", points2, new Polygon(points2_2d), [], plate.getGlobalToLocalMatrix());



      // map all the joints to the two new plates
      const joints1: Joint[] = [];
      const joints2: Joint[] = [];

      for (const joint of plate.getJoints()) {
        const [point1, point2] = joint.getPoints3d(plate);
        const point1In1 = points1.findIndex(p => Util.eq(p.distanceTo(point1), 0));
        const point2In1 = points1.findIndex(p => Util.eq(p.distanceTo(point2), 0));

        // if the joint is intra-plate, make it a proper joint now
        const [jointPlate1, jointPlate2] = joint.getPlates();
        /* if (jointPlate1 === jointPlate2) {

          joints1.push(joint);
          joints2.push(joint);

          if ((point1In1 + 1) % points1.length === point2In1) {
            joint.setPlates(plate1, plate2);
          }
          else {
            joint.setPlates(plate2, plate1);
          }
          continue;
        } */

        // otherwise: push it to the plate that "has" it and change ownership
        if (point1In1 !== -1 && point2In1 !== -1) {
          if (jointPlate1 === plate) joint.setPlates(plate1, jointPlate2);
          else joint.setPlates(jointPlate1, plate1);

          joints1.push(joint);
        }
        else {
          if (jointPlate1 === plate) joint.setPlates(plate2, jointPlate2);
          else joint.setPlates(jointPlate1, plate2);

          joints2.push(joint);
        }
      }

      // create the new joint between these two plates
      /* const [start2d, end2d] = [plate.map3dTo2d(start), plate.map3dTo2d(end)];
      const newJoint = new Joint(plate1, plate2, [start2d, end2d], [end2d, start2d], [start, end], [end, start], getNormal([start2d, end2d]), getNormal([end2d, start2d]));

      joints1.push(newJoint);
      joints2.push(newJoint); */

      plate1.setJoints(joints1);
      plate2.setJoints(joints2);

      plateSet.delete(plate);
      plateSet.add(plate1);
      plateSet.add(plate2);
    }
    return Array.from(plateSet);
  }

  private splitJoint(jointCut: JointCut, cutMap: Map<Joint, Set<JointCut>>): [Joint, Joint] {
    const joint = jointCut[1];

    const [plate1, plate2] = joint.getPlates();
    const newMiddle1 = plate1.map3dTo2d(jointCut[0]);
    const newMiddle2 = plate2.map3dTo2d(jointCut[0]);

    const points1 = joint.getPoints(plate1);
    const points2: THREE.Vector2[] = joint.getPoints(plate2);

    const points1_3d = joint.getPoints3d(plate1);
    const points2_3d = joint.getPoints3d(plate2);

    const normal1 = joint.getNormal(plate1);
    const normal2 = joint.getNormal(plate2);

    const newJoint1 = new Joint(plate1, plate2, [points1[0], newMiddle1], [newMiddle2, points2[1]], [points1_3d[0], jointCut[0]], [jointCut[0], points2_3d[1]], normal1, normal2);
    const newJoint2 = new Joint(plate1, plate2, [newMiddle1, points1[1]], [points2[0], newMiddle2], [jointCut[0], points1_3d[1]], [points2_3d[0], jointCut[0]], normal1, normal2);

    // update all joints in the cutMap
    cutMap.get(joint).delete(jointCut);
    cutMap.set(newJoint1, new Set());
    cutMap.set(newJoint2, new Set());
    const jointLength = points1_3d[1].clone().sub(points1_3d[0]).length();

    // find out if all other cuts lie on newJoint1 or newJoint2
    for (const otherCut of cutMap.get(joint)) {
      const jointVec = otherCut[0].clone().sub(points1_3d[0]);
      const percentage = (100 * jointVec.length()) / jointLength;

      cutMap.get(joint).delete(otherCut);

      if (Util.eq(percentage, 50)) throw new Error("Multiple JointCuts on same point");
      else if (percentage > 50) {
        cutMap.get(newJoint2).add(otherCut);
        otherCut[1] = newJoint2;
      }
      else {
        cutMap.get(newJoint1).add(otherCut);
        otherCut[1] = newJoint1;
      }
    }

    // update the plates by first updating the 3d points and then the 2d points
    for (const plate of [plate1, plate2]) {
      const points3d = plate.getPoints();
      const points2d = plate.get2DShape().getPoints();

      const pointToSearch3d = joint.getPoints3d(plate)[0];
      const pointToSearch2d = joint.getPoints(plate)[0];

      const newPoint2d = plate.map3dTo2d(jointCut[0]);

      const index3d = points3d.findIndex(p => Util.eq(p.distanceTo(pointToSearch3d), 0));
      const index2d = points2d.findIndex(p => Util.eq(p.distanceTo(pointToSearch2d), 0));

      points3d.splice(index3d + 1, 0, jointCut[0]);
      points2d.splice(index2d + 1, 0, newPoint2d);
    }

    return [newJoint1, newJoint2];
  }

  private findCutsOnPath(start: THREE.Vector3, target: THREE.Vector3, startPlate: Plate, targetPlate: Plate, pathJoints: Set<Joint>, plates: Plate[]): CutPath {

    // 1. create unfolding along those joints
    // 2. map 3d start and target to 2d points in Unfolding
    // 3. draw line and get 2d intersections with joints
    // 4. map 2d joint points to 3d

    // -> offer "map3dTo2d" in Unfolding (and plates), easy since the points are sorted (i.e. use vectors between points[0-1 and 0-2]

    // 1. create an Unfolding along the pathJoints, assume it will not overlap
    const plateSet = new Set<Plate>(plates);
    plateSet.delete(startPlate);

    const unfolding = new Unfolding(startPlate);
    this.finishNesting(unfolding, startPlate, plateSet, pathJoints);


    // 2. determine the intersections between all joints
    const start2d = unfolding.map3dToUnfolding(start, startPlate);
    const end2d = unfolding.map3dToUnfolding(target, targetPlate);

    const intersections: JointCut[] = [];

    for (const joint of pathJoints) {
      const plate = joint.getPlates()[0];
      const [jointStart, jointEnd] = joint.getPoints3d(plate);
      const [jointStart2d, jointEnd2d] = [jointStart, jointEnd].map(p => unfolding.map3dToUnfolding(p, plate));

      const intersect = mathjs.intersect([start2d.x, start2d.y], [end2d.x, end2d.y], [jointStart2d.x, jointStart2d.y], [jointEnd2d.x, jointEnd2d.y]);
      const scaleVec = (new THREE.Vector2(intersect[0] as number, intersect[1] as number)).sub(jointStart2d);
      const scalar = scaleVec.length() / (jointEnd2d.clone().sub(jointStart2d)).length();

      const intersect3d = jointStart.clone().add(jointEnd.clone().sub(jointStart).multiplyScalar(scalar));
      intersections.push([intersect3d, joint]);
    }

    return {start: start, jointCuts: intersections, end: target};
  }

  // brute forces the shortest path
  private findShortestPath(start: Plate, end: Plate, unvisitedPlates: Set<Plate>): Joint[] {
    if (start === end) return [];

    let shortest: Joint[] = null;

    for (const joint of start.getJoints()) {
      const otherPlate = joint.getOtherPlate(start);
      if (!unvisitedPlates.has(otherPlate)) continue;
      unvisitedPlates.delete(otherPlate);

      const path = this.findShortestPath(otherPlate, end, unvisitedPlates);
      unvisitedPlates.add(otherPlate);

      if (path === null) continue;
      path.push(joint);

      if (shortest === null || shortest.length > path.length) shortest = path;
    }

    return shortest;
  }

  // does essentially DFS, using only the allowed joints
  private finishNesting(unfolding: Unfolding, currentPlate: Plate, remainingPlates: Set<Plate>, allowedJoints: Set<Joint>): void {

    for (const joint of currentPlate.getJoints()) {

      const otherPlate = joint.getOtherPlate(currentPlate);
      if (!(allowedJoints.has(joint) && remainingPlates.has(otherPlate))) continue;

      if (unfolding.addPlateOnJoint(otherPlate, joint)) {
        remainingPlates.delete(otherPlate);
        this.finishNesting(unfolding, otherPlate, remainingPlates, allowedJoints);
      }
    }
  }
}