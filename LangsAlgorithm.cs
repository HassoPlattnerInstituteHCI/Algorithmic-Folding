using System;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;
using Mathematics;

namespace inClassHacking{

  public class LangsAlgorithm{

    public double sweepingLength = 0.1;

    List<Edge> edges;
    List<Circle> circles;
    double[] input;
    double s, w, x, y, z;

    double[,] distances;

    List<Crease> creases = new List<Crease>();

    public LangsAlgorithm(List<Circle> circles, double[] input){

      this.input = input;
      this.circles = circles;

      edges = new List<Edge>(); //list of edges filled based on circles[]

    }

    public List<Crease> sweepingProcess(){
      // circles[0] is missing - special case, has to be added later

      for(int i=1; i<circles.Count-1; i++){
        edges.Add(new Edge(circles[i], circles[i+1]));
      }
      edges.Add(new Edge(circles.Last(), circles[1]));

      // edges.Add(new Edge(circles[3].getCenter(), 0, circles[4].getCenter(), 1));
      // edges.Add(new Edge(circles[4].getCenter(), 1, circles[5].getCenter(), 2));
      // edges.Add(new Edge(circles[5].getCenter(), 2, circles[2].getCenter(), 3));
      // edges.Add(new Edge(circles[2].getCenter(), 3, circles[3].getCenter(), 0));


      axialCreases(creases);

      addMarkers(edges);

      List<Edge> initialEdges = new List<Edge>();
      foreach(var e in edges){
        initialEdges.Add(new Edge(e));
      }

      distances = calculateTreeDistances();

      sweep(creases, edges, initialEdges);

      return creases;
    }

    void axialCreases(List<Crease> creases){
      for(int i=0; i<circles.Count-1; i++){
        creases.Add(new Crease(circles[i].getCenter(), circles[i+1].getCenter(), Color.Green));
      }
      creases.Add(new Crease(circles[1].getCenter(), circles[7].getCenter(), Color.Green));
      
    }

    void addMarkers(List<Edge> edges){
      Point2D marker = edges[0].p1 + edges[0].vec*input[1];
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

      // marker = edges[0].p1 + edges[0].vec*input[4];
      // edges[0].addMarker(marker);
    }

    double[,] calculateTreeDistances(){
        double[,] distances = new double[circles.Count, circles.Count];

        for(int i=0; i<circles.Count; i++){
          distances[i, i] = 0;
        }

        distances[0, 1] = input[0] + input[4];
        distances[0, 2] = input[4] + input[5] + input[7] + input[1];
        distances[0, 3] = input[4] + input[5] + input[7] + input[8] + input[10] + input[2];
        distances[0, 4] = input[4] + input[5] + input[7] + input[8] + input[10] + input[11] + input[3];
        distances[0, 5] = input[4] + input[5] + input[7] + input[8] + input[10] + input[11] + input[12];
        distances[0, 6] = input[4] + input[5] + input[7] + input[8] + input[9];        
        distances[0, 7] = input[4] + input[5] + input[6];

        distances[1, 2] = input[0] + input[5] + input[7] + input[1];
        distances[1, 3] = input[0] + input[5] + input[7] + input[8] + input[10] + input[2];
        distances[1, 4] = input[0] + input[5] + input[7] + input[8] + input[10] + input[11] + input[3];
        distances[1, 5] = input[0] + input[5] + input[7] + input[8] + input[10] + input[11] + input[12];
        distances[1, 6] = input[0] + input[5] + input[7] + input[8] + input[9];
        distances[1, 7] = input[0] + input[5] + input[6];

        distances[2, 3] = input[1] + input[8] + input[10] + input[2];
        distances[2, 4] = input[1] + input[8] + input[10] + input[11] + input[3];
        distances[2, 5] = input[1] + input[8] + input[10] + input[11] + input[12];
        distances[2, 6] = input[1] + input[8] + input[9];
        distances[2, 7] = input[1] + input[7] + input[6];

        distances[3, 4] = input[2] + input[11] + input[3];
        distances[3, 5] = input[2] + input[11] + input[12];
        distances[3, 6] = input[2] + input[10] + input[9];
        distances[3, 7] = input[2] + input[10] + input[8] + input[7] + input[6];

        distances[4, 5] = input[3] + input[12];
        distances[4, 6] = input[3] + input[11] + input[10] + input[9];
        distances[4, 7] = input[3] + input[11] + input[10] +input[8] + input[7] + input[6];

        distances[5, 6] = input[12] + input[11] + input[10] +input[9];
        distances[5, 7] = input[12] + input[11] + input[10] +input[8] + input[7] + input[6];

        distances[6, 7] = input[9] + input[8] + input[7] + input[6];

        fill2ndHalf(distances);

        return distances;


    }

    void fill2ndHalf(double[,] distances){
      int tableSize = (int)Math.Sqrt(distances.Length); //distances is square table - sqrt of size is int
      for(int i=0; i<tableSize; i++){
        for(int j=0; j<i; j++){
          distances[i, j] = distances[j, i];
        }
      }
    }

