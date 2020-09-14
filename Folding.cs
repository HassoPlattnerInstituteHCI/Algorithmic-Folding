using System;
using System.Collections.Generic;
using SparseCollections;
using Mathematics;

namespace inClassHacking{

  class Folding{

    List<Circle> circles;
    double[] distances;
    double[] input;
    double s, w, x, y, z;

    List<Crease> creases = new List<Crease>();

    public Folding(List<Circle> circles, double[] input, double[] distances){
      this.circles = circles;
      this.input = input;
      this.distances = distances;
      w = distances[0];
      x = distances[1];
      y = distances[2];
      z = distances[3];
      s = distances[4];
    }

    public List<Crease> calculateCreases(){
      axialCreases(creases);
      lowerPart(creases);

      return creases;
    }

    void axialCreases(List<Crease> creases){
      creases.Add(new Crease(circles[2].getCenter(), circles[1].getCenter(), Color.Green));
      creases.Add(new Crease(circles[1].getCenter(), circles[5].getCenter(), Color.Green));
      creases.Add(new Crease(circles[7].getCenter(), circles[6].getCenter(), Color.Green));
      creases.Add(new Crease(circles[3].getCenter(), circles[4].getCenter(), Color.Green));
      creases.Add(new Crease(circles[3].getCenter(), circles[2].getCenter(), Color.Green));
      creases.Add(new Crease(circles[4].getCenter(), circles[7].getCenter(), Color.Green));
      creases.Add(new Crease(circles[5].getCenter(), circles[6].getCenter(), Color.Green));
    }

    void lowerPart(List<Crease> creases){
      
      Vector circle4To7 = new Vector(circles[4].getCenter(), circles[7].getCenter());
      Point2D edgeCircle4right = circles[4].getCenter() + circle4To7.normalized() * circles[4].getRadius();

      Vector up = new Vector(0, -1);
      Vector down = new Vector(0, 1);
      Vector left = new Vector(1, 0);
      Vector right = new Vector(-1, 0);

      Point2D lowerMiddle = new Point2D(s/2, s);

      Point2D helper3 = findIntersection(circle4To7.getNormalRight(), edgeCircle4right, up, lowerMiddle);  //between lowest 3 circles

      Vector circle4To3 = new Vector(circles[4].getCenter(), circles[3].getCenter());
      Point2D edgeCircle4left = circles[4].getCenter() + circle4To3.normalized()*circles[4].getRadius();
      
      Point2D helper1 = findIntersection(circle4To7.getNormalLeft(), edgeCircle4right, circle4To3.getNormalRight(), edgeCircle4left); //between lower left circles

      creases.Add(new Crease(helper1, circles[3].getCenter(), Color.Red));
      creases.Add(new Crease(helper1, circles[4].getCenter(), Color.Red));
      creases.Add(new Crease(helper1, circles[7].getCenter(), Color.Red));

      creases.Add(new Crease(helper3, circles[4].getCenter(), Color.Red));
      creases.Add(new Crease(helper3, circles[7].getCenter(), Color.Red));

      creases.Add(new Crease(helper3, new Point2D(s/2, s), Color.Blue));
      creases.Add(new Crease(helper1, helper3, Color.Blue));

      creases.Add(new Crease(helper1, edgeCircle4left, Color.Blue));

      Point2D edgeCircle3low = circles[3].getCenter() + circle4To3.getReverse().normalized()*circles[3].getRadius();
      Vector circle3ToHelper1 = new Vector(circles[3].getCenter(), helper1);
      Point2D helper2 = findIntersection(circle4To3.getNormalLeft(), edgeCircle3low, circle3ToHelper1, circles[3].getCenter());

      creases.Add(new Crease(helper2, edgeCircle3low, Color.Blue));

      //gussets
      creases.Add(new Crease(circles[3].getCenter(), circles[7].getCenter(), Color.Grey));


      //upper left corner

      Point2D edgeCircle2left = new Point2D(0, circles[2].getCenter().y+circles[2].getRadius());
      Vector circle2To1 = new Vector(circles[2].getCenter(), circles[1].getCenter());
      Point2D edgeCircle2right = circles[2].getCenter() + circle2To1.normalized()*circles[2].getRadius();      
      helper1 = findIntersection(right,  edgeCircle2left, circle2To1.getNormalRight(), edgeCircle2right);

      creases.Add(new Crease(helper1, circles[2].getCenter(), Color.Red));
      creases.Add(new Crease(helper1, edgeCircle2left, Color.Blue));
      creases.Add(new Crease(helper1, edgeCircle2right, Color.Blue));




      //upper middle
      Point2D helper4 = new Point2D(circles[0].getCenter().x-circles[0].getRadius(), circles[5].getCenter().y-circles[5].getRadius()-input[6]);

      creases.Add(new Crease(helper4, new Point2D(s/2, helper4.y), Color.Blue));

      creases.Add(new Crease(helper4, circles[0].getCenter(), Color.Red));
      creases.Add(new Crease(helper4, circles[1].getCenter(), Color.Red));
      creases.Add(new Crease(helper4, circles[5].getCenter(), Color.Red));

      creases.Add(new Crease(helper4, new Point2D(s/2, helper4.y), Color.Blue));

      //gusset (grey)
      creases.Add(new Crease(circles[0].getCenter(), circles[5].getCenter(), Color.Grey));

      Point2D edgeCircle5above = new Point2D(s/2, circles[5].getCenter().y-circles[5].getRadius());
      Point2D edgeCircle5below = new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius());
      Vector helper4ToCircle5 = new Vector(helper4, circles[5].getCenter());
      Point2D helper5 = findIntersection(left, edgeCircle5above, helper4ToCircle5, helper4);
      
