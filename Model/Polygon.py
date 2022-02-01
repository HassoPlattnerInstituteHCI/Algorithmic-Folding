# import * as THREE from 'three';
# import jsts = require("jsts");

# export default class Polygon {

#   private readonly points: THREE.Vector2[];
#   private readonly holes: THREE.Vector2[][];

#   // needs clockwise points!
#   constructor(points: THREE.Vector2[] = [], holes: THREE.Vector2[][] = []) {
#     if (points.length == 0) throw new Error("Cannot create a polygon without points");
#     this.points = points;
#     this.holes = holes;
#   }


class Polygon:
  def __init__(self, points, holes):
    if len(points) == 0: 
      raise Error("Cannot create a polygon without points")
    self.points = points
    self.holes = holes

#   private static _createJstsPolygon(polygon: Polygon, geomFactory: jsts.geom.GeometryFactory): jsts.geom.Polygon {
#     const shellPoints = polygon.getPoints().concat([]);
#     shellPoints.push(polygon.getPoints()[0]);

#     const shellCoordinates = shellPoints.map(numberPair => {
#       return new jsts.geom.Coordinate(numberPair.x, numberPair.y);
#     });

#     const shell = geomFactory.createLinearRing(shellCoordinates);

#     const holes = polygon.getHoles().map(hole => {
#       const holePoints = hole.concat([]);
#       holePoints.push(hole[0]);

#       const holeCoordinates = holePoints.map(numberPair => {
#         return new jsts.geom.Coordinate(numberPair.x, numberPair.y);
#       });
#       return geomFactory.createLinearRing(holeCoordinates);
#     });

#     return geomFactory.createPolygon(shell, holes);
#   }

#   // always clone the points before mutating them
#   public getPoints(): THREE.Vector2[] {
#     return this.points;
#   }

#   public getHoles(): THREE.Vector2[][] {
#     return this.holes;
#   }

#   public overlappingArea(otherPolygon: Polygon): number {
#     const geometryFactory = new jsts.geom.GeometryFactory();

#     const jstsPolygon0 = Polygon._createJstsPolygon(this, geometryFactory);
#     const jstsPolygon1 = Polygon._createJstsPolygon(otherPolygon, geometryFactory);

#     const intersection = jstsPolygon0.intersection(jstsPolygon1);
#     return intersection.getArea();
#   }
# }