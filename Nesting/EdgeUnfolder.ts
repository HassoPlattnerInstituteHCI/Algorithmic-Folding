import CornerFinder from "../Util/CornerFinder";
import Unfolding from "./Unfolding";
import Joint from "../Model/Joint";
import Plate from "../Model/Plate";
import Queue from "../Util/Queue";
import Unfolder from "./Unfolder";
import * as THREE from 'three';


export class SteepestEdgeUnfolder extends Unfolder {

  public nest(plates: Plate[]): Unfolding {

    let cutCounter = 0;
    const directionVector = new THREE.Vector3(0, 0, 1);

    // create a map of joints (edges) by their corners
    const allowedJoints = new Set<Joint>();
    const corners = CornerFinder.getCorners(plates, allowedJoints);
    console.log("Got " + corners.size + " corners.");

    // for each corner: delete the joint that is closest to the directionVector
    for (const corner of corners) {
      let steepest;
      let steepestVal;

      for (const joint of corner[1]) {

        // don't cut the same joint twice
        // if (!allowedJoints.has(joint)) continue;
        
        const steepness = this.getSteepness(corner[0], joint, directionVector);

        if (steepestVal == undefined || steepestVal < steepness){
          steepest = joint;
          steepestVal = steepness;
        }
      }
      if (steepest == undefined) {}
      else {
        if (allowedJoints.has(steepest)) cutCounter++;
        allowedJoints.delete(steepest);
      }
    }

    console.log("Got " + (cutCounter + allowedJoints.size) + " joints, of which a total of " + cutCounter + " were cut. There are " + plates.length + " plates.");

    // create set of remaining plates and allowed allowed joints
    const remainingPlates = new Set<Plate>();
    const startPlate = plates[0];
    for (let i = 1; i < plates.length; i++) remainingPlates.add(plates[i]);

    // start at one plate and try to create an unfolding based on the remaining joints by doing something like DFS
    const unfolding = new Unfolding(startPlate);
    this.finishNesting(unfolding, startPlate, remainingPlates, allowedJoints);

    console.log("The unfolding contains " + unfolding.getPlateCount() + " plates");

    return unfolding;
  }

  /* public nest2(plates: Plate[]): Unfolding {

    const directionVector = new THREE.Vector3(1, 0, 0);

    // get unique joints
    const joints = new Set<Joint>();

    for (const plate of plates) {
      for (const joint of plate.getJoints()) {
        joints.add(joint);
      }
    }

    // for each polygon: delete the joint that is closest to the directionVector
    const deletedJoints = new Set<Joint>();
    for (const plate of plates) {
      let closest;
      let closestAngle;

      for (let joint of plate.getJoints()) {

        const vec = joint.getDirectionVector();
        const angle = GeometryUtil.angleBetween(directionVector, vec) % 180;

        if (closest == undefined || closestAngle > angle){
          closest = joint;
          closestAngle = angle;
        }
      }

      deletedJoints.add(closest);
    }

    console.log("Got " + joints.size + " joints, of which a total of " + deletedJoints.size + " were cut. There are " + plates.length + " plates.");

    // create set of remaining plates and allowed allowed joints
    const remainingPlates = new Set<Plate>();
    const allowedJoints = new Set<Joint>();

    const startPlate = plates[0];
    for (let i = 1; i < plates.length; i++) remainingPlates.add(plates[i]);
    for (const joint of joints) {
      if (!deletedJoints.has(joint)) allowedJoints.add(joint);
    }


    // start at one plate and try to create an unfolding based on the remaining joints by doing something like DFS
    const unfolding = new Unfolding(startPlate);
    this.finishNesting(unfolding, startPlate, remainingPlates, allowedJoints);

    console.log("The unfolding contains " + unfolding.getPlateCount() + " plates");

    return unfolding;
  } */

  // does essentially DFS, using only joints that were not cut
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

