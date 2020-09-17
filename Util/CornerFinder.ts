import GeometryUtil from "./GeometryUtil";
import Plate from "../Model/Plate";
import Joint from "../Model/Joint";
import Util from "../Util/Util";
import * as THREE from 'three';

export default class CornerFinder {

  // finds corners from the joint-graph data structure, can also be used to get a set of all allowed (uncut) joints
  public static getCorners(plates: Plate[], uncutJoints: Set<Joint> = new Set()): Map<THREE.Vector3, Joint[]> {

    const corners = new Map<THREE.Vector3, Joint[]>();

    for (const plate of plates) {
      for (const joint of plate.getJoints()) {
        // only include each joint once
        if (uncutJoints.has(joint)) continue;
        uncutJoints.add(joint);

        // try to find an existing corner that this joint can be added to
        for (const point of joint.getPoints3d(plate)) {
          // if no corners exist: create the first one
          if (corners.size === 0) corners.set(point, [joint]);
          else { // try to find closest corner
            let closest;
            let closestDistance;

            for (const corner of corners.keys()) {
              const distance = GeometryUtil.distanceOf3d(corner, point);

              if (closestDistance === undefined || closestDistance > distance) {
                closest = corner;
                closestDistance = distance;
              }
            }

            // add joint to closest corner or create a new one (if not close enough)
            if (Util.eq(closestDistance, 0)) corners.get(closest).push(joint);
            else corners.set(point, [joint]);
          }
        }
      }
    }
    return corners;
  }
}