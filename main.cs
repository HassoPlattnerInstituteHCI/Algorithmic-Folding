using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {

    public static double[] input = {4, 4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 6.2};
    public static double s, w, x, y, z;
    public const bool DEBUG = true;
    static int zoomFactor = 200;

    public static void Main (string[] args) {

      double[] distances = calculateDistances(input);
      FileHandler f = new FileHandler(DEBUG, distances[4], zoomFactor); 

      List<Circle> circles = calculatePositioning(distances);
      List<River> rivers = new List<River>();

      f.exportSVG(circles, rivers);

    }

    static double[] calculateDistances(double[] input){
      //TODO: calculate distan
      double[] ret = {9.63, 2.7, 14.56, 6.63, 29.26};
      return ret;
    }

    static List<Circle> calculatePositioning(double[] args){
      w=args[0];
      x=args[1];
      y=args[2];
      z=args[3];
      s=args[4];

      List<Circle> ret = new List<Circle>();

      ret.Add(new Circle(new Point2D(s/2, 0), input[4]));
      ret.Add(new Circle(new Point2D(w, 0), input[0]));
      ret.Add(new Circle(new Point2D(0, x), input[1]));
      ret.Add(new Circle(new Point2D(0, s-y), input[2]));
      ret.Add(new Circle(new Point2D(z, s), input[3]));
       
      ret.Add(new Circle(new Point2D(s-z, s), input[3]));
      ret.Add(new Circle(new Point2D(s, s-y), input[2]));
      ret.Add(new Circle(new Point2D(s, x), input[1]));
      ret.Add(new Circle(new Point2D(s-w, 0), input[0]));
      ret.Add(new Circle(new Point2D(s/2, input[4]+input[5]+ret[0].getRadius()+input[6]), input[6]));
      ret.Add(new Circle(new Point2D(s/2, input[4]+input[5]+input[7]+input[8]+ret[0].getRadius()+2*ret[9].getRadius()+input[9]), input[9]));
      double distanceAbove = 2;
      ret.Add(addLastCircleRigid(ret, distanceAbove)); 

      return ret;
    }
//weiter runterrücken durch river
    static Circle addLastCircleRigid(List<Circle> ret, double distanceAbove){ 
      //position between 3 other circles based on center of circumscribed circle of triangle abc:
      //vertex a is lowest point of circleA (extra flap in center of beetle)
      //vertex b and c both are on circle b / c (both length-8 circles)

      Circle CircleA = ret[10];
      Circle CircleB = ret[4];
      Circle CircleC = ret[5];

      Point2D pointA = CircleA.getCenter() + (new Point2D(0, CircleA.getRadius()));

      pointA += new Point2D(0, distanceAbove);

      Vector vectorC = new Vector(pointA.x - CircleB.getCenter().x, pointA.y - CircleB.getCenter().y);
      Vector vectorB = new Vector(pointA.x - CircleC.getCenter().x, pointA.y - CircleB.getCenter().y);

      Point2D pointB = CircleB.getCenter()+vectorC.normalized()*CircleB.getRadius();
      Point2D pointC = CircleC.getCenter()+vectorB.normalized()*CircleC.getRadius();      

      //center of circumscribed circle (https://kilchb.de/faqmath4.php#:~:text=Antwort%3A%20Die%20drei%20Punkte%20seinen,des%20Umkreises%20der%20drei%20Punkte.)
      double u = (pointB.y-pointC.y)*(pointA.x*pointA.x + pointA.y*pointA.y) + (pointC.y-pointA.y)*(pointB.x*pointB.x + pointB.y*pointB.y) + (pointA.y - pointB.y) * (pointC.x*pointC.x + pointC.y*pointC.y);

      double v = (pointC.x-pointB.x) * (pointA.x*pointA.x + pointA.y*pointA.y) + (pointA.x - pointC.x) * (pointB.x*pointB.x + pointB.y*pointB.y) + (pointB.x - pointA.x) * (pointC.x*pointC.x + pointC.y*pointC.y);

      double d = pointA.x*pointB.y + pointB.x*pointC.y + pointC.x*pointA.y - pointA.x*pointC.y - pointB.x*pointA.y - pointC.x*pointB.y;

      double x = 0.5 * u/d;
      double y = 0.5 * v/d;

      Circle c = new Circle(new Point2D(x, y), input[12]);
      // Circle c = new Circle(new Point2D(s/2, ret[10].getCenter().y+ret[10].getRadius()+input[10]+input[11]+input[12]), input[12]);
      return c;
    }
  }
}