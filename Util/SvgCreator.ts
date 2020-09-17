import Polygon from "../Model/Polygon";
import * as THREE from 'three';
import Util from "./Util";

export default class SvgCreator {
  // takes an array of polygons, and creates an SVG file (returned as string)
  public static getSvg(polygons: Polygon[], width: number, height: number): string{
    if(polygons.length == 0) return "";

    const header = "<svg width=\"" + Util.round(width) + "\" height=\"" + Util.round(height) + "\" xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n";
    const style =
      "  <style type=\"text/css\">\n" +
      "    .cut {stroke:red; stroke-width:0.01; fill:none}\n" +
      "  </style>\n";
    
    let paths = "";
    for (const p of polygons.map(poly => SvgCreator.polygonToPath(poly))) {
      paths += p;
    }

    const closeTag = "</svg>";
    
    return header + style + paths + closeTag;
  }

  private static polygonToPath(polygon: Polygon): string {
    const outline: string = this.pointsToPath(polygon.getPoints());
    const holePaths: string[] = polygon.getHoles().map(hole => this.pointsToPath(hole));

    if (holePaths.length > 0) {
      return outline + holePaths.reduce((p, n) => p + n);
    }
    else return outline;
  }

  private static pointsToPath(points: THREE.Vector2[]): string {
    let path = "  <path class=\"cut\" d=\"";
    path += "M" + SvgCreator.pointText(points[0]) + " ";

    for (let i = 1; i < points.length; i++){
      path += "L" + SvgCreator.pointText(points[i]) + " ";
    }

    path += "L" + SvgCreator.pointText(points[0]) + " \"/>\n";
    return path;
  }
  
  private static pointText(point: THREE.Vector2): string {
    const x = Util.round(point.x);
    const y = Util.round(point.y);
    return x + "," + y;
  }
}