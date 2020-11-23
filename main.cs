using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {
    public const bool DEBUG = true;
    public const bool VISUAL = true;
    static int zoomFactor = 100;
    static int demoModel=1;
    public static void Main (string[] args) {
      Tree tree = new Tree();
      List<Circle> circles = new List<Circle>();
      switch (demoModel){
        case 0:{  tree = Tree.exampleBeetleTree();circles = tree.exampleBeetleCircles();break;}           // beetle
        case 1:{  tree = Tree.exampleLongAntennaTree();circles = tree.exampleLongAntennaCircles();break;}    // antenna beetle
        case 2:{  tree = Tree.exampleLizardTree();circles = tree.exampleLizardCircles();break;}         // lizard
      }
      //Console.WriteLine("offset= " + tree.drawingOffsetX);
      LangsAlgorithm lang = new LangsAlgorithm(circles,DEBUG, VISUAL,zoomFactor,2*tree.drawingOffsetX);
      List<Crease> creases = lang.sweepingProcess();
      FileHandler f = new FileHandler(DEBUG, tree.getPaperSizeX(), zoomFactor);
      if (f.exportSVG("export.svg", circles, creases))
        Console.WriteLine("exported SVG");
    }
  }
}
