using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace inClassHacking
{
    class Triangle 
    {
        public Point3D a, b, c;
        public double alpha, beta, gamma;
        public Triangle(Point3D a, Point3D b, Point3D c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            calcAngles();
        }
        public double getHeight()
        {
            // given alpha as top
            double h;
            h = a.getDistance(b) * Math.Sin(beta);
            return h;
        }
        public void calcAngles()
        {
            double ab = a.getDistance(b);
            double bc = b.getDistance(c);
            double ac = a.getDistance(c);
            // law of sines
            this.alpha = sineLaw(ab, ac, bc);
            this.beta  = sineLaw(ab, bc, ac);
            this.gamma = sineLaw(ac, bc, ab);
        }
        public double sineLaw(double adj_side1, double adj_side2, double opp_side)
        {
            return Math.Acos(
                (Math.Pow(adj_side1, 2) +
                 Math.Pow(adj_side2, 2) -
                 Math.Pow(opp_side, 2)
                ) / (2 * adj_side1 * adj_side2)
            );
        }

        public override string ToString(){
          return a.ToString() + ", " + b.ToString() + ", " + c.ToString();
        }
    }
    class Point3D
    {
        public double x, y, z; //changed to public
        public Point3D(double x,double y,double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public double getDistance(Point3D target)
        {
            double dist;
            dist = Math.Sqrt(
                Math.Pow(Math.Abs(x - target.x), 2) +
                Math.Pow(Math.Abs(y - target.y), 2) +
                Math.Pow(Math.Abs(z - target.z), 2)
            );
            return dist;
        }

        public static bool operator==(Point3D p1, Point3D p2){
          return (p1.x == p2.x & p1.y == p2.y & p1.z == p2.z);
        }
        public static bool operator!=(Point3D p1, Point3D p2){
          return !(p1==p2);
        }

        public static Point3D operator+(Point3D p1, Point3D p2){
          return new Point3D(p1.x+p2.x, p1.y+p2.y, p1.z+p2.z);
        }

        public static Point3D operator/(Point3D p, int i){
          return new Point3D(p.x/i, p.y/i, p.z/i);
        }

        public override string ToString(){
          return "(" + x + ", " + y + ", " + z + ")";
        }
    }
    public class Line
    {
        public double x1, x2, y1, y2;
        public Color c;
    }

    struct Neighbors{
      public int aSide, bSide, cSide; //index of DualgraphTriangles that share an edge 

      public Neighbors(int aSide, int bSide, int cSide){
        this.aSide = aSide;
        this.bSide = bSide;
        this.cSide = cSide;
      }
    }

  class DualgraphTriangle: Triangle{ //special class for calculation of dualgraph and hamiltonian refinement
    public Point3D center, centerOfEdgeA, centerOfEdgeB, centerOfEdgeC;
    public int index;
    public Neighbors neighbor;
    public List<Triangle> triangulation = new List<Triangle>(); 

    public DualgraphTriangle(Point3D a, Point3D b, Point3D c): base(a, b, c){
      index = -1;
      neighbor = new Neighbors(-1, -1, -1);
      calculateCenter();
      calculateCenterOfEdges();
      triangulate();
    }

    public DualgraphTriangle(Point3D a, Point3D b, Point3D c, int index): base(a, b, c){
      this.index = index;
      neighbor = new Neighbors(-1, -1, -1);
      calculateCenter();
      calculateCenterOfEdges();
      triangulate();
    }

    public DualgraphTriangle(Triangle triangle, int index): base(triangle.a, triangle.b, triangle.c){
      this.index = index;
      neighbor = new Neighbors(-1, -1, -1);
      calculateCenter();
      calculateCenterOfEdges();
      triangulate();
    }

    private void calculateCenter(){
      double centerX = (a.x + b.x + c.x)/3;
      double centerY = (a.y + b.y + c.y)/3;
      double centerZ = (a.z + b.z + c.z)/3;
      center = new Point3D(centerX,centerY, centerZ);
    }

    private void calculateCenterOfEdges(){
      centerOfEdgeA = (b+c)/2;
      centerOfEdgeB = (a+c)/2;
      centerOfEdgeC = (a+b)/2;
    }

    public void triangulate(){
      triangulation.Add(new Triangle(a, center, centerOfEdgeB));
      triangulation.Add(new Triangle(centerOfEdgeB, center, c));
      triangulation.Add(new Triangle(c, center, centerOfEdgeA));
      triangulation.Add(new Triangle(centerOfEdgeA, center, b));
      triangulation.Add(new Triangle(b, center, centerOfEdgeC));
      triangulation.Add(new Triangle(centerOfEdgeC, center, a));
    }

    public int getStartPoint(int index){ //returns position of first triangle out of triangulation-list that has to be added to the strip dependend on dualpath (index of triangle added before)
      if(index == -1){
        return 0;
      }else if(index == neighbor.aSide){
        neighbor.aSide = -1;
        return 3;
      }else if(index == neighbor.bSide){
        neighbor.bSide = -1;
        return 1;
      }else if(index == neighbor.cSide){
        neighbor.cSide = -1;
        return 5;
      }else{
        Console.WriteLine("Access to triangle does not match dualgraph. Trying to access " + this.index + " from " + index);
        return -1;
      }
    }

    public override string ToString(){
      return index.ToString() + ": " + base.ToString() + " center: " + center.ToString();
    }

  }
}