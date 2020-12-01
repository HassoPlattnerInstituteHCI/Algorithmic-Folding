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
        static int PRECISION= 5;
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

        public Point2D mirrored(double paperSize){
          return new Point2D(paperSize-this.x, this.y);
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
          return (Math.Round(p1.x, PRECISION) == Math.Round(p2.x,PRECISION) & Math.Round(p1.y, PRECISION) == Math.Round(p2.y, PRECISION));
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
    static int PRECISION= 5;
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
    public Vector getNormalRight(){
      return new Vector(-this.y, this.x);
    }

    public Vector getNormalLeft(){
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
      return (Math.Round(v1.x, PRECISION) == Math.Round(v2.x,PRECISION) & Math.Round(v1.y, PRECISION) == Math.Round(v2.y, PRECISION));
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
  public int index1, index2;
  public Vector vec;

  public List<Point2D> markers; //represent inner nodes of the tree on the polygon's edges

  public PolygonEdge(Point2D p1, int index1, Point2D p2, int index2){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    this.p1 = p1;
    this.p2 = p2;
    this.index1 = index1;
    this.index2 = index2;
    this.markers = new List<Point2D>();
    this.vec = new Vector(p1, p2).normalized();
  }
  public PolygonEdge flipped(){
    PolygonEdge e = new PolygonEdge(p2,index2,p1,index1);
    e.markers = this.markers;
    e.vec = new Vector(p2, p1).normalized();
    return e;
  }
  public PolygonEdge(Circle circle1, Circle circle2){ //Edge from p1 to p2, which are positions of two leaf nodes (circle's center)
    this.p1 = circle1.getCenter();
    this.p2 = circle2.getCenter();
    this.index1 = circle1.getIndex();
    this.index2 = circle2.getIndex();
    this.markers = new List<Point2D>();
    this.vec = new Vector(p1, p2).normalized();
  }
  public PolygonEdge(PolygonEdge e){
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

  public void setMarker(Point2D marker){
    foreach(var m in this.markers){ //dont add twice
      if(m == marker) return;
    }
    this.markers.Add(marker);
  }

  public void parallelSweep(double length){
    p1 += vec.getNormalLeft()*length;
    p2 += vec.getNormalLeft()*length;
    for(int i = 0; i<markers.Count; i++)
      if(!(markers[i] == null))
        markers[i] += vec.getNormalLeft()*length;
  }

  public void updateVertices(PolygonEdge left, PolygonEdge right){

    if( (right.vec != this.vec) && (right.vec!= this.vec.getReverse())){
      this.p2 = Geometry.findIntersection(right.vec.getReverse(), right.p2, this.vec, this.p1);
    }

    if( (left.vec != this.vec) && (left.vec != this.vec.getReverse())){
      this.p1 = Geometry.findIntersection(left.vec, left.p1, this.vec.getReverse(), this.p2);
    }
  }

  public void updateMarkers(){
    double epsilon = 0.2;
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
    int index;

    public LeafNode node;

    public Circle(Point2D center, double radius){
      this.radius = radius;
      this.center = center;
    }
    public Circle(Circle c){
      this.radius = c.radius;
      this.center = c.center;
    }
    public Circle (double x, double y, double r){
      this.center = new Point2D(x,y);
      this.radius = r;
    }
    public int getIndex(){return index;}
    public void setIndex(int i){this.index=i;}

    public Point2D getCenter(){
      return center;
    }
    public void setCenter(Point2D c){
      this.center = c;
    }
    public double getRadius(){
      return radius;
    }
    public void parallelSweep(Vector vec){
      this.center += vec;
    }
  }

  public class Crease{
    public Point2D p1;
    public Point2D p2;
    public Color color;
    public Vector direction;

    public Crease(Point2D p1, Point2D p2, Color c){
      this.p1 = p1;
      this.p2 = p2;
      this.color = c;
      this.direction = new Vector (p1,p2);
    }
    public Crease(Crease c){
      this.p1 = c.p1;
      this.p2 = c.p2;
      this.color = c.color;
      this.direction = new Vector (p1,p2);
    }

    public void update(Crease c){
      this.p1 = c.p1;
      this.p2 = c.p2;
      this.color = c.color;
      this.direction = c.direction;
    }
    public void elongate(Point2D fromPoint, Vector v){
      p1 = (fromPoint != p1) ? p1-direction+v : p1;
      p2 = (fromPoint != p2) ? p2-direction+v : p2;
      direction = new Vector (p1,p2);
    }

    public Point2D sharedPoint(Crease c){
      if (containsPoint(c.p1))
        return c.p1;
      if (containsPoint(c.p2))
        return c.p2;
      return null;
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
      LinearEquationSolverStatus solverStatus =
          LinearEquationSolver.Solve(numberOfEquations,
                                      aMatrix,
                                      bVector,
                                      xVector);

      Point2D r = p1+v1*xVector[0];
      // Console.WriteLine("Solution: s=" + xVector[0] + ", t=" + xVector[1] + r);

      return r;
    }
  }
}
