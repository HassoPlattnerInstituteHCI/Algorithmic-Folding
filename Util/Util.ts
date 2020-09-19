import Plate from "../Model/Plate";
import GeometryUtil from "./GeometryUtil";


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

  public static logPlateGroups(plateGroups: Plate[][]): void {
    console.log("["
      + plateGroups.reduce((prev: string, val: Plate[]) => prev + ", " + val.length + " plates", "").substr(2)
      + "]");
  }

  // for testing
  public static printJointCategories(plates: Plate[]): void {

    let convex = 0;
    let concave = 0;

    for (const plate of plates) {
      for (const joint of plate.getJoints()) {
        if (GeometryUtil.isJointConvex(joint, plate)) {
          convex++;
        } else {
          concave++;
        }
      }
    }

    if ((concave % 2 !== 0) || (convex % 2 !== 0)) console.warn("Uneven number of joints...");
    console.log("There are " + convex / 2 + " convex and " + concave / 2 + " concave joints");
  }
}