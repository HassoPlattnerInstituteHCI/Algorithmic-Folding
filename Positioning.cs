using System;
using System.Collections.Generic;

namespace inClassHacking{
  class Positioning{
    double[] distances= new double[5];
    double[] input = new double[13];

    public Positioning(double[] distances, double[] input){
      this.distances = distances;
      this.input = input;
    }

    public List<Circle> calculateCirclePositioning(){
      double s, w, x, y, z;
      w=distances[0];
      x=distances[1];
      y=distances[2];
      z=distances[3];
      s=distances[4];

      List<Circle> circles = new List<Circle>();

      circles.Add(new Circle(new Point2D(s/2, 0), input[4]));
      circles.Add(new Circle(new Point2D(w, 0), input[0]));
      circles.Add(new Circle(new Point2D(0, x), input[1]));
      circles.Add(new Circle(new Point2D(0, s-y), input[2]));
      circles.Add(new Circle(new Point2D(z, s), input[3]));

      circles.Add(new Circle(new Point2D(s/2, calculate6thCirclesY(circles)), input[6]));
      circles.Add(new Circle(new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+input[7]+input[8]+input[9]), input[9]));
      double distanceAbove = input[10]+input[11];
      circles.Add(addLastCircleRigid(circles, distanceAbove, s)); 

      return circles;
    }

    double calculate6thCirclesY(List<Circle> circles){ //distance to cirlce 2 and 9 (top middle left and top middle right) important -> Pythagorean
      double c = circles[1].getRadius()+input[4]+input[5];
      double b = circles[1].getRadius()+circles[0].getRadius();
      double a = Math.Sqrt(c*c - b*b);

      return a;
    }    
//weiter runterrücken durch river
    Circle addLastCircleRigid(List<Circle> circles, double distanceAbove, double s){ 

      double radius = input[12];

      Circle helper = new Circle(new Point2D(s/2, circles[6].getCenter().y+circles[6].getRadius()+distanceAbove+radius), radius);

      double r = circles[4].getRadius();
      double a = Math.Sqrt((r+helper.getRadius())*(r+helper.getRadius()) - r*r);
      double b = Math.Sqrt(Math.Pow(circles[4].getCenter().getDistance(helper.getCenter()), 2) - r*r);
      double scaleFactor = b/a;
      radius = radius*scaleFactor;
      
      Circle circle = new Circle(new Point2D(s/2, circles[6].getCenter().y+circles[6].getRadius()+distanceAbove+radius), radius);

      Console.WriteLine("Changed radius from " + input[12] + " to " + radius + " to get a rigid packing.");

      return circle;
    }
  }
}