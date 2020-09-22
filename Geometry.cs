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

        public static Point2D operator-(Point2D p, Vector v){
          return new Point2D(p.x-v.x, p.y-v.y);
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
      this.x = p2.x-p1.x;
      this.y = p2.y-p1.y;
    }

    public double getLength(){
      return Math.Sqrt(x*x + y*y);
    }

    public Vector normalized(){
      return this/this.getLength();
    }

    public Vector getNormalRight(){
      return new Vector(-this.y, this.x);
    }

    public Vector getNormalLeft(){
      return new Vector(this.y, -this.x);
    }

    public Vector getReverse(){
      return new Vector(-this.x, -this.y);
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

  public class Edge{

  public Point2D p1, p2;
  public int index1, index2;
  public Vector vec;

  public List<Point2D> markers; //represent inner nodes of the tree on the polygon's edge

  public Edge(Point2D p1, int index1, Point2D p2, int index2){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    this.p1 = p1;
    this.p2 = p2;
    this.index1 = index1;
    this.index2 = index2;
    this.markers = new List<Point2D>();
    this.vec = new Vector(p1, p2).normalized();
  }

  public Edge(Circle circle1, Circle circle2){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    this.p1 = circle1.getCenter();
    this.p2 = circle2.getCenter();
    this.index1 = circle1.getIndex();
    this.index2 = circle2.getIndex();
    this.markers = new List<Point2D>();
    this.vec = new Vector(p1, p2).normalized();
  }

  public Edge(Edge e){
    this.p1 = e.p1;
    this.p2 = e.p2;
    this.index1 = e.index1;
    this.index2 = e.index2;
    this.vec = e.vec;
    this.markers = new List<Point2D>(e.markers);
  }

  public double getLength(){
    return Math.Sqrt((p1.x-p2.x)*(p1.x-p2.x) + (p1.y-p2.y)*(p1.y-p2.y));
  }

  public void addMarker(Point2D marker){
    this.markers.Add(marker);
  }
    
  public void parallelSweep(double length){
    p1 += vec.getNormalLeft()*length;
    p2 += vec.getNormalLeft()*length;

    for(int i = 0; i<markers.Count; i++){
      markers[i] += vec.getNormalLeft()*length; 
    }
  }

  public void updateVertices(Edge left, Edge right){

    if( (right.vec.x != this.vec.x) && (right.vec.x != this.vec.getReverse().x)){
      this.p2 = Folding.findIntersection(right.vec.getReverse(), right.p2, this.vec, this.p1);
    }

    if( (left.vec.x != this.vec.x) && (left.vec.x != this.vec.getReverse().x)){
      this.p1 = Folding.findIntersection(left.vec, left.p1, this.vec.getReverse(), this.p2);
    }
    // Edge newEdge = new Edge(p1, p2);
    // return newEdge;
  }

  public void updateMarkers(){
    for(int i=0; i<this.markers.Count; i++){
      Point2D marker = this.markers[i];
      if(marker.x < this.p1.x && marker.x<this.p2.x){
        marker = null;
      }
    }
  }
}



  public class Circle {
    Point2D center;
    double radius;
    int index;

    public Circle(Point2D center, double radius){
      this.radius = radius;
      this.center = center;
    }

    public int getIndex(){return index;}
    public void setIndex(int i){this.index=i;}
    
    public Point2D getCenter(){
      return center;
    }
    public double getRadius(){
      return radius;
    }
    public void parallelSweep(Vector vec){
      this.center += vec;
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

  public enum Color{Red, Green, Blue, Yellow, Grey};

  public class Crease{
    public Point2D p1;
    public Point2D p2;
    public Color color;

    public Crease(Point2D p1, Point2D p2, Color c){
      this.p1 = p1;
      this.p2 = p2;
      this.color = c;
    }
  }
}