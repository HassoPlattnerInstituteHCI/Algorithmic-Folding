using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace inClassHacking{

  class MainClass {

    public static double[] input = {4, 4, 6, 8, 1, 1, 1, 1, 1, 1, 1, 2, 4};
    public const bool DEBUG = true;
    static int zoomFactor = 200;

    public static void Main (string[] args) {
      double[] distances = calculateDistances(input);
      FileHandler f = new FileHandler(DEBUG, distances[4], zoomFactor); 
      Positioning positioning = new Positioning(distances, input);

      List<Circle> circles = positioning.calculateCirclePositioning();
      List<River> rivers = positioning.calculateRiverPositioning(circles);

      f.exportSVG(circles, rivers);

    }


    static double[] calculateDistances(double[] input){
      //TODO: calculate distan
      double[] ret = {9.63, 2.7, 14.56, 6.63, 29.26};
      return ret;
    }

    
  }
}