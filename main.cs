using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {

    public static double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4}; //{3, 4, 5, 6, 1, 2, 1, 1, 3, 1, 2, 2, 3};
    public const bool DEBUG = true;
    static int zoomFactor = 50;

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

      LeafNode head = new LeafNode(10, 1, inNode1, tree, true);
      LeafNode antenna = new LeafNode(11, 1, inNode1, tree);
      inNode1.addInteriorNode(inNode2, 1);

      LeafNode legs = new LeafNode(12, 1, inNode2, tree);
      inNode2.addInteriorNode(inNode3, 1);
      LeafNode legs2 = new LeafNode(13, 1, inNode3, tree);
      LeafNode tail = new LeafNode(14, 1, inNode3, tree, true);

      FileHandler treeFileHandler = new FileHandler(DEBUG, 20, 20, 200);
      treeFileHandler.exportSVG("tree.svg", tree);

      List<Circle> circles = new List<Circle>();
      List<LeafNode> nodes = tree.calculateCirclePositioning();

      foreach(var node in nodes){
        circles.Add(node.circle);
        Console.WriteLine(node.circle.getCenter());
      }

      LangsAlgorithm lang = new LangsAlgorithm(nodes, circles);
      List<Crease> creases = lang.sweepingProcess();

      double paperSizeX = tree.getPaperSizeX();
      double paperSizeY = tree.getPaperSizeY();
      FileHandler f = new FileHandler(DEBUG, paperSizeX, paperSizeY, zoomFactor); 
      
      f.exportSVG("export.svg", circles, creases);

      // treeFunction();

    }


    // static double[] calculateDistances(double[] input){
    //   //TODO: calculate distan
    //   double[] ret = {9.63, 2.7, 14.56, 6.63, 29.26};//{10.16, 4.48, 13.78, 8.13, 32.26}; //{10.16,   4.21, 10.12,   8.16,  28.33};
    //   return ret;
    // }

    static void treeFunction(){
      // FileHandler treeFileHandler = new FileHandler(true, 20, 200);
      
      // Tree tree = new Tree();

      // InteriorNode inNode1 = new InteriorNode(0, tree);
      // InteriorNode inNode2 = new InteriorNode(1, tree);
      // InteriorNode inNode3 = new InteriorNode(2);
      // InteriorNode inNode4 = new InteriorNode(3);
      // InteriorNode inNode5 = new InteriorNode(4);
      // InteriorNode inNode6 = new InteriorNode(5);


      // LeafNode head = new LeafNode(10, 1, inNode1, tree, true);
      // LeafNode antenna = new LeafNode(11, 1, inNode1, tree);
      // inNode1.addInteriorNode(inNode2, 1);
      // LeafNode extra1 = new LeafNode(16, 1, inNode2, tree, true);
      // inNode2.addInteriorNode(inNode3, 1);
      // LeafNode legs1 = new LeafNode(12, 4, inNode3, tree);
      // inNode3.addInteriorNode(inNode4, 1, tree);
      // inNode4.addInteriorNode(inNode5, 1, tree);
      // LeafNode legs2 = new LeafNode(13, 6, inNode5, tree);
      // inNode5.addInteriorNode(inNode6, 2, tree);
      // LeafNode legs3 = new LeafNode(14, 8, inNode6, tree);
      // LeafNode tail = new LeafNode(15, 2, inNode6, tree, true);

      // LeafNode legs = new LeafNode(12, 1, inNode2, tree);
      // LeafNode tail = new LeafNode(13, 1, inNode2, tree, true);

      // treeFileHandler.exportSVG("tree.svg", tree);

      // // List<Circle> circles = tree.calculateCirclePositioning();

      // FileHandler circlesFileHandler = new FileHandler(DEBUG, tree.drawingOffsetX*2, 200); //tree.drawingOffset*2 

      // circlesFileHandler.exportSVG("circles.svg", circles);




    }
  }
}