  private getSteepness(corner: THREE.Vector3, joint: Joint, directionVector: THREE.Vector3): number {
    
    // get "v-w", where v equals corner
    const jointVec = corner.clone().sub(joint.getOtherCorner3d(corner).clone());

    const num = directionVector.clone().dot(jointVec.clone());
    const denom = jointVec.clone().length();

    return num / denom;
  }
}


// Explores joint possibilities by DFS, but does not backtrack in the sense that already placed plates are removed from the unfolding. Hence, it does not always arrive at a solution, even if one exists.
export class DfsEdgeUnfolder extends Unfolder {

  public nest(plates: Plate[]): Unfolding {
    // start at any plate
    const startPlate = plates[0];
    const unfolding = new Unfolding(startPlate);

    // create a set of all plates and remove the startPlate
    const plateSet = new Set<Plate>();
    for (const plate of plates) plateSet.add(plate);
    plateSet.delete(startPlate);

    // nest:
    this.nestDFS(unfolding, plateSet, startPlate);

    return unfolding;
  }


  private nestDFS(unfolding: Unfolding, plates: Set<Plate>, lastPlate: Plate): void {
    for (const joint of lastPlate.getJoints()) {
      if (plates.size === 0) return;

      const otherPlate = joint.getOtherPlate(lastPlate);
      let executed = false;
      if (plates.has(otherPlate)) executed = unfolding.addPlateOnJoint(otherPlate, joint);

      if (executed) {
        plates.delete(otherPlate);
        this.nestDFS(unfolding, plates, otherPlate);
      }
    }
  }
}


// Explores joint possibilities by BFS, but does (again) not backtrack. Hence, it does not always arrive at a solution, even if one exists.
export class BfsEdgeUnfolder extends Unfolder {

  public nest(plates: Plate[]): Unfolding {
    // create a set of all plates
    const plateSet = new Set<Plate>();
    for (const plate of plates) plateSet.add(plate);

    // nest:
    return this.nestBFS(plateSet);
  }

  private nestBFS(plates: Set<Plate>): Unfolding {
    // determine a start plate
    const startPlate = plates.values().next().value;
    const unfolding = new Unfolding(startPlate);

    // create a queue and initialize with the connections of the start plate
    const queue = new Queue<[Plate, Joint]>();
    this.addPlateJointsToQueue(queue, startPlate);

    while (!(queue.isEmpty() || plates.size === 0)) {
      const [otherPlate, joint] = queue.dequeue();

      let executed = false;
      if (plates.has(otherPlate)) executed = unfolding.addPlateOnJoint(otherPlate, joint);

      if (executed) {
        plates.delete(otherPlate);
        this.addPlateJointsToQueue(queue, otherPlate);
      }
    }
    return unfolding;
  }

  private addPlateJointsToQueue(queue: Queue<[Plate, Joint]>, plate: Plate): void {
    for (const joint of plate.getJoints()) {
      const otherPlate = joint.getOtherPlate(plate);
      queue.enqueue([otherPlate, joint]);
    }
  }
}


// Does essentially a brute force and always arrives at a solution (if one exists).
export class BruteForceEdgeUnfolder extends Unfolder {

  public nest(plates: Plate[]): Unfolding {
    // start at any plate
    const startPlate = plates[0];
    const unfolding = new Unfolding(startPlate);

    // create a set of all plates and remove the startPlate
    const plateSet = new Set<Plate>();
    for (const plate of plates) plateSet.add(plate);
    plateSet.delete(startPlate);

    // nest:
    this.nestBruteForce(unfolding, plateSet);

    return unfolding;
  }
  
  private nestBruteForce(unfolding: Unfolding, plates: Set<Plate>): boolean {
    if(plates.size === 0) return true;

    for (const plate of plates) {
      for (const joint of plate.getJoints()) {
        // if plate can be laid out there, try to finish the unfolding
        if (unfolding.addPlateOnJoint(plate, joint)) {
          plates.delete(plate);
          if (this.nestBruteForce(unfolding, plates)) return true;
        
          // if the unfolding didn't work: backtrack
          unfolding.deletePlate(plate);
          plates.add(plate);
        }
      }
    }
    return false; // no solution found
  }
}