      creases.Add(new Crease(edgeCircle5above, helper5, Color.Blue));

      Vector circle1To5 = new Vector(circles[1].getCenter(), circles[5].getCenter());
      Point2D helper6 = findIntersection(circle1To5.getNormalRight(), helper5, right, edgeCircle5below);

      creases.Add(new Crease(helper6, edgeCircle5below, Color.Blue));
      creases.Add(new Crease(helper6, helper5, Color.Blue));

      creases.Add(new Crease(helper6, circles[5].getCenter(), Color.Red));

      Vector circle1ToHelper6 = new Vector(circles[1].getCenter(), helper6);

      Point2D helper9 = findIntersection(circle1To5.getNormalRight(), helper4, circle1ToHelper6, helper6);

      creases.Add(new Crease(helper9, helper4, Color.Blue));
      
      Point2D edgeCircle6left = new Point2D(circles[6].getCenter().x-circles[6].getRadius(), circles[6].getCenter().y);

      Vector edge6ToHelper6 = new Vector(edgeCircle6left, helper6);
      Point2D middleInput7 = new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+input[7]);
      Point2D helper7 = findIntersection(edge6ToHelper6, edgeCircle6left, left, middleInput7);
      Point2D middleInput8 = new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+input[7]+input[8]);
      Point2D helper8 = findIntersection(edge6ToHelper6, edgeCircle6left, left, middleInput8);

      creases.Add(new Crease(helper6, edgeCircle6left, Color.Red));
      creases.Add(new Crease(edgeCircle6left, circles[6].getCenter(), Color.Red));
      creases.Add(new Crease(middleInput7, helper7, Color.Blue));
      creases.Add(new Crease(middleInput8, helper8, Color.Blue));

    }

    public Point2D calculateCenterOfCircumsizedCircle(Point2D pointA, Point2D pointB, Point2D pointC){
      //center of circumscribed circle (https://kilchb.de/faqmath4.php#:~:text=Antwort%3A%20Die%20drei%20Punkte%20seinen,des%20Umkreises%20der%20drei%20Punkte.)
      double u = (pointB.y-pointC.y)*(pointA.x*pointA.x + pointA.y*pointA.y) + (pointC.y-pointA.y)*(pointB.x*pointB.x + pointB.y*pointB.y) + (pointA.y - pointB.y) * (pointC.x*pointC.x + pointC.y*pointC.y);

      double v = (pointC.x-pointB.x) * (pointA.x*pointA.x + pointA.y*pointA.y) + (pointA.x - pointC.x) * (pointB.x*pointB.x + pointB.y*pointB.y) + (pointB.x - pointA.x) * (pointC.x*pointC.x + pointC.y*pointC.y);

      double d = pointA.x*pointB.y + pointB.x*pointC.y + pointC.x*pointA.y - pointA.x*pointC.y - pointB.x*pointA.y - pointC.x*pointB.y;

      double x = 0.5 * u/d;
      double y = 0.5 * v/d;

      return new Point2D(x, y);
    }

    public Point2D findIntersection(Vector v1, Point2D p1, Vector v2, Point2D p2){
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
                Console.WriteLine("Solution: s=" + xVector[0] + ", t=" + xVector[1] + r);

                
                return r;
    
    }

  }
}