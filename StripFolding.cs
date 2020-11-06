using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace inClassHacking
{
    public enum Color{Red,Green,Blue, Yellow}
    public enum type {Fold, Plain}
    public enum MountainValley {Mountain, Valley}
    public enum Direction { Left, Right }

    class Strip
    {
        private double stripWidth;
        private bool DEBUG;
        private Direction d = Direction.Right;

        int counter = 0;

        private List<Segment> segments = new List<Segment>();
        public Strip(double w, bool DEBUG=false)
        {
            this.stripWidth = w;
            this.DEBUG = DEBUG;
        }

        public void addPlain(double len){
            segments.Add(new Segment(
                stripWidth,
                len
            ));
            if(DEBUG) MainClass.debug.drawStrip(len, d);
        }

        public void addFold(MountainValley mv, int angle)
        {
            segments.Add(new Segment(
                stripWidth,
                mv,
                angle
            ));
            if (DEBUG) Console.WriteLine("added fold");
        }
        public void addCornerGadget(MountainValley mv)
        {
            if (d == Direction.Right){
                addFold(mv,45);
                addFold(mv,-45);
            }else{
                addFold(mv,-45);
                addFold(mv,45);
            }
            if (DEBUG) Console.WriteLine("added corner");
        }
        public void turn(){
          d = (d==Direction.Right) ? Direction.Left: Direction.Right;
          if (DEBUG) Console.WriteLine("turn");
        }

        public List<Segment>getSegments(){
            return segments;
        }
        public double getLength()
        {
            double length = 0;
            if (segments != null)
                foreach (var segment in segments)
                    length += segment.length;
            return length;
        }
        public double lengthAt(int index)
        {
            double length = 0;
            if (segments == null) return 0;
            for (int i = 0; i < index;  i++)
                length += segments.ElementAt(i).length;
            return length;
        }
        private double cornerExtension(Triangle t){
          return (d==Direction.Left)? Math.Tan(0.5*Math.PI-t.gamma)*stripWidth : Math.Tan(0.5*Math.PI-t.beta)*stripWidth;
        }
        public void addTriangle(Triangle triangle)
        {
            addPlain (Math.Tan(0.5*Math.PI-triangle.beta)*2*stripWidth);
            addCornerGadget(MountainValley.Valley);
            double posY =2*stripWidth;
            while (posY<triangle.getHeight()){
                //continue turning, we have not reached the bottom
                turn (); // change direction

                double flat = (triangle.b.getDistance(triangle.c)*posY)/triangle.getHeight();
                flat += cornerExtension(triangle);
                addPlain(flat);
                addCornerGadget(MountainValley.Valley);
                posY +=stripWidth;
            }
            turn ();
            addPlain (triangle.b.getDistance(triangle.c)+cornerExtension(triangle)+stripWidth);

            if(d == Direction.Right){
              addFold(MountainValley.Valley, 90);
              turn();
              addPlain(triangle.b.getDistance(triangle.c)+cornerExtension(triangle)+stripWidth);
            }
            counter++;
            if (DEBUG) Console.WriteLine("added triangle to strip");
        }

      public int getNumberOfTriangles(){
        return counter;
      }
    }

    public class Line
    {
        public double x1, x2, y1, y2;
        public Color c;
    }

    public class Fold
    {
        public MountainValley mv;
        private int angle;
        public Fold (int angle, MountainValley direction = MountainValley.Mountain)
        {
            this.mv = direction;
            this.angle = angle;
        }
        public bool fromTop()
        {
            return (angle > 0) ? true:false;
        }
    }
    public class Plain
    {
        private double length;
        public Plain(double len){
            this.length = len;
        }
        public double getLength() {return length;}
    }
    public class Segment
    {
        private double width;
        public double length;
        private Fold f;
        private type t;
        private Plain p;
        public Segment(double w, MountainValley d, int a, type t = type.Fold) // specify fold segment
        {
            this.t = t;
            this.width = w;
            double rad = a * 2 * Math.PI / 360;
            this.length = Math.Abs((w / Math.Tan(rad)));
            this.f = new Fold(a, d);
        }
        public Segment(double w, double l, type t = type.Plain) // specify plain segment
        {
            this.length = l;
            this.t = t;
            this.width = w;
            this.p = new Plain(l);
        }
        public Line draw()
        {
            Line l = new Line();
            l.x1 = 0;
            l.x2 = length;
            if (f.fromTop())
            { l.y1 = 0; l.y2 = width; }
            else
            { l.y1 = width; l.y2 = 0; }
            l.c = (f.mv == MountainValley.Mountain) ? Color.Red : Color.Blue;
            return l;
        }
        public Plain getPlain(){return p;}
        public Fold getFold(){return f;}
        public type getType(){ return t;}
    }
}
