using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {

    public static double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4}; //{3, 4, 5, 6, 1, 2, 1, 1, 3, 1, 2, 2, 3};
    public const bool DEBUG = true;
    static int zoomFactor = 500;

    public static void Main (string[] args) {
      // double[] distances = calculateDistances(input);
       
      // Positioning positioning = new Positioning(distances, input);

      // List<Circle> circles = positioning.calculateCirclePositioning();

      // Folding folding = new Folding(circles, input, distances);
      // List<Crease> creases = folding.calculateCreases();

     
      
      Tree tree = new Tree();

      InteriorNode inNode1 = new InteriorNode(0, tree);
      InteriorNode inNode2 = new InteriorNode(1, tree);
      InteriorNode inNode3 = new InteriorNode(2, tree);
      InteriorNode inNode4 = new InteriorNode(3, tree);
      InteriorNode inNode5 = new InteriorNode(4, tree);
      InteriorNode inNode6 = new InteriorNode(5, tree);
      
      // LeafNode head = new LeafNode(10, 1, inNode1, tree, true);
      // LeafNode antenna = new LeafNode(11, 4, inNode1, tree);
      // inNode1.addInteriorNode(inNode2, 1);
      // LeafNode middle1 = new LeafNode(12, 1, inNode2, tree, true);
      // inNode2.addInteriorNode(inNode3, 1);
      // LeafNode legs1 = new LeafNode(13, 4, inNode3, tree);
      // inNode3.addInteriorNode(inNode4, 1);
      // LeafNode middle2 = new LeafNode(14, 1, inNode4, tree, true);
      // inNode4.addInteriorNode(inNode5, 1);
      // LeafNode legs2 = new LeafNode(15, 6, inNode5, tree);
      // inNode5.addInteriorNode(inNode6, 2);
      // LeafNode legs3 = new LeafNode(16, 8, inNode6, tree);
      // LeafNode tail = new LeafNode(17, 4, inNode6, tree, true);







      // LeafNode n1 = new LeafNode(10, 1, inNode1, tree);
      // LeafNode m1 = new LeafNode(13, 1, inNode1, tree, true);
      // LeafNode n2 = new LeafNode(11, 1, inNode2, tree);
      // LeafNode m2 = new LeafNode(14, 1, inNode2, tree, true);
      // inNode1.addInteriorNode(inNode2, 3);
      // inNode2.addInteriorNode(inNode3, 1);
      // LeafNode m3 = new LeafNode(15, 1, inNode3, tree, true);
      // LeafNode n3 = new LeafNode(16, 1, inNode3, tree);


      

      FileHandler treeFileHandler = new FileHandler(DEBUG, 20, 20, 200);
      
      // tree = Tree.exampleBeetleTree();
      treeFileHandler.exportSVG("tree.svg", tree);

      // List<LeafNode> nodes = tree.exampleBeetleNodes();

      // List<LeafNode> nodes = tree.calculateCirclePositioning();
      double paperSizeX = 1;//tree.getPaperSizeX();
      double paperSizeY = 1;//tree.getPaperSizeY();
      FileHandler f = new FileHandler(DEBUG, paperSizeX, paperSizeY, zoomFactor); 
      
      
      // f.exportSVG("export.svg", nodes);

      // LangsAlgorithm lang = new LangsAlgorithm(nodes);
      // List<Crease> creases = lang.sweepingProcess();

      List<Circle> deerCircles = new List<Circle>();
      deerCircles.Add(new Circle(new Point2D(0.5, 0.1635), 0.3));
      deerCircles.Add(new Circle(new Point2D(0.4396, 0), 0.1057));
      deerCircles.Add(new Circle(new Point2D(0.2775, 0.1635), 0.1057));
      deerCircles.Add(new Circle(new Point2D(0.2281, 0), 0.1057));
      deerCircles.Add(new Circle(new Point2D(0.0698, 0.0698), 0.1057));
      deerCircles.Add(new Circle(new Point2D(0, 0.2594), 0.1057));
      deerCircles.Add(new Circle(new Point2D(0.2775, 0.5), 0.22));



      LangsAlgorithm langDeer = new LangsAlgorithm(deerCircles);
      List<Crease> creases = langDeer.sweepingProcess();
      

      
      

      // List<Edge> e = new List<Edge>();
      // List<Edge> e2 = new List<Edge>();
      // e.Add(new Edge(circles[0].getCenter(),0, circles[2].getCenter(),1));
      // e.Add(new Edge(circles[2].getCenter(), 1, circles[2].getCenter().mirrored(paperSizeX), 2));
      // e.Add(new Edge(circles[2].getCenter().mirrored(paperSizeX), 2, circles[0].getCenter(), 0));
      // e2.Add(new Edge(circles[0].getCenter(),0, circles[2].getCenter(),1));
      // e2.Add(new Edge(circles[2].getCenter(), 1, circles[2].getCenter().mirrored(paperSizeX), 2));
      // e2.Add(new Edge(circles[2].getCenter().mirrored(paperSizeX), 2, circles[0].getCenter(), 0));

      // foreach(var edge in e2){
      //   creases.Add(new Crease(edge.p1, edge.p1, Color.Grey));
      // }


      // lang.sweep(creases, e, e2);


      // f.exportSVG("export.svg", nodes, creases);
      f.exportSVG("export.svg", deerCircles, creases);
    }
  }
}