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
      
      Vector up = new Vector(0, -1);
      Vector down = new Vector(0, 1);
      Vector left = new Vector(1, 0);
      Vector right = new Vector(-1, 0);

      Point2D lowerMiddle = new Point2D(s/2, s);

//important points related to circles
      Vector circle2To1 = new Vector(circles[2].getCenter(), circles[1].getCenter());
      Point2D edgeCircle1Left = circles[1].getCenter() + circle2To1.getReverse().normalized()*circles[1].getRadius();
      Point2D edgeCircle2left = new Point2D(0, circles[2].getCenter().y+circles[2].getRadius());
      Point2D edgeCircle2right = circles[2].getCenter() + circle2To1.normalized()*circles[2].getRadius();
      Point2D betweenCircle1_2 = edgeCircle2right + circle2To1.normalized()*input[7];
      Point2D betweenCircles2_3  = new Point2D(circles[2].getCenter().x, edgeCircle2left.y + input[8]);
      Point2D edgeCircle3Above = new Point2D(circles[3].getCenter().x, circles[3].getCenter().y-circles[3].getRadius());


      Vector circle4To3 = new Vector(circles[4].getCenter(), circles[3].getCenter());
      Vector circle4To7 = new Vector(circles[4].getCenter(), circles[7].getCenter());

      Point2D edgeCircle3low = circles[3].getCenter() + circle4To3.getReverse().normalized()*circles[3].getRadius();
      Point2D edgeCircle4right = circles[4].getCenter() + circle4To7.normalized() * circles[4].getRadius();
      Point2D edgeCircle4left = circles[4].getCenter() + circle4To3.normalized()*circles[4].getRadius();
      
      Point2D edgeCircle5above = new Point2D(s/2, circles[5].getCenter().y-circles[5].getRadius());
      Point2D edgeCircle5below = new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius());
      Point2D edgeCircle6left = new Point2D(circles[6].getCenter().x-circles[6].getRadius(), circles[6].getCenter().y);
      Point2D edegCircle6Below = circles[6].getCenter() + down*circles[6].getRadius();
      Point2D betweenCircle6_7 = new Point2D(circles[6].getCenter().x, edegCircle6Below.y+input[10]);
      Point2D circle7Above = new Point2D(circles[7].getCenter().x, circles[7].getCenter().y-circles[7].getRadius());


      //helpers & vectors
      Point2D helper0 = findIntersection(circle4To7.getNormalLeft(), edgeCircle4right, circle4To3.getNormalRight(), edgeCircle4left);
      Point2D helper1 = findIntersection(right,  edgeCircle2left, circle2To1.getNormalRight(), edgeCircle2right);
      Vector circle3ToHelper1 = new Vector(circles[3].getCenter(), helper0);
      Point2D helper2 = findIntersection(circle4To3.getNormalLeft(), edgeCircle3low, circle3ToHelper1, circles[3].getCenter());
      Point2D helper3 = findIntersection(circle4To7.getNormalRight(), edgeCircle4right, up, lowerMiddle); 
      Point2D helper4 = new Point2D(circles[0].getCenter().x-circles[0].getRadius(), circles[5].getCenter().y-circles[5].getRadius()-input[6]);

      Vector helper4ToCircle5 = new Vector(helper4, circles[5].getCenter());
      Vector circle1To5 = new Vector(circles[1].getCenter(), circles[5].getCenter());

      Point2D helper5 = findIntersection(left, edgeCircle5above, helper4ToCircle5, helper4);
      Point2D helper6 = findIntersection(circle1To5.getNormalRight(), helper5, right, edgeCircle5below);

      Vector edge6ToHelper6 = new Vector(edgeCircle6left, helper6);

      Point2D middleInput7 = new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+input[7]);
      Point2D helper7 = findIntersection(edge6ToHelper6, edgeCircle6left, left, middleInput7);
      Point2D middleInput8 = new Point2D(s/2, circles[5].getCenter().y+circles[5].getRadius()+input[7]+input[8]);
      Point2D helper8 = findIntersection(edge6ToHelper6, edgeCircle6left, left, middleInput8);

      Vector circle1ToHelper6 = new Vector(circles[1].getCenter(), helper6);
      Vector circle6LeftTo1 = new Vector(edgeCircle6left, circles[1].getCenter());

      Point2D helper9 = findIntersection(circle1To5.getNormalRight(), helper4, circle1ToHelper6, helper6);
      Point2D helper10 = findIntersection(left, edegCircle6Below, circle6LeftTo1.getNormalLeft(), helper8);

      Vector upperHelper10 = new Vector(helper8, helper10);
      
      Point2D helper20 = helper7+upperHelper10;
      Point2D helper21 = helper6+upperHelper10;

      Vector leftHelper1 = new Vector(edgeCircle2left, helper1);

      Point2D helper30 = betweenCircles2_3 + leftHelper1;
      Point2D helper31 = edgeCircle3Above + leftHelper1;

      Vector circle3To7 = new Vector(circles[3].getCenter(), circles[7].getCenter());

      Point2D helper40 = findIntersection(circle3To7.getNormalLeft(), helper0, left, circle7Above);
      Point2D helper41 = findIntersection(circle3To7.getNormalLeft(), helper2, left, betweenCircle6_7);
      // helper41 += new Vector(helper10, helper41).normalized();
      Vector helper31To41 = new Vector(helper31, helper41);

      Point2D helper42 = helper30 + helper31To41;
      Point2D helper43 = helper1 + helper31To41;

      Vector curveDistance = new Vector(helper42, helper43);

      Point2D helper44 = helper43 + curveDistance.normalized()*input[7];
      Point2D helper45 = helper44 + curveDistance.normalized()*input[5];

      Vector helper43To1 = new Vector(helper43, helper1);

      Point2D helper60 = findIntersection(helper43To1, helper45, circle2To1.getNormalRight(),  edgeCircle1Left);
      Point2D helper61 = findIntersection(helper43To1, helper44, circle2To1.getNormalRight(), betweenCircle1_2);

      Vector helper44To21 = new Vector(helper44, helper21);

      Point2D helper22 = findIntersection(helper44To21, helper45, upperHelper10, helper9);
      if(helper22.x<helper45.x) helper22.x = helper45.x;

      Vector helper1To61 = new Vector(helper1, helper61);      
      Vector helper41To45 = new Vector(helper41, helper45);
  
      Point2D vertexCircle1Rivers = findIntersection(helper1To61 ,helper1, helper41To45, helper41);

