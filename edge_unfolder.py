# import CornerFinder from "../Util/CornerFinder";
# import Unfolding from "./Unfolding";
# import Joint from "../Model/Joint";
# import Plate from "../Model/Plate";
# import Queue from "../Util/Queue";
# import Unfolder from "./Unfolder";
# import * as THREE from 'three';


from Model.joint import Joint
from Nesting.Unfolding import Unfolding
import numpy as np

# export class SteepestEdgeUnfolder extends Unfolder {

#   public nest(plates: Plate[]): Unfolding {

#     let cutCounter = 0;
#     const directionVector = new THREE.Vector3(0, 0, 1);

#     // create a map of joints (edges) by their corners
#     const allowedJoints = new Set<Joint>();
#     const corners = CornerFinder.getCorners(plates, allowedJoints);
#     console.log("Got " + corners.size + " corners.");

class SteepestEdgeUnfolder:
  def nest(plates):
    cut_counter = 0
    direction_vector = np.array([0, 0, 1])

    allowed_joints = set()

    # TODO implement corner finder
    # corners = CornerFinder.get_corners(plates, allowed_joints)
    corners = []
    print("Got {} corners.".format(len(corners)))

#     // for each corner: delete the joint that is closest to the directionVector
#     for (const corner of corners) {
#       let steepest;
#       let steepestVal;

#       for (const joint of corner[1]) {

#         // don't cut the same joint twice
#         // if (!allowedJoints.has(joint)) continue;

#         const steepness = this.getSteepness(corner[0], joint, directionVector);

#         if (steepestVal == undefined || steepestVal < steepness) {
#           steepest = joint;
#           steepestVal = steepness;
#         }
#       }
#       if (steepest == undefined) {
#       } else {
#         if (allowedJoints.has(steepest)) cutCounter++;
#         allowedJoints.delete(steepest);
#       }
#     }

    # for each corner: delete the joint that is closest to the directionVector
    for corner in corners:
        steepest # TODO find different naming 
        steepest_val = 0

        for joint in corner[1]:
          # don't cut the same joint twice
          if not joint in allowed_joints:
            continue

          steepness = self._get_steepness(corner[0], joint, direction_vector)

          if steepest_val == None or steepest_val < steepness:
            steepest = allowed_joints
            steepest_val = steepness

        if steepest != None:
          if steepest in allowed_joints:
            cut_counter = cut_counter +1

          allowed_joints.remove(steepest)


#     console.log("Got " + (cutCounter + allowedJoints.size) + " joints, of which a total of " + cutCounter + " were cut. There are " + plates.length + " plates.");
    print("Got {}  joints, of which a total of {} were cut. There are {} plates."\
      .format(cut_counter + len(allowed_joints), cut_counter, len(plates)))

    # create set of remaining plates and allowed allowed joints
    remaining_plates = set()
    start_plate = plates[0]

#     const startPlate = plates[0];
#     for (let i = 1; i < plates.length; i++) remainingPlates.add(plates[i]);
    remaining_plates.update(plates[1:])

    # start at one plate and try to create an unfolding based on the remaining joints by doing something like DFS

#     const unfolding = new Unfolding(startPlate);
#     this.finishNesting(unfolding, startPlate, remainingPlates, allowedJoints);

    unfolding = Unfolding(start_plate)

    SteepestEdgeUnfolder._finish_nesting(unfolding, start_plate, remaining_plates, allowed_joints)

    print("The unfolding contains {} plates.".format(unfolding.get_plate_count()));

    return unfolding
#     return unfolding;
#   }


#   private finishNesting(unfolding: Unfolding, currentPlate: Plate, remainingPlates: Set<Plate>, allowedJoints: Set<Joint>): void {

#     for (const joint of currentPlate.getJoints()) {

#       const otherPlate = joint.getOtherPlate(currentPlate);
#       if (!(allowedJoints.has(joint) && remainingPlates.has(otherPlate))) continue;

#       if (unfolding.addPlateOnJoint(otherPlate, joint)) {
#         remainingPlates.delete(otherPlate);
#         this.finishNesting(unfolding, otherPlate, remainingPlates, allowedJoints);
#       }
#     }
#   }

  # does essentially DFS, using only joints that were not cut
  def _finish_nesting(unfolding, current_plate, remaining_plates, allowed_joints):
    for joint in current_plate.joints:
      # TODO think of sth cleaner than 'other plate'
      other_plate = joint.get_other_plate(current_plate)

      if not allowed_joints in joint and other_plate in remaining_plates:
        continue

      if unfolding.add_plate_on_joint(other_plate, joint):
        remaining_plates.remove(other_plate)
        SteepestEdgeUnfolder._finish_nesting(unfolding, other_plate, remaining_plates, allowed_joints)




