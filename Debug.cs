using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking
{
  class Debug{
    List<string> svg = new List<string>();
    double lastX = 500, x, y = 5, stripWidth;

    FileHandler fh;

    public void drawRect(double width, Direction d){
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

    public Debug(double s){
      this.stripWidth = s;
      fh = new FileHandler(stripWidth, true);
      fh.SVG_init(svg);
    }

    public void createDebuggingOutput(){
      fh.SVG_ending(svg);
      Console.WriteLine(svg.Count);
      File.WriteAllLines("debug.svg",svg.ToArray());
    }
  }
}