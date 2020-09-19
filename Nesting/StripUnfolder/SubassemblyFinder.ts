import * as jsgraphs from "js-graph-algorithms";
import Plate from "../../Model/Plate";
import GeometryUtil from "../../Util/GeometryUtil";
import Util from "../../Util/Util";

export default class SubassemblyFinder {

  public static findSubassemblies(plates: Plate[]): Plate[][] {

    // each plate gets an integer ID
    const indexes = new Map<Plate, number>();
    plates.forEach((plate, index) => indexes.set(plate, index));

    // convert all convex connections into a jsgraphs graph object
    const g = new jsgraphs.Graph(plates.length + 1);
    for (const plate of plates) {
      for (const joint of plate.getJoints()) {
        if (GeometryUtil.isJointConvex(joint)) g.addEdge(indexes.get(plate), indexes.get(joint.getOtherPlate(plate)));
      }
    }

    // create connected components, each of which is a true subassembly
    const cc = new jsgraphs.ConnectedComponents(g);
    const subassemblies = new Map<number, Plate[]>();

    for (const plate of plates) {
      const sub: number = cc.componentId(indexes.get(plate));
      if (!subassemblies.has(sub)) subassemblies.set(sub, []);
      subassemblies.get(sub).push(plate);
    }
    // Original implementation sometimes re-merges subassemblies... Do we need this?

    const subassemblyArray = Array.from(subassemblies.values());
    console.log("Found " + subassemblies.size + " subassemblies:\n");
    Util.logPlateGroups(subassemblyArray);

    return subassemblyArray;
  }
}