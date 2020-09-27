using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {

    public static double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4}; //{3, 4, 5, 6, 1, 2, 1, 1, 3, 1, 2, 2, 3};
    public const bool DEBUG = true;
    static int zoomFactor = 200;

    public static void Main (string[] args) {
      double[] distances = calculateDistances(input);
      FileHandler f = new FileHandler(DEBUG, distances[4], zoomFactor); 
      Positioning positioning = new Positioning(distances, input);

      List<Circle> circles = positioning.calculateCirclePositioning();

      // Folding folding = new Folding(circles, input, distances);
      // List<Crease> creases = folding.calculateCreases();
      
      LangsAlgorithm lang = new LangsAlgorithm(circles, input);
      List<Crease> creases = lang.sweepingProcess();
      

      f.exportSVG(circles, creases);

      treeFunction();

    }


    static double[] calculateDistances(double[] input){
      //TODO: calculate distan
      double[] ret = {9.63, 2.7, 14.56, 6.63, 29.26};//{10.16, 4.48, 13.78, 8.13, 32.26}; //{10.16,   4.21, 10.12,   8.16,  28.33};
      return ret;
    }

    static void treeFunction(){
      FileHandler fh = new FileHandler(true, 20, 200);

      InteriorNode inNode1 = new InteriorNode(0);
      InteriorNode inNode2 = new InteriorNode(1);
      InteriorNode inNode3 = new InteriorNode(2);
      InteriorNode inNode4 = new InteriorNode(3);
      InteriorNode inNode5 = new InteriorNode(4);
      InteriorNode inNode6 = new InteriorNode(5);


      LeafNode head = new LeafNode(10, 1, inNode1, true);
      LeafNode antenna = new LeafNode(11, 4, inNode1);
      inNode1.addInteriorNode(inNode2, 1);
      LeafNode extra1 = new LeafNode(16, 1, inNode2, true);
      inNode2.addInteriorNode(inNode3, 1);
      LeafNode legs1 = new LeafNode(12, 4, inNode3);
      inNode3.addInteriorNode(inNode4, 1);
      inNode4.addInteriorNode(inNode5, 1);
      LeafNode legs2 = new LeafNode(13, 6, inNode5);
      inNode5.addInteriorNode(inNode6, 2);
      LeafNode legs3 = new LeafNode(14, 8, inNode6);
      LeafNode tail = new LeafNode(15, 2, inNode6, true);


      Tree tree = new Tree();
      tree.addNode(inNode1);
      tree.addNode(inNode2);
      tree.addNode(inNode3);
      tree.addNode(inNode4);
      tree.addNode(inNode5);
      tree.addNode(inNode6);

      tree.addNode(head);
      tree.addNode(antenna);
      tree.addNode(legs1);
      tree.addNode(legs2);
      tree.addNode(legs3);
      tree.addNode(tail);

      List<Circle> circles = tree.calculateCirclePositioning();

      fh.exportSVG(tree, "tree.svg");
    }
  }
}