import Unfolding from "./Unfolding";
import Plate from "../Model/Plate";
import MultiUnfolding from "./MultiUnfolding";

export default abstract class Unfolder {
  public abstract nest(plates: Plate[]): Unfolding | MultiUnfolding;
}