    void sweep(List<Crease> creases, List<Edge> edges, List<Edge> initialEdges){
      drawRivers(creases, edges, initialEdges);

      // Console.WriteLine("initial");
      // foreach(var it in initialEdges){
      //   Console.Write(it.p1);
      // }
      // Console.WriteLine("\nedges");
      // foreach(var it in edges){
      //   Console.Write(it.index1);
      // }
      Console.WriteLine();

      for(int i=0; i<edges.Count; i++){
        Edge edge = edges[i];

        if(edge.getLength() < 5*sweepingLength){ //recursion's end condition 
          return;
        }

        //splitting events
        if(edges.Count > 3){ //do not split triangles
          for(int j=i; j<edges.Count; j++){
            if(i==0 && j==edges.Count-1) continue;
            Edge secondEdge = edges[j];
            if(Math.Abs(edge.index1 - secondEdge.index1) <= 1) continue; //do not split edges next to each other
            if(Math.Abs(edge.index1 - secondEdge.index1) == edges.Count) continue; //do not split last and first edge (next to each other)

            if(edge.p1.getDistance(secondEdge.p1) - sweepingLength <
                  distances[edge.index1, secondEdge.index1]){

              Console.WriteLine("split between " + edge.index1 + " and " + secondEdge.index1);

              //avoid splitting same edges twice
              distances[edge.index1, secondEdge.index1] = -1;
              distances[secondEdge.index1, edge.index1] = -1;
              
              creases.Add(new Crease(edge.p1, secondEdge.p1, Color.Grey));
              
              Edge splittingEdge = new Edge(secondEdge.p1, secondEdge.index1, edge.p1, edge.index1);
              Vector splitVector = new Vector(edge.p1, secondEdge.p1);

              //left poly
              List<Edge> e = new List<Edge>();
              List<Edge> initialEdges1 = new List<Edge>();
              for(int k=i; k<j; k++){
                initialEdges1.Add(new Edge(edges[k]));
                e.Add(new Edge(edges[k]));

                foreach(var marker in edges[k].markers){
                  Vector toMarker = new Vector(edges[k].p1, marker);
                  double d = toMarker.getLength();
                  // splittingEdge.addMarker(edges[k].p1+splitVector.normalized()*d);
                }
              } 
              
              
              initialEdges1.Add(new Edge(splittingEdge));
              e.Add(splittingEdge);
              
              //right poly
              List<Edge> e2 = new List<Edge>();
              List<Edge> initialEdges2 = new List<Edge>();
              Edge splittingEdge2 = new Edge(edge.p1, edge.index1, secondEdge.p1, secondEdge.index1);
              int n;
              for(n=0; n<i; n++){
                e2.Add(new Edge(edges[n]));
                initialEdges2.Add(new Edge(edges[n]));

                foreach(var marker in edges[n].markers){
                  Vector toMarker = new Vector(edges[n].p1, marker);
                  double d = toMarker.getLength();
                  // splittingEdge2.addMarker(edges[n].p1+splitVector.normalized()*d);
                }
              }
              for(int m=j; m<edges.Count; m++){
                e2.Add(new Edge(edges[m])); 
                initialEdges2.Add(new Edge(edges[m]));

                foreach(var marker in edges[m].markers){
                  Vector toMarker = new Vector(edges[m].p2, marker);
                  double d = toMarker.getLength();
                  splittingEdge2.addMarker(splittingEdge2.p1+splitVector.normalized()*d);
                }       
              }
              
              
              e2.Insert(n, splittingEdge2);
              initialEdges2.Insert(n, new Edge(splittingEdge2));

              // Console.WriteLine("in e: ");
              // foreach(var it in e){
              //   Console.Write(it.index1);
              // }
              // Console.WriteLine("\nin e2:");
              // foreach(var it in e2){
              //   Console.Write(it.index1);
              // }
              // Console.WriteLine();
              
              
              //   Console.WriteLine("start recursion with " + e.Count + " Edges and " + initialEdges1.Count + " initialEdges.");
              sweep(creases, e, initialEdges1);
              sweep(creases, e2, initialEdges2);
              return;
            }
          }
        }

        edge.parallelSweep(sweepingLength); //sweep every edge
      }
      edges = updateVerticesandMarkers(edges); //update vertices of all edges

      foreach(var edge in edges){
        creases.Add(new Crease(edge.p1, edge.p2, Color.Red));
      }

      sweep(creases, edges, initialEdges);
      return;
    }


    List<Edge> updateVerticesandMarkers(List<Edge> edges){
      edges[0].updateVertices(edges.Last(), edges[1]);
      edges[0].updateMarkers();

      for(int i = 1; i<edges.Count-1; i++){
        edges[i].updateVertices(edges[i-1], edges[i+1]);
        edges[i].updateMarkers();
      }
      
      edges.Last().updateVertices(edges[edges.Count-2], edges[0]);
      edges.Last().updateMarkers();


      return edges;
    }
  

  void drawRivers(List<Crease> creases, List<Edge> edges, List<Edge> initialEdges){
    Console.WriteLine("in drawRiver");
    Console.WriteLine("init: " + initialEdges.Count);
    Console.WriteLine("edges: " + edges.Count);
    for(int l=0; l<edges.Count; l++){
        Edge edge = edges[l];
        for(int k=0; k<edge.markers.Count; k++){
          Console.WriteLine(edge.index1);
          Console.WriteLine(initialEdges[l].index1);
          // Console.WriteLine(k);
          // if(edge.markers[k]!= null){
            creases.Add(new Crease(initialEdges[l].markers[k], edge.markers[k], Color.Blue));
          // }
        }
    }

    Console.WriteLine("out drawRiver");
  }
}
}