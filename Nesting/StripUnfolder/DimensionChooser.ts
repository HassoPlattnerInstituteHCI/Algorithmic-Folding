import Plate from "../../Model/Plate";
import PlateInfo from "./PlateInfo";
import * as THREE from "three";


export default class DimensionChooser {

  public static getWingDimension(plates: Plate[], cShapes: Plate[][], infoMap: Map<Plate, PlateInfo>): THREE.Vector3 {

    const plateSet = new Set<Plate>(plates);

    let xCount = 0;
    let xConnections = 0;
    let xCShapes = 0;

    let yCount = 0;
    let yConnections = 0;
    let yCShapes = 0;

    let zCount = 0;
    let zConnections = 0;
    let zCShapes = 0;

    // set count and connections (parallelism of normals)
    for (const plate of plates) {
      // count the connections
      let connections = 0;
      plate.getJoints().forEach(joint => {
        const otherPlate = joint.getOtherPlate(plate);
        if (plateSet.has(otherPlate)) connections++;
      });

      // count the directions and add the connections
      const info = infoMap.get(plate);
      if (info.isFacing("x")) {
        xCount++;
        xConnections += connections;
      }
      if (info.isFacing("y")) {
        yCount++;
        yConnections += connections;
      }
      if (info.isFacing("z")) {
        zCount++;
        zConnections += connections;
      }
    }

    // count the directions of the cShape-plates
    for (const cShape of cShapes) {
      for (const plate of cShape) {
        const info = infoMap.get(plate);
        if (info.isFacing("x")) xCShapes++;
        if (info.isFacing("y")) yCShapes++;
        if (info.isFacing("z")) zCShapes++;
      }
    }

    const totalConnections = xConnections + yConnections + zConnections;
    const totalCShapes = xCShapes + yCShapes + zCShapes;

    const xPot =
      xCount === 0 || xConnections === 0
        ? 0
        : xCount *
        (xConnections / (totalConnections / xCount) + (totalCShapes > 0 ? xCShapes / (totalCShapes / xCount) : 0));
    const yPot =
      yCount === 0 || yConnections === 0
        ? 0
        : yCount *
        (yConnections / (totalConnections / yCount) + (totalCShapes > 0 ? yCShapes / (totalCShapes / yCount) : 0));
    const zPot =
      zCount === 0 || zConnections === 0
        ? 0
        : zCount *
        (zConnections / (totalConnections / zCount) + (totalCShapes > 0 ? zCShapes / (totalCShapes / zCount) : 0));

    // eliminate the dimension with the lowest (strip) potential (i.e. use it as wing dimension)
    if (xPot <= yPot && xPot <= zPot) return new THREE.Vector3(1, 0, 0);
    else if (yPot <= xPot && yPot <= zPot) return new THREE.Vector3(0, 1, 0);
    else if (zPot <= xPot && zPot <= yPot) return new THREE.Vector3(0, 0, 1);
    else {
      return new THREE.Vector3(0, 1, 0);
    }
  }
}