//start adding creases
      //lower part

       //middle
      creases.Add(new Crease(helper3, circles[4].getCenter(), Color.Red));
      creases.Add(new Crease(helper3, circles[7].getCenter(), Color.Red));
      creases.Add(new Crease(helper3, lowerMiddle, Color.Blue));

      creases.Add(new Crease(helper0, helper3, Color.Blue));

        //left
      creases.Add(new Crease(helper0, circles[3].getCenter(), Color.Red));
      creases.Add(new Crease(helper0, circles[4].getCenter(), Color.Red));
      creases.Add(new Crease(helper0, circles[7].getCenter(), Color.Red));
      creases.Add(new Crease(helper0, edgeCircle4left, Color.Blue));
      creases.Add(new Crease(helper2, edgeCircle3low, Color.Blue));
      creases.Add(new Crease(circles[3].getCenter(), circles[7].getCenter(), Color.Grey));


      //upper left corner
      creases.Add(new Crease(helper1, circles[2].getCenter(), Color.Red));
      creases.Add(new Crease(helper1, edgeCircle2left, Color.Blue));
      creases.Add(new Crease(helper1, edgeCircle2right, Color.Blue));


      //upper middle
      creases.Add(new Crease(helper4, new Point2D(s/2, helper4.y), Color.Blue));
      creases.Add(new Crease(helper4, circles[0].getCenter(), Color.Red));
      creases.Add(new Crease(helper4, circles[1].getCenter(), Color.Red));
      creases.Add(new Crease(helper4, circles[5].getCenter(), Color.Red));
      creases.Add(new Crease(helper4, new Point2D(s/2, helper4.y), Color.Blue));
      creases.Add(new Crease(edgeCircle5above, helper5, Color.Blue));

      creases.Add(new Crease(circles[0].getCenter(), circles[5].getCenter(), Color.Grey));

      creases.Add(new Crease(helper6, edgeCircle5below, Color.Blue));
      creases.Add(new Crease(helper6, helper5, Color.Blue));
      creases.Add(new Crease(helper6, circles[5].getCenter(), Color.Red));
      creases.Add(new Crease(helper6, edgeCircle6left, Color.Red));

      //middle - between circle 5 and 6
      creases.Add(new Crease(edgeCircle6left, circles[6].getCenter(), Color.Red));
      creases.Add(new Crease(middleInput7, helper7, Color.Blue));
      creases.Add(new Crease(middleInput8, helper8, Color.Blue));

      // shifted vertex near center of circle 1 to center, try if it wokrs
      // Point2D nearCenter1 = circles[1].getCenter() + down*1 + left*1;
      creases.Add(new Crease(helper9, helper4, Color.Blue));
      creases.Add(new Crease(helper6, circles[1].getCenter(), Color.Red));
      creases.Add(new Crease(edgeCircle6left, circles[1].getCenter(), Color.Grey));


      creases.Add(new Crease(helper10, edegCircle6Below, Color.Blue));
      creases.Add(new Crease(helper10, helper8, Color.Blue));
      creases.Add(new Crease(helper10, edgeCircle6left, Color.Red));

      creases.Add(new Crease(helper7, helper20, Color.Blue));
      creases.Add(new Crease(helper6, helper21, Color.Blue));

      //left rivers between circles 2 and 3
      creases.Add(new Crease(betweenCircles2_3, helper30, Color.Blue));
      creases.Add(new Crease(edgeCircle3Above, helper31, Color.Blue));

        //river from lower lowerPart
      creases.Add(new Crease(helper40, helper0, Color.Blue));
      creases.Add(new Crease(helper40, circle7Above, Color.Blue));
      creases.Add(new Crease(helper41, helper2, Color.Blue));
      creases.Add(new Crease(helper41, betweenCircle6_7, Color.Blue));



      creases.Add(new Crease(helper31, helper41, Color.Blue));

      creases.Add(new Crease(helper30, helper42, Color.Blue));
      creases.Add(new Crease(helper42, helper10, Color.Blue));

      creases.Add(new Crease(helper43, helper1, Color.Blue));
      creases.Add(new Crease(helper43, helper20, Color.Blue));

      creases.Add(new Crease(helper44, helper21, Color.Blue));
      creases.Add(new Crease(helper61 ,helper44, Color.Blue));
      creases.Add(new Crease(betweenCircle1_2, helper61, Color.Blue));

      creases.Add(new Crease(helper60, helper45, Color.Blue));
      creases.Add(new Crease(helper45, helper22, Color.Blue));
      creases.Add(new Crease(helper9, helper22, Color.Blue));
      creases.Add(new Crease(edgeCircle1Left, helper60, Color.Blue));
      
      
      creases.Add(new Crease(vertexCircle1Rivers, helper1, Color.Red));
      creases.Add(new Crease(vertexCircle1Rivers, helper41, Color.Red));
      creases.Add(new Crease(vertexCircle1Rivers, circles[1].getCenter(), Color.Red));

      Vector helper10To20 = new Vector(helper10, helper20);
      Vector vertex1ToCircle1 = new Vector(vertexCircle1Rivers, circles[1].getCenter());

      Point2D helper70 = findIntersection(helper10To20, helper10, vertex1ToCircle1, vertexCircle1Rivers);
      
      creases.Add(new Crease(helper70, helper10, Color.Red));

      Vector helper1To30 = new Vector(helper1, helper30);
      Vector helper1To43 = new Vector(helper1, helper43);
      Point2D vertexInCircle3 = findIntersection(helper1To30, helper1, helper1To43.getNormalRight(), vertexCircle1Rivers);

      creases.Add(new Crease(vertexInCircle3, helper1, Color.Red));
      creases.Add(new Crease(vertexCircle1Rivers, vertexInCircle3, Color.Grey));
      creases.Add(new Crease(vertexInCircle3, circles[3].getCenter(), Color.Red));
      creases.Add(new Crease(vertexInCircle3, helper41, Color.Red));


      Vector helper41To40 = new Vector(helper41, helper40);
      Point2D vertexInCircle7 = findIntersection(helper41To40, helper41, down, helper10);

      creases.Add(new Crease(vertexInCircle7, helper10, Color.Red));
      creases.Add(new Crease(vertexInCircle7, helper41, Color.Red));
      creases.Add(new Crease(vertexInCircle7, circles[7].getCenter(), Color.Red));

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