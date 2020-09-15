import Polygon from "../Model/Polygon";
import * as THREE from 'three';

export default class SvgCreator {
  // takes an array of polygons, and creates an SVG file (returned as string)
  public static getSvg(polygons: Polygon[]): string{
    if(polygons.length == 0) return "";

    const header = "<svg " + SvgCreator._getSize(polygons) + " xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n";
    const style =
      "  <style type=\"text/css\">\n" +
      "    .cut {stroke:red; stroke-width:0.01; fill:none}\n" +
      "  </style>\n";
    
    let paths = "";
    for (const p of polygons.map(poly => SvgCreator._polygonToPath(poly))) {
      paths += p;
    }

    const closeTag = "</svg>";
    
    return header + style + paths + closeTag;
  }

  private static _polygonToPath(polygon: Polygon): string {
    const points = polygon.getPoints();

    let path = "  <path class=\"cut\" d=\"";
    path += "M" + SvgCreator._pointText(points[0]) + " ";

    for (let i = 1; i < points.length; i++){
      path += "L" + SvgCreator._pointText(points[i]) + " ";
    }

    path += "L" + SvgCreator._pointText(points[0]) + " \"/>\n";
    return path;
  }
  
  private static _pointText(point: THREE.Vector2): string {
    const x = SvgCreator._round(point.x);
    const y = SvgCreator._round(point.y);
    return x + "," + y;
  }

  private static _getSize(polygons: Polygon[]): string{
    const points = [];
    polygons.forEach(poly => poly.getPoints().forEach(p => points.push(p)));

    let minX = points[0].x;
    let maxX = points[0].x;
    let minY = points[0].y;
    let maxY = points[1].y;

    for (const p of points) {
      minX = Math.min(minX, p.x);
      maxX = Math.max(maxX, p.x);
      minY = Math.min(minY, p.y);
      maxY = Math.max(maxY, p.y);
    }

    const width = SvgCreator._round(maxX) - SvgCreator._round(minX);
    const height = SvgCreator._round(maxY) - SvgCreator._round(minY);

    return "width=\"" + width + "\" height=\"" + height + "\"";
  }

  // round to 3 decimals
  private static _round(num: number): number {
    return (Math.round(num * 1000)) / 1000;
  }
}