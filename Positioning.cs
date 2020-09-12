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
      circles.Add(new Circle(new Point2D(s-z, s), input[3]));
      circles.Add(new Circle(new Point2D(s, s-y), input[2]));
      circles.Add(new Circle(new Point2D(s, x), input[1]));
      circles.Add(new Circle(new Point2D(s-w, 0), input[0]));
      circles.Add(new Circle(new Point2D(s/2, calculate10thCirclesY(circles)), input[6]));
      circles.Add(new Circle(new Point2D(s/2, circles[9].getCenter().y+circles[9].getRadius()+input[7]+input[8]+input[9]), input[9]));
      double distanceAbove = input[10]+input[11];
      circles.Add(addLastCircleRigid(circles, distanceAbove, s)); 

      Console.WriteLine(s-Math.Sqrt((circles[4].getRadius()+6.2)*(circles[4].getRadius()+6.2)-circles[4].getRadius()*circles[4].getRadius()));

      return circles;
    }

    double calculate10thCirclesY(List<Circle> circles){ //distance to cirlce 2 and 9 (top middle left and top middle right) important -> Pythagorean
      double c = circles[1].getRadius()+input[4]+input[5];
      double b = circles[1].getRadius()+circles[0].getRadius();
      double a = Math.Sqrt(c*c - b*b);

      return a;
    }    
//weiter runterrücken durch river
    Circle addLastCircleRigid(List<Circle> circles, double distanceAbove, double s){ 

      double radius = input[12];

      Circle c = new Circle(new Point2D(s/2, circles[10].getCenter().y+circles[10].getRadius()+distanceAbove+radius), radius);

      double r = circles[4].getRadius();
      double a = Math.Sqrt((r+c.getRadius())*(r+c.getRadius()) - r*r);
      double b = Math.Sqrt(Math.Pow(circles[4].getCenter().getDistance(c.getCenter()), 2) - r*r);
      double scaleFactor = Math.Round(b/a, 2);
      radius = radius*scaleFactor;
      
      Circle circle = new Circle(new Point2D(s/2, circles[10].getCenter().y+circles[10].getRadius()+distanceAbove+radius), radius);

      Console.WriteLine("Changed radius from " + input[12] + " to " + radius + " to get a rigid packing.");

      return circle;
      
      // Circle CircleA = circles[10];
      // Circle CircleB = circles[4];
      // Circle CircleC = circles[5];

      // Point2D pointA = CircleA.getCenter() + (new Point2D(0, CircleA.getRadius() + distanceAbove));

      // Point2D center = new Point2D(x, y);
      // Vector radiusVector = new Vector(center, pointC);
      // double radius = (radiusVector.getLength()>input[12])? radiusVector.getLength() : input[12];

      // Circle c = new Circle(center, radius);
      // return c;
    }

    public List<River> calculateRiverPositioning(List<Circle> circles){
      List<River> rivers = new List<River>();
//upper left
/*
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));
//middle left
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));
//lower left
      rivers.Add(new River(new Point2D(), new Point2D()));
//lower right
      rivers.Add(new River(new Point2D(), new Point2D()));
//middle right
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));
//upper right
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));
//upper middle
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));
//lower middle
      rivers.Add(new River(new Point2D(), new Point2D()));
      rivers.Add(new River(new Point2D(), new Point2D()));

      */
      return rivers;
    }


  }
}