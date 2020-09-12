using System;
using System.Collections.Generic;

namespace inClassHacking{

  class Folding{

    List<Circle> circles;
    List<River> rivers;
    double[] distances;

    List<Crease> creases = new List<Crease>();

    public Folding(List<Circle> circles, List<River> rivers, double[] distances){
      this.circles = circles;
      this.rivers = rivers;
      this.distances = distances;
    }

    public List<Crease> calculateCreases(){
      axialCreases(creases);

      return creases;

    }
    void axialCreases(List<Crease> creases){
      creases.Add(new Crease(circles[2].getCenter(), circles[1].getCenter(), Color.Green));
      creases.Add(new Crease(circles[1].getCenter(), circles[9].getCenter(), Color.Green));
      creases.Add(new Crease(circles[9].getCenter(), circles[8].getCenter(), Color.Green));
      creases.Add(new Crease(circles[8].getCenter(), circles[7].getCenter(), Color.Green));
      creases.Add(new Crease(circles[9].getCenter(), circles[10].getCenter(), Color.Green));
      creases.Add(new Crease(circles[10].getCenter(), circles[11].getCenter(), Color.Green));
      creases.Add(new Crease(circles[11].getCenter(), circles[4].getCenter(), Color.Green));
      creases.Add(new Crease(circles[11].getCenter(), circles[5].getCenter(), Color.Green));
      creases.Add(new Crease(circles[4].getCenter(), circles[3].getCenter(), Color.Green));
      creases.Add(new Crease(circles[5].getCenter(), circles[6].getCenter(), Color.Green));
    }


  }
}