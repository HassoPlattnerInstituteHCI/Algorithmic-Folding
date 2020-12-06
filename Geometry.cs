using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;
using Mathematics;

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
        public Point2D(Point2D p){
          this.x = p.x;
          this.y = p.y;
        }
        public double getDistance(Point2D target)
        {
            return Math.Sqrt(
                Math.Pow(Math.Abs(x - target.x), 2) +
                Math.Pow(Math.Abs(y - target.y), 2)
            );
        }
        public override bool Equals(Object obj){
           if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
               return false;
           else{
               Point2D p = (Point2D) obj;
               return (p.x == x & p.y == y);
               }
         }
         public override int GetHashCode()
         {
           throw new Exception("no hash code logic implemented yet");
         }
        public static bool operator==(Point2D p1, Point2D p2){
          if(object.ReferenceEquals(p2, null) && object.ReferenceEquals(p1, null))return true;
          if(object.ReferenceEquals(p1, null))return false;
          if(object.ReferenceEquals(p2, null))return false;
          return (Math.Round(p1.x, Defaults.precision) == Math.Round(p2.x,Defaults.precision) & Math.Round(p1.y, Defaults.precision) == Math.Round(p2.y, Defaults.precision));
        }

        public static bool operator!=(Point2D p1, Point2D p2){
          return !(p1==p2);
        }

        public static Point2D operator+(Point2D p1, Point2D p2){
          if(object.ReferenceEquals(p1, null))return null;
          if(object.ReferenceEquals(p2, null))return null;
          return new Point2D(p1.x+p2.x, p1.y+p2.y);
        }

        public static Point2D operator+(Point2D p, Vector v){
          if(object.ReferenceEquals(p, null))return null;
          if(object.ReferenceEquals(v, null))return null;
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
    public Vector perpendicular(){
      return new Vector(this.y, -this.x);
    }
    public Vector getReverse(){
      return new Vector(-this.x, -this.y);
    }
    public override bool Equals(Object obj){
       if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
           return false;
       else{
           Vector v = (Vector) obj;
           return (v.x == x & v.y == y);
           }
     }
     public override int GetHashCode()
     {
       throw new Exception("no hash code logic implemented yet");
     }
    public static Vector operator/(Vector v, double d){
      return new Vector(v.x/d, v.y/d);
    }
    public static Vector operator*(Vector v, double d){
      return new Vector(v.x*d, v.y*d);
    }
    public static bool operator==(Vector v1, Vector v2){
      if(object.ReferenceEquals(v2, null) && object.ReferenceEquals(v1, null))return true;
      if(object.ReferenceEquals(v1, null))return false;
      if(object.ReferenceEquals(v2, null))return false;
      return (Math.Round(v1.x, Defaults.precision) == Math.Round(v2.x,Defaults.precision) & Math.Round(v1.y, Defaults.precision) == Math.Round(v2.y, Defaults.precision));
    }
    public static bool operator!=(Vector v1, Vector v2){
      return !(v1==v2);
    }
    public override string ToString(){
      return ("(" + this.x + " ," + this.y + ")");
    }
  }
  public class PolygonEdge{
  public Point2D p1, p2;
  public int index;
  public Vector vec;
  public LeafNode n1, n2;
  public PolygonEdge left,right;
  public List<Point2D> markers; //represent inner nodes of the tree on the polygon's edges

  public PolygonEdge(Point2D p1, int index, Point2D p2){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    this.p1 = p1;
    this.p2 = p2;
    this.index = index;
    this.markers = new List<Point2D>();
    this.vec = new Vector(p1, p2).normalized();
    this.left = null;
    this.right = null;
  }
  public PolygonEdge(Point2D p1, PolygonEdge e, Point2D p2){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    this.p1 = p1;
    this.p2 = p2;
    this.index = e.index;
    this.n1 = e.n1;
    this.n2 = e.n2;
    this.markers = new List<Point2D>();
    this.vec = new Vector(p1, p2).normalized();
    this.left = null;
    this.right = null;
  }
  public PolygonEdge(LeafNode node1, LeafNode node2, int index){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    copy(new PolygonEdge(node1.circle.getCenter(), index, node2.circle.getCenter()));
    this.n1 = node1;
    this.n2 = node2;
  }
  private void copy(PolygonEdge e){
    this.p1 = e.p1;
    this.p2 = e.p2;
    this.index = e.index;
    this.markers = e.markers;
    this.vec = e.vec;
  }
  public PolygonEdge(PolygonEdge e){
    this.p1 = e.p1;
    this.p2 = e.p2;
    this.index = e.index;
    this.vec = e.vec;
    this.n1 = e.n1;
    this.n2 = e.n2;
    this.left = e.left;
    this.right = e.right;
    this.markers = new List<Point2D>(e.markers);
  }
  public double getLength(){
    return Math.Sqrt((p1.x-p2.x)*(p1.x-p2.x) + (p1.y-p2.y)*(p1.y-p2.y));
  }
  public void setMarker(Point2D marker){
    foreach(var m in this.markers){ //dont add twice
      if(m == marker) return;
    }
    this.markers.Add(marker);
  }
  public void parallelSweep(double length){
    p1 += vec.perpendicular()*length;
    p2 += vec.perpendicular()*length;
    for(int i = 0; i<markers.Count; i++)
      if(!(markers[i] == null))
        markers[i] += vec.perpendicular()*length;
  }
  public void removeOverlap(PolygonEdge left, PolygonEdge right, double sweepingLength){
    if( (right.vec != this.vec) && (right.vec!= this.vec.getReverse())){
      this.p2 = Geometry.findIntersection(right.vec.getReverse(), right.p2, this.vec, this.p1);
    }
    if( (left.vec != this.vec) && (left.vec != this.vec.getReverse())){
      this.p1 = Geometry.findIntersection(left.vec, left.p1, this.vec.getReverse(), this.p2);
    }
    updateMarkers(sweepingLength);
  }
  public void updateMarkers(double epsilon){
    for(int i=0; i<this.markers.Count; i++){
      if(!(this.markers[i] == null)){
        Point2D marker = this.markers[i];
        if(marker.x < this.p1.x-epsilon && marker.x < this.p2.x-epsilon)
          this.markers[i] = null;
        else if(marker.x > this.p1.x+epsilon && marker.x > this.p2.x+epsilon)
          this.markers[i] = null;
        else if(marker.y < this.p1.y-epsilon && marker.y < this.p2.y-epsilon)
          this.markers[i] = null;
        else if(marker.y > this.p1.y+epsilon && marker.y > this.p2.y+epsilon)
          this.markers[i] = null;
      }
    }
  }
}
  public class Circle {
    Point2D center;
    double radius;
    public Circle next;
    public LeafNode node;
    public Circle(Point2D center, double radius){
      this.radius = radius;
      this.center = center;
    }
    public Circle(Circle c){
      this.copy(new Circle (c.center,c.radius));
    }
    public Circle (double x, double y, double r){
      this.copy(new Circle(new Point2D(x,y),r));
    }
    public Point2D getCenter(){ return center;}
    public void setCenter(Point2D c){this.center = c;}
    public double getRadius(){return radius;}
    private void copy (Circle c){
      this.center = c.center;
      this.radius = c.radius;
    }
  }
  public class Crease{
    public Point2D p1,p2;
    public Color color;
    public Vector direction;
    public Crease(Point2D p1, Point2D p2, Color c){
      this.p1 = p1;
      this.p2 = p2;
      this.color = c;
      this.direction = new Vector (p1,p2);
    }
    public Crease(Crease c){
      copy(new Crease(c.p1,c.p2,c.color));
    }
    public bool similarDirection(Crease cr){
      return ((cr.direction.normalized() == this.direction.normalized()));
    }
    public bool containsPoint(Point2D p){
      return (p == p1 || p==p2);
    }
    public bool isColinearWith(Crease cr){
      return ((cr.color == this.color) && (cr.containsPoint(this.p1) || (cr.containsPoint(this.p2)))&& cr.similarDirection(this));
    }
    private void copy(Crease c){
      this.p1 = c.p1;
      this.p2 = c.p2;
      this.color = c.color;
    }
  }
  public class Geometry{
   public static Point2D findIntersection(Vector v1, Point2D p1, Vector v2, Point2D p2){
      Sparse2DMatrix<int, int, double> aMatrix = new Sparse2DMatrix<int, int, double>();
      aMatrix[0, 0] = v1.x; aMatrix[0, 1] = v2.x;
      aMatrix[1, 0] = v1.y; aMatrix[1, 1] = v2.y;
      SparseArray<int, double> bVector = new SparseArray<int, double>();
      bVector[0] = p2.x-p1.x; bVector[1] = p2.y-p1.y;
      SparseArray<int, double> xVector = new SparseArray<int, double>();
      int numberOfEquations = 2;
      // Solve the simultaneous equations.
      LinearEquationSolver.Solve(numberOfEquations,
                                      aMatrix,
                                      bVector,
                                      xVector);
      Point2D r = p1+v1*xVector[0];
      return r;
    }
  }
}
