using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace inClassHacking
{
    public enum Color{Red,Green,Blue, Yellow};

    class FileHandler{
      double s, w, x, y, z;
      private bool DEBUG;
      int zoomFactor;

      public FileHandler(bool debug, double[] args)
        {
            DEBUG = debug;
            w=args[0];
            x=args[1];
            y=args[2];
            z=args[3];
            s=args[4];
        }
        public FileHandler(bool debug, double s, int zoomFactor)
        {
            DEBUG = debug;
            this.s = s;
            this.zoomFactor = zoomFactor;
        }
        public void exportSVG(List<Circle> circles, List<River> rivers)
        {
            List<string> svg = new List<string>();
            SVG_init(svg, s);

            foreach(var circle in circles){
              drawCircle(svg, circle);
            }
            foreach(var river in rivers){
              drawRiver(svg, river);
            }

            SVG_ending(svg);
            File.WriteAllLines("export.svg",svg.ToArray());
            if (DEBUG) Console.WriteLine("exported SVG");
        }
        public void SVG_ending (List<string> svg)
        {
            svg.Add("</svg>"); 
        }
        // public void SVG_addSegment(List<string> svg, Segment s, double x)
        // {
            // svg.Add("<line fill=\"none\" " +
            //         "stroke=\"" + colToHex(s.draw().c) + "\" " +
            //         "stroke-miterlimit=\"10\" " +
            //         "x1=\"" + (int)(x + s.draw().x1) + "\" " +
            //         "y1=\"" + (int)(s.draw().y1+1) + "\" " +
            //         "x2=\"" + (int)(x + s.draw().x2) + "\" " +
            //         "y2=\"" + (int)(s.draw().y2 +1) + 
            //         "\"/>"
            //        );
        // }

        public List<string> SVG_init(List<string> svg, double s)
        {
            svg.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            svg.Add("<svg version = \"1.1\" " +
                    "id=\"Layer_1\" " +
                    "xmlns=\"http://www.w3.org/2000/svg\" " +
                    "xmlns:xlink=\"http://www.w3.org/1999/xlink\" " +
                    "x=\"0px\" y=\"0px\" " +
                    "width=\""+s*zoomFactor+ "px\" height=\"" + s*zoomFactor + "px\" " +
                    "viewBox=\"0 0 "+zoomFactor*s+" "+zoomFactor*s+"\" " +
                    "enable-background=\"new 0 0 3456 6912\" " +
                    "xml:space = \"preserve\">"
                   );
            return svg;
        }
        public void drawRiver(List<string> svg, River river){}
        public void drawCircle(List<string> svg, Circle circle){
          svg.Add("<circle cx=\"" + circle.getCenter().x*zoomFactor + "\" cy=\"" + circle.getCenter().y*zoomFactor + "\" r=\""+ zoomFactor*circle.getRadius()+ "\" stroke=\"black\" stroke-width=\"3\" fill=\"red\" />");
          // svg.Add("<circle cx=\"300\" cy=\"100\" r=\"\75\" />");

          // if(DEBUG) Console.WriteLine("draw circle at " + circle.getCenter().x*zoomFactor + ", " + circle.getCenter().y*zoomFactor + " with radius " + circle.getRadius()*zoomFactor);
          if(DEBUG) Console.WriteLine("draw circle at " + circle.getCenter().x + ", " + circle.getCenter().y + " with radius " + circle.getRadius());
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
    }
}