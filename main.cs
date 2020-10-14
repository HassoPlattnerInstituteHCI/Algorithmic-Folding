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
        public static Debug debug = new Debug(stripWidth);

        public static void Main(string[] args)
        {
            string input;
            bool withHamiltonianRefinement = true;
            Strip s = new Strip(stripWidth, DEBUG);
            FileHandler f = new FileHandler(stripWidth,DEBUG);
            if (args.Length >0){
              if (String.Equals(args[0], "h",StringComparison.OrdinalIgnoreCase)){
                Console.WriteLine("usage: main.exe path_to_input withHamiltonianRefinement");
                return;
              }
              if (args.Length > 1){
                if (String.Equals(args[0], "false",StringComparison.OrdinalIgnoreCase)){
                  withHamiltonianRefinement=false;
                }
              }
              input = args[0];  //first param = input string
            }else{
            //  input = "test.stl";  //simple triangle
            //  input = "input.stl"; //cube
            //  input = "zip.stl";
              input = "turm.stl";  //castle's tower
            // input = "Part2.stl";
            }
            List<Triangle> stl = f.importSTL(input);

            if(withHamiltonianRefinement){
              HamiltonianRefinement hamiltonian = new HamiltonianRefinement();
              foreach(var triangle in stl){
                hamiltonian.addTriangle(triangle);
              }
              hamiltonian.createDualGraph();
              hamiltonian.triangulate();
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
