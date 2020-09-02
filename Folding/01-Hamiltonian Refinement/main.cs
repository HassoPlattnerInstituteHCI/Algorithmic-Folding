using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking
{
    class MainClass
    {
        public static double stripWidth = 5; 
        public const bool DEBUG = true;
        public static void Main(string[] args)
        {
            Strip s = new Strip(stripWidth, DEBUG);
            FileHandler f = new FileHandler(stripWidth,DEBUG); 
            List<Triangle> stl = f.importSTL();
            
            HamiltonianRefinement hamilton = new HamiltonianRefinement();
            foreach(var triangle in stl){
              hamilton.addTriangle(triangle);
            }
            hamilton.createDualGraph();
            foreach(var it in hamilton.dualGraph) Console.WriteLine(it);

            foreach(var it in hamilton.input){
              Console.WriteLine(it.Item1.index + " - " + it.Item1.neighbor.aSide + it.Item1.neighbor.bSide + it.Item1.neighbor.cSide);
            }
            // foreach (var triangle in stl)
            // {
            //     s.addTriangle(triangle);
            //  }
            f.exportSVG(s);
        }
    }
}
