import Plate from "../Model/Plate";


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
}