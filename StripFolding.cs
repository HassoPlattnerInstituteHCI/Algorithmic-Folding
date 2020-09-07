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

        private List<Segment> segments = new List<Segment>();
        public Strip(double w, bool DEBUG)
        {
            this.stripWidth = w;
            this.DEBUG = DEBUG;
        }

        public void addPlain(double len){
            Segment s = new Segment(
                stripWidth, 
                len
            );
            segments.Add(s);
        }

        public void addFold(MountainValley mv, int angle)
        {
            Segment s = new Segment(
                stripWidth, 
                mv, 
                angle
            );
            segments.Add(s);
        }

        public void addCornerGadget(MountainValley mv)
        {
            if (d == Direction.Right){
                addFold(mv,45);
                addFold(mv,-45);
            }
            if (d == Direction.Left){
                addFold(mv,-45);
                addFold(mv,45);
            }
            // turn(); //turn() is called in addTriangle(), would be doubled here
        }
        public void turn(){
                if (d == Direction.Right)
                    d = Direction.Left;
                else
                    d = Direction.Right;
        }

        public List<Segment>getSegments(){
            return segments;
        }
        public double getLength()
        {
            double length = 0;
            if (segments != null){
                foreach (var segment in segments)
                {
                    length += segment.length;
                }
            }
            return length;
        }
        public double lengthAt(int index)
        {
            double length = 0;
            if (segments == null)
                return 0;
            for (int i = 0; i < index;  i++)
            {
                length += segments.ElementAt(i).length;
            }
            return length;
        }
        private double cornerExtension(Triangle t){
                if (d==Direction.Left){
                    return Math.Tan(0.5*Math.PI-t.gamma)*stripWidth;
                    }
                else{
                    return Math.Tan(0.5*Math.PI-t.beta)*stripWidth;
                    }
        }
        public void addTriangle(Triangle triangle)
        {
            addPlain (Math.Tan(0.5*Math.PI-triangle.beta)*2*stripWidth);
            if(DEBUG) MainClass.debug.draw(Math.Tan(0.5*Math.PI-triangle.beta)*2*stripWidth, d);
            addCornerGadget(MountainValley.Valley);
            double posY =2*stripWidth;
            while (posY<triangle.getHeight()){
                //continue turning, we have not reached the bottom
                turn (); // change direction

                double flat = (triangle.b.getDistance(triangle.c)*posY)/triangle.getHeight();
                flat += cornerExtension(triangle);
                addPlain(flat);
                if(DEBUG) MainClass.debug.draw(flat,  d);
                addCornerGadget(MountainValley.Valley);
                posY +=stripWidth;
            }
            turn (); 
            addPlain (triangle.b.getDistance(triangle.c)+cornerExtension(triangle)+stripWidth);
            if(DEBUG) MainClass.debug.draw(triangle.b.getDistance(triangle.c)+cornerExtension(triangle)+stripWidth, d);

            if(d == Direction.Right){
              addFold(MountainValley.Valley, 90);
              turn();
              addPlain(triangle.b.getDistance(triangle.c)+cornerExtension(triangle)+stripWidth);
              if(DEBUG) MainClass.debug.draw(triangle.b.getDistance(triangle.c)+cornerExtension(triangle)+stripWidth, d);
            }
            if (DEBUG) Console.WriteLine("added triangle to strip");
        }
    }

    public class Plain
    {
        private double length;
        public Plain(double len){
            this.length = len;
        }
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
            if (angle > 0)
                return true;
            else
                return false;
        }
    }

    public class Segment
    {
        private double width;
        public double length;
        private Fold f;
        private type t;

        public Segment(double w, MountainValley d, int a, type t = type.Fold) // specify fold segment
        {
            this.t = t;
            this.width = w;
            double rad = a * 2 * Math.PI / 360;
            this.length = Math.Abs((w / Math.Tan(rad)));
            f = new Fold(a, d);
        }
        public Segment(double w, double l, type t = type.Plain) // specify plain segment
        {
            this.length = l;
            this.t = t;
            this.width = w;
            Plain p = new Plain(l);
        }
        public Fold getFold()
        {
            return f;
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
            if (f.mv == MountainValley.Mountain) { l.c = Color.Red; }
            else { l.c = Color.Blue; }
            return l;
        }

        public type getType()
        {
            return t;
        }
    }
}