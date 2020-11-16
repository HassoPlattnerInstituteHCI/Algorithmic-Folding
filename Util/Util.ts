import {JointCut} from "../Nesting/GeneralUnfolder";
import Plate from "../Model/Plate";
import * as THREE from 'three';

export default class Util {

  private static readonly epsilon = 0.0001;

  /**
   * Very basic utility methods
   */
  public static eq(num1: number, num2: number, epsilon: number = Util.epsilon): boolean {
    return (num1 - epsilon < num2 && num2 < num1 + epsilon);
  }

  // round to 3 decimals
  public static round(num: number): number {
    return (Math.round(num * 1000)) / 1000;
  }

  /**
   * Finds a plate that both points are on
   */
  public static findCommonPlate(obj1: THREE.Vector3 | JointCut, obj2: JointCut): Plate {

    if (obj1 instanceof THREE.Vector3) {
      return this.findPlateWithPoints(obj1, obj2[0], obj2[1].getPlates())
    }
    else { // both are JointCuts
      const jointPlates = new Set(obj1[1].getPlates());
      const joint2Plates = obj2[1].getPlates();

      if (jointPlates.has(joint2Plates[0])) return joint2Plates[0];
      else if (jointPlates.has(joint2Plates[1])) return joint2Plates[1];
      else throw new Error("JointCuts do not share a plate");
    }
  }

  public static findPlateWithPoints(point1: THREE.Vector3, point2: THREE.Vector3, plates: Plate[]): Plate {

    for (const plate of plates) {
      const index1 = plate.getPoints().findIndex(p => Util.eq(p.distanceTo(point1), 0));
      const index2 = plate.getPoints().findIndex(p => Util.eq(p.distanceTo(point2), 0));

      if (index1 !== -1 && index2 !== -1) return plate;
    }
    throw new Error("The given points do not share a plate");
  }
}