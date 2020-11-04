using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace inClassHacking
{
    class FileHandler{
      private double stripWidth;
      private bool DEBUG;
      public FileHandler(double w, bool DEBUG=false)
        {
            this.stripWidth = w;
            this.DEBUG = DEBUG;
        }
        public bool exportSVG(Strip strip)
        {
            List<string> svg = new List<string>();
            SVG_init(svg);
            SVG_addPaper(svg, strip);
            double x = 1;
            foreach (Segment s in strip.getSegments())
            {
                if (s.getType() == type.Plain)
                    x += s.length;
                if (s.getType() == type.Fold)
                {
                    SVG_addSegment(svg,s,x);
                    x += s.length;
                }
            }
            SVG_ending(svg);
            File.WriteAllLines("export.svg",svg.ToArray());
            if (DEBUG) Console.WriteLine("exported SVG");
            return true;
        }
        public void SVG_ending (List<string> svg)
        {
            svg.Add("</svg>");
        }
        public void SVG_addSegment(List<string> svg, Segment s, double x)
        {
            svg.Add("<line fill=\"none\" " +
                    "stroke=\"" + colToHex(s.draw().c) + "\" " +
                    "stroke-miterlimit=\"10\" " +
                    "x1=\"" + (int)(x + s.draw().x1) + "\" " +
                    "y1=\"" + (int)(s.draw().y1+1) + "\" " +
                    "x2=\"" + (int)(x + s.draw().x2) + "\" " +
                    "y2=\"" + (int)(s.draw().y2 +1) +
                    "\"/>"
                   );
        }
        public List<string> SVG_init(List<string> svg)
        {
            svg.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            svg.Add("<svg version = \"1.1\" " +
                    "id=\"Layer_1\" " +
                    "xmlns=\"http://www.w3.org/2000/svg\" " +
                    "xmlns:xlink=\"http://www.w3.org/1999/xlink\" " +
                    "x=\"0px\" y=\"0px\" " +
                    "width=\"3456px\" height=\"6912px\" " +
                    "viewBox=\"0 0 3456 6912\" " +
                    "enable-background=\"new 0 0 3456 6912\" " +
                    "xml:space = \"preserve\">"
                   );
            return svg;
        }
        public void SVG_addPaper(List<string> svg, Strip strip){
            svg.Add("<rect x=\"1\" y=\"1\" " +
                    "fill=\"none\" " +
                    "stroke=\"#000000\" " +
                    "stroke-miterlimit=\"10\" " +
                    "width=\""
                    + (int)(strip.getLength()+1) +
                    "\" height=\""
                    + (int)(stripWidth) +
                    "\"/>"
                   );
        }
         private string colToHex(Color c)
        {
            string hex = "";
            if (c == Color.Red)
                hex = "#FF0000";
            if (c == Color.Blue)
                hex = "#0000FF";
            if (c == Color.Green)
                hex = "#00FF00";
            if (c == Color.Yellow)
                hex = "FFFF00";
            return hex;
        }
        // public static string
        public List<Triangle> importSTL(string filename)
        {
            List<Triangle> triangles = new List<Triangle>();
            string[] stl = File.ReadAllLines(filename);
            int val =0;
            Point3D a=null, b =null, c = null;
            foreach (string line in stl)
            {
                if (line.Contains("vertex"))
                {
                    switch (val)
                    {
                        case 0:{a = pointFromString(line);break;
                            }
                        case 1:{b = pointFromString(line);break;
                            }
                        case 2:{c = pointFromString(line);break;
                            }
                    }
                    val++;
                }
                if (line.Contains("endloop"))
                {
                    val = 0;
                    triangles.Add(new Triangle(a,b,c));
                }
            }
            return triangles;
        }
        public Point3D pointFromString(string s){
            string sub= s.Trim();
            string[] coords = sub.Split(' ');
            return new Point3D(
                double.Parse(coords[1]),
                double.Parse(coords[2]),
                double.Parse(coords[3])
            );
        }
    }
}
