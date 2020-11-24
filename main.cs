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
    public static double sweepingLength = 0.05;
    public static void Main (string[] args) {
      clearExportFolder();
      Tree tree = new Tree();
      if (!parseArgs(args))
        return;                                                   // wrong arguments, close program
      switch (demoModel){
        case 0:{  tree = Tree.exampleBeetleTree();break;}         // beetle
        case 1:{  tree = Tree.exampleLongAntennaTree();break;}    // antenna beetle
        case 2:{  tree = Tree.exampleLizardTree();break;}         // lizard
        default:{Console.WriteLine("undefined model"); return;}
      }
      LangsAlgorithm lang = new LangsAlgorithm(tree,DEBUG, VISUAL,zoomFactor, sweepingLength);
      List<Crease> creases = lang.sweepingProcess(tree);
      FileHandler f = new FileHandler(DEBUG, tree.getPaperSizeX(), zoomFactor);
      if (f.exportSVG("export.svg", tree, creases))
        Console.WriteLine("exported SVG");
    }
    public static void clearExportFolder(){
      System.IO.DirectoryInfo di = new DirectoryInfo("export");
      foreach (FileInfo file in di.GetFiles())
        file.Delete();
    }
    public static bool parseArgs(string[] args){
      int c=0;
      while (args.Length > c){
        if (String.Equals(args[c], "-h",StringComparison.OrdinalIgnoreCase)||
            String.Equals(args[c], "-help",StringComparison.OrdinalIgnoreCase)||
            String.Equals(args[c], "help",StringComparison.OrdinalIgnoreCase)){
          Console.WriteLine("usage: main.exe -z zoomFactor -debug -visual -m model -p precision");
          return false;
        }
        if (String.Equals(args[c], "-visual",StringComparison.OrdinalIgnoreCase))
          VISUAL = true;
        if (String.Equals(args[c], "-debug",StringComparison.OrdinalIgnoreCase))
          DEBUG = true;
          if (String.Equals(args[c], "-p",StringComparison.OrdinalIgnoreCase))
            if (++c < args.Length){
              sweepingLength = double.Parse(args[c]);
            }else{
              Console.WriteLine("provide a double as precision (default= 0.05)");
              return false;
            }
        if (String.Equals(args[c], "-z",StringComparison.OrdinalIgnoreCase))
          if (++c < args.Length){
            zoomFactor = int.Parse(args[c]);
          }else{
            Console.WriteLine("provide an integer as zoomfactor (default= 100)");
            return false;
          }
        if (String.Equals(args[c], "-m",StringComparison.OrdinalIgnoreCase))
          if (++c < args.Length){
            demoModel = int.Parse(args[c]);
          }else{
            Console.WriteLine("provide index of model, 0=beetle, 1=antenna beetle, 2=lizard");
            return false;
          }
        c++;
      }
      return true;
    }
  }
}
