using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
namespace inClassHacking{

  class MainClass {
    public static bool DEBUG = false;
    public static bool VISUAL = false;
    public static int demoModel = 0;
    public static int zoomFactor = 100;

    public static void Main (string[] args) {
      Tree tree = new Tree();
      List<Circle> circles = new List<Circle>();
      if (parseArgs(args) == false) {return;}
      switch (demoModel){
        case 0:{  tree = Tree.exampleBeetleTree();break;}         // beetle
        case 1:{  tree = Tree.exampleLongAntennaTree();break;}    // antenna beetle
        case 2:{  tree = Tree.exampleLizardTree();break;}         // lizard
      }
      LangsAlgorithm lang = new LangsAlgorithm(tree,DEBUG, VISUAL,zoomFactor,tree.getPaperSizeX());
      List<Crease> creases = lang.sweepingProcess();
      FileHandler f = new FileHandler(DEBUG, tree.getPaperSizeX(), zoomFactor);
      if (f.exportSVG("export.svg", tree, creases))
        Console.WriteLine("exported SVG");
    }
    public static bool parseArgs(string[] args){
      int c=0;
      while (args.Length > c){
        if (String.Equals(args[c], "h",StringComparison.OrdinalIgnoreCase)){
          Console.WriteLine("usage: main.exe -z zoomFactor -debug -visual -m model");
          return false;
        }
        if (String.Equals(args[c], "-visual",StringComparison.OrdinalIgnoreCase)){
          VISUAL = true;
        }
        if (String.Equals(args[c], "-debug",StringComparison.OrdinalIgnoreCase)){
          DEBUG = true;
        }
        if (String.Equals(args[c], "-z",StringComparison.OrdinalIgnoreCase)){
          if (++c < args.Length){
            zoomFactor = int.Parse(args[c]);
          }else{
            Console.WriteLine("provide an integer as zoomfactor");
            return false;
          }
        }
        if (String.Equals(args[c], "-m",StringComparison.OrdinalIgnoreCase)){
          if (++c < args.Length){
            demoModel = int.Parse(args[c]);
          }else{
            Console.WriteLine("provide index of model, 0=beetle, 1=antenna beetle, 2=lizard");
            return false;
          }
        }
        c++;
      }
      return true;
    }
  }
}
