using System;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;
using Mathematics;

namespace inClassHacking{

  public class LangsAlgorithm{

    public double sweepingLength = 0.05;
    double undef = -1;

    List<Edge> edges = new List<Edge>();
    List<Circle> circles = new List<Circle>();

    double[,] distances;
    int removedEdgesCounter = 0;

    List<Crease> creases = new List<Crease>();
    List<Edge> inputEdges = new List<Edge>();
    List<LeafNode> nodes = new List<LeafNode>();

    public LangsAlgorithm(List<Circle> circles){
      this.circles = circles;
      foreach(var circle in circles){
        nodes.Add(circle.node);
      }
    }

    public List<Crease> sweepingProcess(){
      axialCreases(creases);                // create creases and edges between the circles
      foreach(var e in edges)               // copy edges to inputEdges
        inputEdges.Add(new Edge(e));
      distances = calculateTreeDistances(); // builds a matrix of all distances in the tree
      sweep(creases, edges, inputEdges);    // the actual sweeping of the polygon
      return creases;
    }
    void axialCreases(List<Crease> creases){  // create edges and creases that connect the circles
      for(int i=0; i<circles.Count-1; i++){
        creases.Add(new Crease(circles[i].getCenter(), circles[i+1].getCenter(), Color.Green));
        edges.Add(new Edge(circles[i].getCenter(), i, circles[i+1].getCenter(), i+1));
      }
      creases.Add(new Crease(circles[0].getCenter(), circles.Last().getCenter(), Color.Green)); //connect the last circle with an edge to the first one to close the polygon
      edges.Add(new Edge(circles.Last().getCenter(), circles.Count-1, circles[0].getCenter(), 0));
    }

    void addEdgesWithMarkers(List<Edge> edges, List<LeafNode> nodes){
      Edge newEdge;
      for(int i=0; i<nodes.Count-1; i++){
        newEdge = new Edge(nodes[i].circle.getCenter(), i, nodes[i+1].circle.getCenter(), i+1);
        if(nodes[i].relatedNode != nodes[i+1].relatedNode)
           addMarker(newEdge, nodes[i], nodes[i+1]);
        edges.Add(newEdge);
      }
      newEdge = new Edge(nodes.Last().circle.getCenter(), nodes.Count-1, nodes[0].circle.getCenter(), 0);
      if(nodes[0].relatedNode != nodes.Last().relatedNode)
        addMarker(newEdge, nodes.Last(), nodes[0]);
      edges.Add(newEdge);
    }

    void addMarker(Edge edge, LeafNode node1, LeafNode node2){
      if(node1.relatedNode != node2.relatedNode)
        addMarker(edge, node1.relatedNode, node2);
    }

