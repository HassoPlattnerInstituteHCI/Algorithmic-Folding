using System;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;
using Mathematics;

namespace inClassHacking{

  public class LangsAlgorithm{

    public double sweepingLength = 0.05;
    public bool DEBUG;
    List<Edge> edges = new List<Edge>();
    List<Circle> circles = new List<Circle>();

    double[,] distances;

    List<Crease> creases = new List<Crease>();
    List<Edge> inputEdges = new List<Edge>();
    List<LeafNode> nodes = new List<LeafNode>();

    public LangsAlgorithm(List<Circle> circles, bool debug=false){
      this.circles = circles;
      foreach(var circle in circles){
        nodes.Add(circle.node);
      }
      if (debug)
        this.DEBUG=debug;
    }

    public List<Crease> sweepingProcess(){
      axialCreasesAndMarkers(creases);                // create creases and edges between the circles
      foreach(var e in edges){               // copy edges to inputEdges
        inputEdges.Add(new Edge(e));
      }
      distances = calculateTreeDistances(); // builds a matrix of all distances in the tree
      sweep(creases, edges, inputEdges);    // the actual sweeping of the polygon
      return creases;
    }
    void axialCreasesAndMarkers(List<Crease> creases){  // create edges and creases that connect the circles
    Edge newEdge;
      for(int i=0; i<circles.Count-1; i++){
        creases.Add(new Crease(circles[i].getCenter(), circles[i+1].getCenter(), Color.Green));
        newEdge = new Edge(circles[i].getCenter(), i, circles[i+1].getCenter(), i+1);
        edges.Add(newEdge);
        if(nodes[i].relatedNode != nodes[i+1].relatedNode)
           addMarker(newEdge, nodes[i], nodes[i+1]);
      }
      creases.Add(new Crease(circles[0].getCenter(), circles.Last().getCenter(), Color.Green)); //connect the last circle with an edge to the first one to close the polygon
      newEdge = new Edge(circles.Last().getCenter(), circles.Count-1, circles[0].getCenter(), 0);
      edges.Add(newEdge);
      if(nodes[0].relatedNode != nodes.Last().relatedNode)
        addMarker(newEdge, nodes.Last(), nodes[0]);
    }

    void addMarker(Edge edge, LeafNode node1, LeafNode node2){
      if(node1.relatedNode != node2.relatedNode)
        addMarker(edge, node1.relatedNode, node2);
    }

    bool addMarker(Edge edge, InteriorNode inNode, LeafNode node2, InteriorNode lastChecked=null){
      if(inNode == node2.relatedNode){
        // edge.addMarker(node2.circle.getCenter()-edge.vec*node2.size);
        edge.addMarker(edge.p2-edge.vec*node2.size);
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
              if(edge.index1 - secondEdge.index1 == edges.Count) continue; //do not split last and first edge (next to each other)
              if(edge.vec == secondEdge.vec) continue;
              if(isSplitEvent(edge,secondEdge,inputEdges,edges)){
                  again = false;
                  if (DEBUG){Console.WriteLine("split between " + edge.index1 + " and " + secondEdge.index1 + " with length: ");
                  Console.WriteLine(edge.p1.getDistance(secondEdge.p1));}

                  //avoid splitting same edges twice
                  distances[edge.index1, secondEdge.index1] = -1;
                  distances[secondEdge.index1, edge.index1] = -1;

                  creases.Add(new Crease(edge.p1, secondEdge.p1, Color.Grey));

                  Edge splittingEdge = new Edge(secondEdge.p1, secondEdge.index1, edge.p1, edge.index1);
                  Edge splittingEdge2 = new Edge(edge.p1, edge.index1, secondEdge.p1, secondEdge.index1);
                  List<Edge> e,e2,initialEdges1,initialEdges2;
                  initialEdges1 = new List<Edge>();
                  initialEdges2 = new List<Edge>();
                  e = new List<Edge>();
                  e2 = new List<Edge>();

                  for(int k=i; k<j; k++){
                    processEdge(edges[k], initialEdges1, e, splittingEdge, j-i);
                  }
                  int n;
                  for(n=0; n<i; n++){
                    processEdge(edges[n], initialEdges2, e2, splittingEdge2, i+edges.Count-j);
                  }
                  for(int m=j; m<edges.Count; m++){
                    processEdge(edges[m], initialEdges2, e2, splittingEdge2, edges.Count-j);
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

    void processEdge(Edge edge, List<Edge> initialEdges, List<Edge> e, Edge splittingEdge, int z){
      initialEdges.Add(new Edge(edge));
      e.Add(new Edge(edge));
      if(z<3)
        splittingEdge = addMarkersToSplittingEdge(splittingEdge, edge);
    }

    bool isSplitEvent(Edge edge, Edge secondEdge, List<Edge> input, List<Edge> es){
      double equationSolution = Int64.MaxValue;
      if(secondEdge.vec == input[secondEdge.index1].vec && edge.vec == input[edge.index1].vec){
        equationSolution = solveEquation(edge, input[secondEdge.index1],input[secondEdge.index1].p1,secondEdge, secondEdge.p1, secondEdge.p1);
      }else{ //the according edge was already splitted so we try the other edge of this vertex
        Edge altSecondEdge = (secondEdge.index1!=0) ? es[secondEdge.index1-1] : es.Last();
        Edge altInputEdge = (secondEdge.index1!=0) ? inputEdges[secondEdge.index1-1] : inputEdges.Last();
        equationSolution = solveEquation(edge, altInputEdge,altInputEdge.p2,altSecondEdge, altSecondEdge.p2, secondEdge.p1);
      }
      return (equationSolution < distances[edge.index1, secondEdge.index1]);
    }
    double solveEquation(Edge edge, Edge firstEdge, Point2D p1, Edge secondEdge, Point2D p2, Point2D p3){
      double AA_, CC_;
      Point2D A_, C_;
      C_ = Folding.findIntersection(firstEdge.vec, p1, secondEdge.vec.getNormalRight(), p2);
      CC_ = p1.getDistance(C_);
      A_ = Folding.findIntersection(inputEdges[edge.index1].vec, inputEdges[edge.index1].p1, edge.vec.getNormalRight(), edge.p1);
      AA_ = inputEdges[edge.index1].p1.getDistance(A_);
      return Math.Round(edge.p1.getDistance(p3) + AA_ + CC_, 2);
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
