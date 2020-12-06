using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace inClassHacking
{
    public enum Color{Red, Green, Blue, Yellow, Grey, Black, Burlywood, BlanchedAlmond};
    class FileHandler{
      double paperSize;
      private bool DEBUG;
      int zoomFactor;

      List<int> drawnNodes = new List<int>();
      public FileHandler(bool debug, List<Tree> trees, int zoomFactor)
      {
          DEBUG = debug;
          this.paperSize = trees.First().getPaperSize();
          this.zoomFactor = zoomFactor;
      }
        public FileHandler(bool debug, double paperSize, int zoomFactor)
        {
            DEBUG = debug;
            this.paperSize = paperSize;
            this.zoomFactor = zoomFactor;
        }
        public bool exportSVG(string filename, List<Tree> trees, List<List<Crease>> creases = null)
        {
            creases = creases ?? new List<List<Crease>>();
            List<string> svg = new List<string>();
            SVG_init(svg);
            int i =0;
            foreach (Tree tree in trees)
              foreach(var circle in tree.getCircles())
                drawCircle(svg, circle, tree.mirrored);
            foreach (Tree tree in trees)
              foreach(var crease in creases[i++])
                drawCrease(svg, crease, tree.mirrored);
            return writeFile(filename,svg);
        }
        public void SVG_ending (List<string> svg)
        {
            svg.Add("</svg>");
        }
        public bool writeFile(string file, List<string> svg){
          SVG_ending(svg);
          Directory.CreateDirectory("export");
          File.WriteAllLines("export/"+file, svg.ToArray());
          return true;
        }
        public List<string> SVG_init(List<string> svg)
        {
            svg.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            svg.Add("<svg version = \"1.1\" " +
                    "id=\"Layer_1\" " +
                    "xmlns=\"http://www.w3.org/2000/svg\" " +
                    "xmlns:xlink=\"http://www.w3.org/1999/xlink\" " +
                    "x=\"0px\" y=\"0px\" " +
                    "width=\""+paperSize*zoomFactor+ "px\" height=\"" + paperSize*zoomFactor + "px\" " +
                    "viewBox=\"0 0 "+zoomFactor*paperSize+" "+zoomFactor*paperSize+"\" " +
                    "enable-background=\"new 0 0 3456 6912\" " +
                    "style=\"background: "+colToHex(Defaults.backgroundColor) + "\" " +
                    "xml:space = \"preserve\">"
                   );
            return svg;
        }
        public void drawCircle(List<string> svg, Circle circle, bool mirrored=false){
          svg.Add("<circle cx=\"" + circle.getCenter().x*zoomFactor + "\" cy=\"" + circle.getCenter().y*zoomFactor + "\" r=\""+ zoomFactor*circle.node.size+ "\" stroke=\"black\" stroke-width=\"3\" fill=\""+colToHex(Defaults.circleColor) + "\" />");
          if(mirrored) svg.Add("<circle cx=\"" + ((paperSize-circle.getCenter().x)*zoomFactor) + "\" cy=\"" + circle.getCenter().y*zoomFactor + "\" r=\""+ zoomFactor*circle.getRadius()+ "\" stroke=\"black\" stroke-width=\"3\" fill=\""+colToHex(Defaults.circleColor) + "\" />");
          if(DEBUG) Console.WriteLine("draw circle at " + circle.getCenter().x + ", " + circle.getCenter().y + " with radius " + circle.node.size);
        }
        public void drawCrease(List<string> svg, Crease crease, bool mirrored=false){
          svg.Add("<line x1=\""+ crease.p1.x*zoomFactor + "\" y1=\"" + crease.p1.y*zoomFactor + "\" x2=\"" + crease.p2.x*zoomFactor + "\" y2=\"" + crease.p2.y*zoomFactor + "\" stroke=\"" + colToHex(crease.color) +  "\" style=\"stroke-width:4\" />");
          if(mirrored)svg.Add("<line x1=\""+ ((paperSize-crease.p1.x)*zoomFactor) + "\" y1=\"" + crease.p1.y*zoomFactor + "\" x2=\"" + ((paperSize-crease.p2.x)*zoomFactor) + "\" y2=\"" + crease.p2.y*zoomFactor + "\" stroke=\"" + colToHex(crease.color) +  "\" style=\"stroke-width:4\" />");
        }
        private string colToHex(Color c)
        {   string hex = "";
            if (c == Color.Red)           hex = "#FF0000";
            if (c == Color.Blue)          hex = "#0000FF";
            if (c == Color.Green)         hex = "#00FF00";
            if (c == Color.Yellow)        hex = "#FFFF00";
            if (c== Color.Grey)           hex = "#808080";
            if (c== Color.Black)          hex = "#000000";
            if (c== Color.Burlywood)      hex = "#deb887";
            if (c== Color.BlanchedAlmond) hex = "#ffebcd";
            return hex;
        }
      }
}