#   private getSteepness(corner: THREE.Vector3, joint: Joint, directionVector: THREE.Vector3): number {

#     // get "v-w", where v equals corner
#     const jointVec = corner.clone().sub(joint.getOtherCorner3d(corner).clone());

#     const num = directionVector.clone().dot(jointVec.clone());
#     const denom = jointVec.clone().length();

#     return num / denom;
#   }
# }

  def _get_steepness(corner, joint, direction_vector):
    # TODO is direction vector the same as normal vector? 
    # TODO make this comment more helpful
    # TODO make variable names more descriptive 

    # get "v-w", where v equals corner
    joint_vec = corner - joint.get_ther_corner_3d(corner)

    return np.dot(direction_vector, joint_vec) / len(joint_vec)



# class DfsEdgeUnfolder:
#     def nest(plates):
      

# // Explores joint possibilities by DFS, but does not backtrack in the sense that already placed plates are removed from the unfolding. Hence, it does not always arrive at a solution, even if one exists.
# export class DfsEdgeUnfolder extends Unfolder {

#   public nest(plates: Plate[]): Unfolding {
#     // start at any plate
#     const startPlate = plates[0];
#     const unfolding = new Unfolding(startPlate);

#     // create a set of all plates and remove the startPlate
#     const plateSet = new Set<Plate>(plates);
#     plateSet.delete(startPlate);

#     // nest:
#     this.nestDFS(unfolding, plateSet, startPlate);

#     return unfolding;
#   }


#   private nestDFS(unfolding: Unfolding, plates: Set<Plate>, lastPlate: Plate): void {
#     for (const joint of lastPlate.getJoints()) {
#       if (plates.size === 0) return;

#       const otherPlate = joint.getOtherPlate(lastPlate);
#       let executed = false;
#       if (plates.has(otherPlate)) executed = unfolding.addPlateOnJoint(otherPlate, joint);

#       if (executed) {
#         plates.delete(otherPlate);
#         this.nestDFS(unfolding, plates, otherPlate);
#       }
#     }
#   }
# }


# // Explores joint possibilities by BFS, but does (again) not backtrack. Hence, it does not always arrive at a solution, even if one exists.
# export class BfsEdgeUnfolder extends Unfolder {

#   public nest(plates: Plate[]): Unfolding {
#     // create a set of all plates
#     const plateSet = new Set<Plate>(plates);

#     // nest:
#     return this.nestBFS(plateSet);
#   }

#   private nestBFS(plates: Set<Plate>): Unfolding {
#     // determine a start plate
#     const startPlate = plates.values().next().value;
#     const unfolding = new Unfolding(startPlate);

#     // create a queue and initialize with the connections of the start plate
#     const queue = new Queue<[Plate, Joint]>();
#     this.addPlateJointsToQueue(queue, startPlate);

#     while (!(queue.isEmpty() || plates.size === 0)) {
#       const [otherPlate, joint] = queue.dequeue();

#       let executed = false;
#       if (plates.has(otherPlate)) executed = unfolding.addPlateOnJoint(otherPlate, joint);

#       if (executed) {
#         plates.delete(otherPlate);
#         this.addPlateJointsToQueue(queue, otherPlate);
#       }
#     }
#     return unfolding;
#   }

#   private addPlateJointsToQueue(queue: Queue<[Plate, Joint]>, plate: Plate): void {
#     for (const joint of plate.getJoints()) {
#       const otherPlate = joint.getOtherPlate(plate);
#       queue.enqueue([otherPlate, joint]);
#     }
#   }
# }


# // Does essentially a brute force and always arrives at a solution (if one exists).
# export class BruteForceEdgeUnfolder extends Unfolder {

#   public nest(plates: Plate[]): Unfolding {
#     // start at any plate
#     const startPlate = plates[0];
#     const unfolding = new Unfolding(startPlate);

#     // create a set of all plates and remove the startPlate
#     const plateSet = new Set<Plate>(plates);
#     plateSet.delete(startPlate);

#     // nest:
#     this.nestBruteForce(unfolding, plateSet);

#     return unfolding;
#   }

#   private nestBruteForce(unfolding: Unfolding, plates: Set<Plate>): boolean {
#     if (plates.size === 0) return true;

#     for (const plate of plates) {
#       for (const joint of plate.getJoints()) {
#         // if plate can be laid out there, try to finish the unfolding
#         if (unfolding.addPlateOnJoint(plate, joint)) {
#           plates.delete(plate);
#           if (this.nestBruteForce(unfolding, plates)) return true;

#           // if the unfolding didn't work: backtrack
#           unfolding.deletePlate(plate);
#           plates.add(plate);
#         }
#       }
#     }
#     return false; // no solution found
#   }
# }