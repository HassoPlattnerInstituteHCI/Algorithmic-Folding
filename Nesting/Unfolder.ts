import Unfolding from "./Unfolding";
import Plate from "../Model/Plate";

export default abstract class Unfolder {
  public abstract nest(plates: Plate[]): Unfolding;
}