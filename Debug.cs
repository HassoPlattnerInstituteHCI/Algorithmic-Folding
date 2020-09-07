using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking
{
  class Debug{
    List<string> svg = new List<string>();
    double lastX = 1500, x, y = 5, stripWidth; //lastX affects the x-position in debug.svg

    FileHandler fh;

    public void draw(double width, Direction d){
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

    public Debug(double stripWidth){
      this.stripWidth = stripWidth;
      fh = new FileHandler(stripWidth, true);
      fh.SVG_init(svg);
    }

    public void createDebuggingOutput(){
      fh.SVG_ending(svg);
      File.WriteAllLines("debug.svg", svg.ToArray());
    }
  }
}