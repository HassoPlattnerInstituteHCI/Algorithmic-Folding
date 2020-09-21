using System;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;
using Mathematics;

namespace inClassHacking{

  public class LangsAlgorithm{

    public double sweepingLength = 0.5;

    List<Edge> edges;
    List<Circle> circles;
    double[] distances;
    double[] input;
    double s, w, x, y, z;

    List<Crease> creases = new List<Crease>();

    public LangsAlgorithm(List<Circle> circles, double[] input, double[] distances){

      this.input = input;
      this.distances = distances;
      this.circles = circles;

      Point2D A = circles[1].getCenter();
      Point2D B = circles[2].getCenter();
      Point2D C = circles[3].getCenter();
      Point2D D = circles[4].getCenter();
      Point2D E = circles[7].getCenter();
      Point2D F = circles[6].getCenter();
      Point2D G = circles[5].getCenter();

      Edge AB = new Edge(A, B);
      Edge BC = new Edge(B, C);
      Edge CD = new Edge(C, D);
      Edge DE = new Edge(D, E);
      Edge EF = new Edge(E, F);
      Edge FG = new Edge(F, G);
      Edge GA = new Edge(G, A);

      Edge[] e = {AB, BC, CD, DE, EF, FG, GA};
      edges = new List<Edge>(e);
    }

    public List<Crease> sweepingProcess(){
      axialCreases(creases);

      addMarkers(ref edges);

      List<Edge> initialEdges = new List<Edge>();
      foreach(var e in edges){
        initialEdges.Add(new Edge(e));
      }

      sweep(creases, edges, initialEdges);
     
      for(int i=0; i<edges.Count; i++){
        Console.WriteLine(edges[i].p1);
        Console.WriteLine(edges[i].p2);
        Console.WriteLine();
        Console.WriteLine(initialEdges[i].p1);
        Console.WriteLine(initialEdges[i].p2);
        Console.WriteLine();
        Console.WriteLine();
        foreach(var m in edges[i].markers){
          Console.WriteLine("Marker: " + m);
        }
        foreach(var m in initialEdges[i].markers){
          Console.WriteLine("Marker: " + m);
        }
      }
      return creases;
    }

    void axialCreases(List<Crease> creases){
      creases.Add(new Crease(circles[2].getCenter(), circles[1].getCenter(), Color.Green));
      creases.Add(new Crease(circles[1].getCenter(), circles[5].getCenter(), Color.Green));
      creases.Add(new Crease(circles[7].getCenter(), circles[6].getCenter(), Color.Green));
      creases.Add(new Crease(circles[3].getCenter(), circles[4].getCenter(), Color.Green));
      creases.Add(new Crease(circles[3].getCenter(), circles[2].getCenter(), Color.Green));
      creases.Add(new Crease(circles[4].getCenter(), circles[7].getCenter(), Color.Green));
      creases.Add(new Crease(circles[5].getCenter(), circles[6].getCenter(), Color.Green));
    }

    void addMarkers(ref List<Edge> edges){
      Point2D marker = edges[0].p1 + edges[0].vec*input[0];
      edges[0].addMarker(marker);

      marker += edges[0].vec*input[5];
      edges[0].addMarker(marker);

      marker += edges[0].vec*input[7];
      edges[0].addMarker(marker);

      marker = edges[1].p1 + edges[1].vec * input[1];
      edges[1].addMarker(marker);

      marker += edges[1].vec*input[8];
      edges[1].addMarker(marker);

      marker += edges[1].vec * input[10];
      edges[1].addMarker(marker);

      marker = edges[2].p1 + edges[2].vec * input[2];
      edges[2].addMarker(marker);

      marker += edges[2].vec * input[11];
      edges[2].addMarker(marker);

      marker = edges[6].p1 + edges[6].vec*input[6];
      edges[6].addMarker(marker);

      marker += edges[6].vec*input[5];
      edges[6].addMarker(marker);

      marker = edges[5].p1 + edges[5].vec * input[9];
      edges[5].addMarker(marker);

      marker += edges[5].vec * input[8];
      edges[5].addMarker(marker);

      marker += edges[5].vec * input[7];
      edges[5].addMarker(marker);

      marker = edges[4].p2 - edges[4].vec*input[9];
      edges[4].addMarker(marker);

      marker -= edges[4].vec * input[10];
      edges[4].addMarker(marker);

      marker -= edges[4].vec * input[11];
      edges[4].addMarker(marker);
    }

    void sweep(List<Crease> creases, List<Edge> edges, List<Edge> initialEdges){

      Console.WriteLine("lllll" + edges[0].markers.Count);
      foreach(var edge in edges){
        if(edge.getLength() < sweepingLength){
          for(int i=0; i<edges.Count; i++){
            for(int k=0; k<edges[i].markers.Count; k++){
              creases.Add(new Crease(edges[i].markers[k], initialEdges[i].markers[k], Color.Blue));
              Console.WriteLine("ADD CREASE" + edges[i].markers[k].ToString() + initialEdges[i].markers[k].ToString());
            }
          }
          return;
        }
        edge.parallelSweep(sweepingLength);
      }
      edges = updateVertices(edges);
      Console.WriteLine("lllll" + edges[0].markers.Count);

      foreach(var edge in edges){
        creases.Add(new Crease(edge.p1, edge.p2, Color.Red));
        Console.WriteLine(edge.markers.Count);
        // foreach(var marker in edge.markers){
        //   creases.Add(new Crease(marker, circles[0].getCenter(), Color.Green));
        //   Console.WriteLine("MARKER");
        // }
      }

      sweep(creases, edges, initialEdges);
    }

    List<Edge> updateVertices(List<Edge> edges){
      edges[0].updateVertices(edges.Last(), edges[1]);

      for(int i = 1; i<edges.Count-1; i++){
        edges[i].updateVertices(edges[i-1], edges[i+1]);
      }
      
      edges[edges.Count-1].updateVertices(edges[edges.Count-2], edges[0]);

      return edges;
    }
  }
}