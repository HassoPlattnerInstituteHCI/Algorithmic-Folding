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
        public const bool withHamiltonianRefinement = true;
        public static Debug debug = new Debug(stripWidth);

        public static void Main(string[] args)
        {
            Strip s = new Strip(stripWidth, DEBUG);
            FileHandler f = new FileHandler(stripWidth,DEBUG); 

            // List<Triangle> stl = f.importSTL("input.stl"); //cube
            // List<Triangle> stl = f.importSTL("test.stl");  //one simple triangle
            List<Triangle> stl = f.importSTL("Part2.stl");    //some 3d printable part
            // List<Triangle> stl = f.importSTL("zip.stl");   
            // List<Triangle> stl = f.importSTL("turm.stl");  //castle's tower
            
            if(withHamiltonianRefinement){
              HamiltonianRefinement hamiltonian = new HamiltonianRefinement();
              foreach(var triangle in stl){
                hamiltonian.addTriangle(triangle);
              }
              hamiltonian.createDualGraph();
              
              hamiltonian.toStrip(s); //adds triangles to strip with hamiltonian refinement
              if(DEBUG) Console.WriteLine("number of triangles imported: " + stl.Count);
              if(DEBUG) Console.WriteLine("number of triangles exported: " + hamiltonian.getNumberOfImportedTriangles());
              
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