    bool addMarker(Edge edge, InteriorNode inNode, LeafNode node2, InteriorNode lastChecked=null){
      if(inNode == node2.relatedNode){
        edge.addMarker(node2.circle.getCenter()-edge.vec*node2.size);
        return true;
      }

      foreach(var next in inNode.relatedInteriorNodes.Keys){
        if(next == lastChecked) continue;
        if(addMarker(edge, next, node2, inNode)){
          edge.addMarker(edge.markers.Last()-edge.vec*inNode.relatedInteriorNodes[next]);
          return true;
        }
      }
      return false;
    }
    double[,] calculateTreeDistances(){ // builds a matrix of all distances in the tree
        double[,] distances = new double[nodes.Count, nodes.Count];
        for(int i=0; i<nodes.Count; i++)
          for(int j=0; j<nodes.Count; j++)
            distances[i, j] = nodes[i].getTreeDistanceTo(nodes[j]);
        return distances;
    }
    public void sweep(List<Crease> creases, List<Edge> edges, List<Edge> initialEdges){
      bool again = true;
      while(again){
        parallelSweep(edges, sweepingLength);     //sweep every edge by sweepingLength
        edges = updateVerticesandMarkers(edges);  //update vertices of polygon
        drawRivers(creases, edges, initialEdges);
        for(int i=0; i<edges.Count; i++){
          Edge edge = edges[i];
          if(edge.getLength() < 2*sweepingLength){  //contraction event (next iteration we would have 0 length)
            edges.Remove(edge);
            removedEdgesCounter++;
          }
          if(edges.Count<3){                        // rabit ear molecule
            for(int z=0; z<edges.Count-1; z++)
              if(edges[z].getLength() > 2*sweepingLength)
                creases.Add(new Crease(edges[z].p1, edges[z].p2, Color.Red));
            return;
          }
          if(linedUp(edges)) return;              // colinear edges make no sense--polygon is death now
          if(edges.Count > 3){                    // no contractions found, check for splitting events
            for(int j=i; j<edges.Count; j++){
              if(i==0 && j==edges.Count-1) continue;
              Edge secondEdge = edges[j];
              if(secondEdge==null) continue;
              if(Math.Abs(edge.index1 - secondEdge.index1) <= 1) continue; //do not split edges next to each other
              if(Math.Abs(edge.index1 - secondEdge.index1) == edges.Count) continue; //do not split last and first edge (next to each other)
              if(edge.vec == secondEdge.vec) continue;
              if(isSplitEvent(edge,secondEdge,inputEdges, j)){
                  again = false;
                  Console.WriteLine("split between " + edge.index1 + " and " + secondEdge.index1 + " with length: ");
                  Console.WriteLine(edge.p1.getDistance(secondEdge.p1));

                  //avoid splitting same edges twice
                  distances[edge.index1, secondEdge.index1] = -1;
                  distances[secondEdge.index1, edge.index1] = -1;

                  creases.Add(new Crease(edge.p1, secondEdge.p1, Color.Grey));

                  //splitEdges(edge,secondEdge,i);
                  //left poly
                  Edge splittingEdge = new Edge(secondEdge.p1, secondEdge.index1, edge.p1, edge.index1);
                  Edge splittingEdge2 = new Edge(edge.p1, edge.index1, secondEdge.p1, secondEdge.index1);
                  List<Edge> e,e2,initialEdges1,initialEdges2;
                  initialEdges1 = new List<Edge>();
                  initialEdges2 = new List<Edge>();
                  e = new List<Edge>();
                  e2 = new List<Edge>();

                  for(int k=i; k<j; k++){
                    initialEdges1.Add(new Edge(edges[k]));
                    e.Add(new Edge(edges[k]));
                    if(j-i<3)
                      splittingEdge = addMarkersToSplittingEdge(splittingEdge, edges[k]);
                  }
                  int n;
                  for(n=0; n<i; n++){
                    e2.Add(new Edge(edges[n]));
                    initialEdges2.Add(new Edge(edges[n]));
                    if(i+edges.Count-j < 3)
                      splittingEdge2 = addMarkersToSplittingEdge(splittingEdge2, edges[n]);
                  }
                  for(int m=j; m<edges.Count; m++){
                    e2.Add(new Edge(edges[m]));
                    initialEdges2.Add(new Edge(edges[m]));
                    if(i+edges.Count-j < 3)
                      splittingEdge2 = addMarkersToSplittingEdge(splittingEdge2, edges[m]);
                  }
                  foreach(var marker in splittingEdge.markers){
                    splittingEdge2.addMarker(marker);
                  }
                  foreach(var marker in splittingEdge2.markers){
                    splittingEdge.addMarker(marker);
                  }
                  initialEdges1.Add(new Edge(splittingEdge));
                  e.Add(splittingEdge);
                  initialEdges2.Insert(n, new Edge(splittingEdge2));
                  e2.Insert(n, splittingEdge2);

                  sweep(creases, e, initialEdges1);
                  sweep(creases, e2, initialEdges2);
              }
            }
          }
        }
      }
    }
    bool isSplitEvent(Edge edge, Edge secondEdge,List<Edge> inputEdges, int j){
      double equationSolution = Int64.MaxValue;
      double AA_ = undef;
      double CC_ = undef;
      Point2D A_ = null;
      Point2D C_ = null;

      if(secondEdge.vec == inputEdges[secondEdge.index1].vec && edge.vec == inputEdges[edge.index1].vec){
          C_ = Folding.findIntersection(inputEdges[secondEdge.index1].vec, inputEdges[secondEdge.index1].p1, secondEdge.vec.getNormalRight(), secondEdge.p1);
          CC_ = inputEdges[secondEdge.index1].p1.getDistance(C_);
          A_ = Folding.findIntersection(inputEdges[edge.index1].vec, inputEdges[edge.index1].p1, edge.vec.getNormalRight(), edge.p1);
          AA_ = inputEdges[edge.index1].p1.getDistance(A_);
          equationSolution = edge.p1.getDistance(secondEdge.p1) + AA_ + CC_;
          // Console.WriteLine("first");

          equationSolution = Math.Round(equationSolution, 2);

      }else{ //the according edge was already splitted so we try the other edge of this vertex

          Edge altSecondEdge = (j!=0) ? edges[j-1] : edges.Last();
          Edge altInputEdge = (j!=0) ? inputEdges[j-1] : inputEdges.Last();

            C_ = Folding.findIntersection(altInputEdge.vec, altInputEdge.p2, altSecondEdge.vec.getNormalRight(), altSecondEdge.p2);
            CC_ = altInputEdge.p2.getDistance(C_);
            A_ = Folding.findIntersection(inputEdges[edge.index1].vec, inputEdges[edge.index1].p1, edge.vec.getNormalRight(), edge.p1);
            AA_ = inputEdges[edge.index1].p1.getDistance(A_);
            equationSolution = edge.p1.getDistance(secondEdge.p1) + AA_ + CC_;
          }
      return (equationSolution < distances[edge.index1, secondEdge.index1]);
    }
    bool linedUp(List<Edge> edges){
      for(int i=1; i<edges.Count; i++){
        if(edges[i].vec != edges[i-1].vec && edges[i].vec != edges[i-1].vec.getReverse()){
          return false;
        }
      }
      return true;
    }

    void parallelSweep(List<Edge> edges, double sweepingLength){
      foreach(var edge in edges){
        edge.parallelSweep(sweepingLength);
      }
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
    for(int l=0; l<edges.Count; l++){
        Edge edge = edges[l];
        for(int k=0; k<edge.markers.Count; k++){
          if(!(edge.markers[k] == null)){
            if(k>initialEdges[l].markers.Count-1)continue;
            creases.Add(new Crease(initialEdges[l].markers[k], edge.markers[k], Color.Blue));
          }
        }
        creases.Add(new Crease(edge.p1, initialEdges[l].p1, Color.Red));
    }
  }

  Edge addMarkersToSplittingEdge(Edge splittingEdge, Edge edge){
    Vector splitVector = new Vector(splittingEdge.p2, splittingEdge.p1).normalized();
    foreach(var marker in edge.markers){
      if(!(marker == null)){
        if(edge.p2 == splittingEdge.p1){
          double d = edge.p2.getDistance(marker);
          splittingEdge.addMarker(splittingEdge.p1+splitVector.getReverse()*d);
        }else{
          double d = edge.p1.getDistance(marker);
          splittingEdge.addMarker(splittingEdge.p2-splitVector.getReverse()*d);
        }
      }
    }
    return splittingEdge;
  }
}
}
