using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace inClassHacking
{

  public class Point2D
    {
        public double x, y; 
        public Point2D(double x,double y)
        {
            this.x = x;
            this.y = y;
        }
        public double getDistance(Point2D target)
        {
            double dist;
            dist = Math.Sqrt(
                Math.Pow(Math.Abs(x - target.x), 2) +
                Math.Pow(Math.Abs(y - target.y), 2)
            );
            return dist;
        }
    

        public static bool operator==(Point2D p1, Point2D p2){
          return (p1.x == p2.x & p1.y == p2.y);
        }
        public static bool operator!=(Point2D p1, Point2D p2){
          return !(p1==p2);
        }

        public static Point2D operator+(Point2D p1, Point2D p2){
          return new Point2D(p1.x+p2.x, p1.y+p2.y);
        }

        public static Point2D operator+(Point2D p, Vector v){
          return new Point2D(p.x+v.x, p.y+v.y);
        }

        public static Point2D operator/(Point2D p, int i){
          return new Point2D(p.x/i, p.y/i);
        }

        public override string ToString(){
          return "(" + x + ", " + y + ")";
        }
    }


  public class Vector{
    public double x, y;

    public Vector(double x, double y){
      this.x = x;
      this.y = y;
    }

    public Vector(Point2D p1, Point2D p2){
      this.x = p1.x-p2.x;
      this.y = p1.y-p2.y;
    }

    public double getLength(){
      return Math.Sqrt(x*x + y*y);
    }

    public Vector normalized(){
      return this/this.getLength();
    }

    public static Vector operator/(Vector v, double d){
      return new Vector(v.x/d, v.y/d);
    }
    public static Vector operator*(Vector v, double d){
      return new Vector(v.x*d, v.y*d);
    }
    public override string ToString(){
      return ("(" + this.x + " ," + this.y + ")");
    }

  }
  public class Circle {
    Point2D center;
    double radius;

    public Circle(Point2D center, double radius){
      this.radius = radius;
      this.center = center;
    }
    
    public Point2D getCenter(){
      return center;
    }
    public double getRadius(){
      return radius;
    }
  }

  public class River{ //rectangle in the .svg-file
    Point2D p1; //upper left corner
    Point2D p2; //lower right corner

    public River(Point2D p1, Point2D p2){
      this.p1 = p1;
      this.p2 = p2;
    }

    public Point2D getP1(){
      return p1;
    }

    public Point2D getP2(){
      return p2;
    }
    
  }

  public class Crease{
    public Point2D p1;
    public Point2D p2;
    public Color color;

    public Crease(Point2D p1, Point2D p2, Color c){
      this.p1 = p1;
      this.p2 = p2;
      this.color = color;
    }
  }
}