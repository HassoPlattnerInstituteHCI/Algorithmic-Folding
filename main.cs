using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking
{
    class MainClass
    {
        public static double stripWidth = 1; 
        public const bool DEBUG = true;
        public const bool withHamiltonianRefilement = true;
        public static Debug debug = new Debug(stripWidth);

        public static void Main(string[] args)
        {
            Strip s = new Strip(stripWidth, DEBUG);
            FileHandler f = new FileHandler(stripWidth,DEBUG); 
            List<Triangle> stl = f.importSTL("input.stl");
            // List<Triangle> stl = f.importSTL("test.stl");
            

            if(withHamiltonianRefilement){
              HamiltonianRefinement hamilton = new HamiltonianRefinement();
              foreach(var triangle in stl){
                hamilton.addTriangle(triangle);
              }
              hamilton.createDualGraph();
              foreach(var it in hamilton.dualGraph) Console.WriteLine(it);

              hamilton.walkAround(s); //adds triangles to strip with hamiltonian refinement
              
            }else{
              foreach (var triangle in stl)
              {
                  s.addTriangle(triangle);
              }
            }
          f.exportSVG(s);
          if(DEBUG) debug.createDebuggingOutput();
            
        }
    }
}
