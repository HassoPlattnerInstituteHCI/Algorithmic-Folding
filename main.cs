using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking
{
    class MainClass
    {
        public static double stripWidth = 1; //5 is best for origami simulation, 0.2 - 1 for debugging output
        public const bool DEBUG = true;
        public const bool withHamiltonianRefinement = true;
        public static Debug debug = new Debug(stripWidth);

        public static void Main(string[] args)
        {
            Strip s = new Strip(stripWidth, DEBUG);
            FileHandler f = new FileHandler(stripWidth,DEBUG); 

            // string importFile = "test.stl";  //simple triangle
            // string importFile = "input.stl"; //cube
            // string importFile = "zip.stl";   
            // string importFile = "turm.stl";  //castle's tower
            string importFile = "Part2.stl";    

            List<Triangle> stl = f.importSTL(importFile);     
            
            if(withHamiltonianRefinement){
              HamiltonianRefinement hamiltonian = new HamiltonianRefinement();
              foreach(var triangle in stl){
                hamiltonian.addTriangle(triangle);
              }
              hamiltonian.createDualGraph();
              
              hamiltonian.toStrip(s); //adds triangles to strip with hamiltonian refinement
              if(DEBUG) Console.WriteLine("number of triangles imported: " + stl.Count);
              if(DEBUG) Console.WriteLine("number of triangles exported: " + s.getNumberOfTriangles());
              
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
