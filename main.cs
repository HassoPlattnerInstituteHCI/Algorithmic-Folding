using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {

    public static double[] input = {4,4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4}; //{3, 4, 5, 6, 1, 2, 1, 1, 3, 1, 2, 2, 3};
    public const bool DEBUG = true;
    static int zoomFactor = 200;

    public static void Main (string[] args) {
      double[] distances = calculateDistances(input);
      FileHandler f = new FileHandler(DEBUG, distances[4], zoomFactor); 
      Positioning positioning = new Positioning(distances, input);

      List<Circle> circles = positioning.calculateCirclePositioning();

      Folding folding = new Folding(circles, input, distances);
      List<Crease> creases = folding.calculateCreases();

      f.exportSVG(circles, creases);

    }


    static double[] calculateDistances(double[] input){
      //TODO: calculate distan
      double[] ret = {9.63, 2.7, 14.56, 6.63, 29.26};//{10.16, 4.48, 13.78, 8.13, 32.26}; //{10.16,   4.21, 10.12,   8.16,  28.33};
      return ret;
    }

    
  }
}