using System;
using System.Collections.Generic;

namespace inClassHacking{
  class Positioning{
    double[] distances= new double[5];
    // static double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4};

    public Positioning(double[] distances, double[] input){
      this.distances = distances;
      // this.input = input;
    }

    public static List<Circle> calculateCirclePositioning(){
      double[] distances = {9.63, 2.7, 14.56, 6.63, 29.26};
      double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4};
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

      circles.Add(new Circle(new Point2D(s/2, Positioning.calculate6thCirclesY(circles)), input[6]));
      circles.Insert(5, new Circle(new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+input[7]+input[8]+input[9]), input[9]));
      double distanceAbove = input[10]+input[11];
      circles.Insert(5, Positioning.addLastCircleRigid(circles, distanceAbove, s)); 

      for(int i=0; i<circles.Count;i++){
        circles[i].setIndex(i);
      }

      return circles;
    }

    static double calculate6thCirclesY(List<Circle> circles){ //distance to cirlce 2 and 9 (top middle left and top middle right) important -> Pythagorean
      double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4};
      double c = circles[1].getRadius()+input[4]+input[5];
      double b = circles[1].getRadius()+circles[0].getRadius();
      double a = Math.Sqrt(c*c - b*b);

      return a;
    }    
//weiter runterrücken durch river
    static Circle addLastCircleRigid(List<Circle> circles, double distanceAbove, double s){ 

      double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4};

      double radius = input[12];

      Circle helper = new Circle(new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+distanceAbove+radius), radius);

      double r = circles[4].getRadius();
      double a = Math.Sqrt((r+helper.getRadius())*(r+helper.getRadius()) - r*r);
      double b = Math.Sqrt(Math.Pow(circles[4].getCenter().getDistance(helper.getCenter()), 2) - r*r);
      double scaleFactor = b/a;
      radius = radius*scaleFactor;
      
      Circle circle = new Circle(new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+distanceAbove+radius), radius);

      if(radius<input[12]){
         radius = input[12];
         Console.WriteLine("RigidPacking not possible");
      }else{
        Console.WriteLine("Changed radius from " + input[12] + " to " + radius + " to get a rigid packing.");
      }
      return circle;
    }
  }
}