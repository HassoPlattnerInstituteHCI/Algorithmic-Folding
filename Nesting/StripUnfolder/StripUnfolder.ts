import {DfsEdgeUnfolder} from "../EdgeUnfolder";
import GeometryUtil from "../../Util/GeometryUtil";
import Unfolding from "../Unfolding";
import Plate from "../../Model/Plate";
import Unfolder from "../Unfolder";
import * as THREE from 'three';
import MultiUnfolding from "../MultiUnfolding";
import HoleFinder from "./HoleFinder";
import DimensionChooser from "./DimensionChooser";
import Util from "../../Util/Util";

export default class StripUnfolder extends Unfolder {

  public nest(plates: Plate[]): MultiUnfolding {
    const holeFinder = new HoleFinder(plates);
    const [holes, cShapes] = holeFinder.getGroups();
    const infoMap = holeFinder.getInfoMap();

    const unfoldings: Unfolding[] = [];

    const components = this.getComponents(plates, holes);
    Util.logPlateGroups(components);
    Util.logPlateGroups(holes);
    Util.logPlateGroups(cShapes);

    // use real subassembly
    // const components = SubassemblyFinder.findSubassemblies(plates);

    for (const subassembly of components) {
      const wingDimension = DimensionChooser.getWingDimension(subassembly, cShapes, infoMap);
      unfoldings.push(this.nestStrip(subassembly, wingDimension));
    }

    return new MultiUnfolding(unfoldings);
  }

  private nestStrip(plates: Plate[], wingDimension = new THREE.Vector3(0, 1, 0)): Unfolding {
    // find strip plates
    const stripPlates: Plate[] = [];
    const wingPlates: Plate[] = [];

    plates.forEach(plate => {
      if (GeometryUtil.vectorsParallel(plate.getNormal().clone(), wingDimension)) wingPlates.push(plate);
      else stripPlates.push(plate);
    });

    if (wingPlates.length === 0) console.warn("No wing plates found, make sure that the model is axis aligned");

    // sort stripPlates by count of connections, DESC
    stripPlates.sort((p1, p2) => p2.getJoints().length - p1.getJoints().length);

    // assume that you can fit all plates into the strip
    const strip = this.constructStrip(stripPlates);

    // add the wing plates
    this.addWingsToStrip(wingPlates, strip);

    if (strip.getPlateCount() < plates.length) console.warn("Strip only contains " + strip.getPlateCount() + " of " + plates.length + " plates");
    return strip;
  }

  // construct a strip using DFS
  private constructStrip(stripPlates: Plate[]): Unfolding {
    return new DfsEdgeUnfolder().nest(stripPlates);
  }

  private addWingsToStrip(wingPlates: Plate[], strip: Unfolding): void {

    for (const plate of wingPlates) {
      // get joints of this plate sorted by length
      const joints = plate.getJoints();
      joints.sort((j1, j2) => {
        const length1 = j1.getPoints()[1].clone().sub(j1.getPoints()[0]).length();
        const length2 = j2.getPoints()[1].clone().sub(j2.getPoints()[0]).length();
        return length2 - length1;
      });

      // if no joint works, the plate is not added
      for (const joint of joints) {
        if (strip.addPlateOnJoint(plate, joint)) break;
      }
    }
  }

  // subdivides the model by first taking out and then finding connected components in the remaining graph
  private getComponents(plates: Plate[], holes: Plate[][]): Plate[][] {
    const usedPlates = new Set<Plate>();
    holes.forEach(h => h.forEach(plate => usedPlates.add(plate)));

    const components: Plate[][] = [];
    components.push(...holes);

    for (const plate of plates) {
      if (usedPlates.has(plate)) continue;

      const component = [];
      this.addToComponent(plate, component, usedPlates);
      components.push(component);
    }

    return components;
  }

  // add to component with DFS
  private addToComponent(plate: Plate, comp: Plate[], usedPlates: Set<Plate>): void {
    if (usedPlates.has(plate)) return;

    comp.push(plate);
    usedPlates.add(plate);

    for (const joint of plate.getJoints()) {
      const other = joint.getOtherPlate(plate);
      this.addToComponent(other, comp, usedPlates);
    }
  }
}