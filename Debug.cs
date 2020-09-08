using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking
{
  class Debug{
    List<string> svg = new List<string>();
    double x, stripWidth, lastX = 1500, y = 5; //lastX affects the x-position in debug.svg

    FileHandler fh;

    public Debug(double stripWidth){
      this.stripWidth = stripWidth;
      fh = new FileHandler(stripWidth, true);
      fh.SVG_init(svg);
    }

    public void drawStrip(double width, Direction d){
      if(d == Direction.Left){
        x = lastX-width;
        svg.Add("<rect x=\"" + x + "\" y=\"" + y + 
        "\" width=\"" + width + "\" height=\"" + stripWidth + "\"/>");
        lastX = x;
      }
      else{
        x = lastX;
        svg.Add("<rec x=\"" + x + "\" y=\"" + y + 
        "\" width=\"" + width + "\" height=\"" + stripWidth + "\"/>");
        lastX += width;
      }
      y += stripWidth;
    }

    public void createDebuggingOutput(){
      fh.SVG_ending(svg);
      File.WriteAllLines("debug.svg", svg.ToArray());
    }
  }
}