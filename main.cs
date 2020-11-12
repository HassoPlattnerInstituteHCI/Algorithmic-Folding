using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {
    public const bool DEBUG = false;
    static int zoomFactor = 50;
    public static void Main (string[] args) {
      Tree tree = new Tree();
      // tree = Tree.exampleBeetleTree(); //overwrites tree with beetle
      // List<Circle> circles = tree.exampleBeetleCircles();  //circles for the beetle's pattern

      tree = Tree.exampleLongAntennaTree(); //overwrites tree with long antenna beetle
      List<Circle> circles = tree.exampleLongAntennaCircles(); //circles for long antenna beetle's pattern

      LangsAlgorithm lang = new LangsAlgorithm(circles);
      List<Crease> creases = lang.sweepingProcess();
      FileHandler f = new FileHandler(DEBUG, tree.getPaperSizeX(), tree.getPaperSizeY(), zoomFactor);
      if (f.exportSVG("export.svg", circles, creases))
        Console.WriteLine("exported SVG");
    }
  }
}
