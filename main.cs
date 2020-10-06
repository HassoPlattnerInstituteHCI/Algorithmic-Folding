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
      
      Tree tree = new Tree();

      InteriorNode inNode1 = new InteriorNode(0, tree);
      InteriorNode inNode2 = new InteriorNode(1, tree);
      InteriorNode inNode3 = new InteriorNode(2, tree);
      InteriorNode inNode4 = new InteriorNode(3, tree);
      // InteriorNode inNode5 = new InteriorNode(4, tree);
      // InteriorNode inNode6 = new InteriorNode(5, tree);
      

  //symmetric test tree
      LeafNode n1 = new LeafNode(10, 1, inNode1, tree);
      LeafNode m1 = new LeafNode(13, 1, inNode1, tree, true);
      LeafNode n2 = new LeafNode(11, 1, inNode2, tree);
      LeafNode m2 = new LeafNode(14, 1, inNode2, tree, true);
      inNode1.addInteriorNode(inNode2, 3);
      inNode2.addInteriorNode(inNode3, 1);
      LeafNode m3 = new LeafNode(15, 1, inNode3, tree, true);
      LeafNode n3 = new LeafNode(16, 1, inNode3, tree);


      
//visualize the tree
      FileHandler treeFileHandler = new FileHandler(DEBUG, 20, 20, 200);
      
      tree = Tree.exampleBeetleTree(); //overwrites tree with beetle

      treeFileHandler.exportSVG("tree.svg", tree); 
      
      // f.exportSVG("export.svg", nodes);


      // List<Circle> circles = Tree.exampleDeerCircles(); //circles for the deer's pattern - IMPORTANT: Change sweepingLength in LangsAlgorithm to 0.0005
      List<Circle> circles = tree.exampleBeetleCircles();  //circles for the beetle's pattern
      // List<Circle> circles = tree.calculateCirclePositioning();  //where circles should be calculated


      LangsAlgorithm lang = new LangsAlgorithm(circles);
      List<Crease> creases = lang.sweepingProcess();


      // double paperSizeX = 1;
      // double paperSizeY = 1;

      double paperSizeX = tree.getPaperSizeX();
      double paperSizeY = tree.getPaperSizeY();

      FileHandler f = new FileHandler(DEBUG, paperSizeX, paperSizeY, zoomFactor); 

      f.exportSVG("export.svg", circles, creases);
    }
  }
}