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
      double treeDrawingOffset =0;
      Color circleColor = Color.Burlywood;
      Color backgroundColor = Color.BlanchedAlmond;
      List<int> drawnNodes = new List<int>();

        public FileHandler(bool debug, double paperSize, int zoomFactor)
        {
            DEBUG = debug;
            this.paperSize = paperSize;
            this.zoomFactor = zoomFactor;
        }

        public bool exportSVG(string filename, Tree tree, List<Crease> creases = null)
        {
            creases = creases ?? new List<Crease>();

            List<string> svg = new List<string>();
            SVG_init(svg);

            foreach(var circle in tree.getCircles()){
              drawCircle(svg, circle);
            }
            foreach(var crease in creases){
              drawCrease(svg, crease);
            }
            return writeFile(filename,svg);
        }

        public bool exportSVG(string filename, List<LeafNode> nodes, List<Crease> creases = null)
        {
            List<Circle> circles = new List<Circle>();

            foreach(var node in nodes){
              circles.Add(node.circle);
            }
            creases = creases ?? new List<Crease>();

            List<string> svg = new List<string>();
            SVG_init(svg);

            foreach(var circle in circles){
              drawCircle(svg, circle);
            }
            foreach(var crease in creases){
              drawCrease(svg, crease);
            }
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
                    "style=\"background: "+colToHex(backgroundColor) + "\" " +
                    "xml:space = \"preserve\">"
                   );
            return svg;
        }
        public void drawCircle(List<string> svg, Circle circle){
          svg.Add("<circle cx=\"" + circle.getCenter().x*zoomFactor + "\" cy=\"" + circle.getCenter().y*zoomFactor + "\" r=\""+ zoomFactor*circle.getRadius()+ "\" stroke=\"black\" stroke-width=\"3\" fill=\""+colToHex(circleColor) + "\" />");
          svg.Add("<circle cx=\"" + ((paperSize-circle.getCenter().x)*zoomFactor) + "\" cy=\"" + circle.getCenter().y*zoomFactor + "\" r=\""+ zoomFactor*circle.getRadius()+ "\" stroke=\"black\" stroke-width=\"3\" fill=\""+colToHex(circleColor) + "\" />");

          if(DEBUG) Console.WriteLine("draw circle at " + circle.getCenter().x + ", " + circle.getCenter().y + " with radius " + circle.getRadius());
        }

        public void drawCrease(List<string> svg, Crease crease){
          svg.Add("<line x1=\""+ crease.p1.x*zoomFactor + "\" y1=\"" + crease.p1.y*zoomFactor + "\" x2=\"" + crease.p2.x*zoomFactor + "\" y2=\"" + crease.p2.y*zoomFactor + "\" stroke=\"" + colToHex(crease.color) +  "\" style=\"stroke-width:4\" />");

          svg.Add("<line x1=\""+ ((paperSize-crease.p1.x)*zoomFactor) + "\" y1=\"" + crease.p1.y*zoomFactor + "\" x2=\"" + ((paperSize-crease.p2.x)*zoomFactor) + "\" y2=\"" + crease.p2.y*zoomFactor + "\" stroke=\"" + colToHex(crease.color) +  "\" style=\"stroke-width:4\" />");
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
                hex = "#FFFF00";
            if (c== Color.Grey)
                hex = "#808080";
            if (c== Color.Black)
                hex = "#000000";
            if (c== Color.Burlywood)
                hex = "#deb887";
            if (c== Color.BlanchedAlmond)
                hex = "#ffebcd";
            return hex;
        }


//Tree methods
    public bool exportSVG(string file, Tree tree){
          List<Circle> drawing = new List<Circle>();
          List<Crease> lines = new List<Crease>();
          Point2D firstPosition = new Point2D(0, 2);

          List<string> svg = new List<string>();
          SVG_init(svg);

          Node startingNode = tree.treeNodes.First();

          foreach(var node in tree.treeNodes){
            if(node.middle) {
              startingNode = node;
              break;
            }
          }

          addNodesToDrawing(drawing, lines, startingNode, firstPosition);

          treeDrawingOffset = -treeDrawingOffset+2; //offset is negative
          paperSize = (treeDrawingOffset)*2;
          for(int i=0; i<drawing.Count; i++){
            drawing[i].setCenter(new Point2D(drawing[i].getCenter().x+treeDrawingOffset, drawing[i].getCenter().y+treeDrawingOffset));
            drawCircle(svg, drawing[i]);
          }

          for(int i=0; i<lines.Count; i++){
            Crease line = lines[i];
            line.p1.x += treeDrawingOffset;
            line.p1.y += treeDrawingOffset;
            line.p2.x += treeDrawingOffset;
            line.p2.y += treeDrawingOffset;
            drawCrease(svg, line);
          }
          return writeFile(file,svg);
        }

        void addNodesToDrawing(List<Circle> drawing, List<Crease> lines, Node startingNode, Point2D position){
          Circle nodeDrawing = new Circle(position, 0.3);
          drawing.Add(nodeDrawing);
          drawnNodes.Add(startingNode.index);

          if(startingNode.GetType() == typeof(LeafNode)){

            LeafNode node = (LeafNode) startingNode;

            Point2D nextPosition = new Point2D(position);
            nextPosition.x += node.size;
            addNodesToDrawing(drawing, lines, node.relatedNode, nextPosition);

          }else{

            InteriorNode node = (InteriorNode) startingNode;

            // foreach(var leafNode in node.relatedLeafNodes.Values){
            for(int i=0; i<node.relatedLeafNodes.Count; i++){
              var item = node.relatedLeafNodes.ElementAt(i);
              LeafNode leafNode = item.Value;
              Circle leafNodeDrawing = new Circle(nodeDrawing);
              Point2D drawingCenter = new Point2D(nodeDrawing.getCenter());

              if(leafNode.middle){
                if(i==0){
                  drawingCenter.y -= leafNode.size;
                }else{
                  drawingCenter.y += leafNode.size;
                }
                leafNodeDrawing.setCenter(drawingCenter);
                drawing.Add(leafNodeDrawing);
                drawnNodes.Add(leafNode.index);
                lines.Add(new Crease(drawingCenter, new Point2D(position), Color.Green));
                continue;
              }

              switch(i){
                case 0:
                        drawingCenter.x -= leafNode.size;
                        break;

                case 1:
                        drawingCenter -= new Vector(1, 1).normalized()*leafNode.size;
                        break;

                case 2:
                        drawingCenter -= new Vector(1, -1).normalized()*leafNode.size;
                        break;

              }

              leafNodeDrawing.setCenter(drawingCenter);
              if(drawingCenter.x < treeDrawingOffset) treeDrawingOffset = drawingCenter.x;
              if(drawingCenter.y < treeDrawingOffset) treeDrawingOffset = drawingCenter.y;

              drawing.Add(leafNodeDrawing);
              drawnNodes.Add(leafNode.index);
              lines.Add(new Crease(leafNodeDrawing.getCenter(), new Point2D(position), Color.Grey));

            }
            foreach(var interiorNode in node.relatedInteriorNodes.Keys){
              Point2D nextPosition = new Point2D(position);
              nextPosition.y += node.relatedInteriorNodes[interiorNode];

              if(drawnNodes.Contains(interiorNode.index)) continue;
              lines.Add(new Crease(new Point2D(nextPosition.x, nextPosition.y), position, Color.Grey));
              addNodesToDrawing(drawing, lines, interiorNode, nextPosition);
            }
          }
        }
    }
}
