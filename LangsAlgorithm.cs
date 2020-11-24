using System;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;
//using Mathematics;

namespace inClassHacking{

  public class LangsAlgorithm{
    public double sweepingLength = 0.05;
    public bool DEBUG;
    public bool VISUAL;
    public int zoom;
    public static Color axialCreaseColor = Color.Black;
    public static Color riverColor = Color.Blue;
    public static Color creaseColor = Color.Red;
    public static Color internalCreaseColor = Color.Grey;
    int counter=0;
    List<PolygonEdge> inputEdges = new List<PolygonEdge>();

    public LangsAlgorithm(Tree tree, bool debug=false, bool visual=false, int zoomFactor=90, double sweepingLength = 0.05){
      tree.setLeafNodes();
      this.DEBUG=debug;
      this.VISUAL=visual;
      this.zoom = zoomFactor;
      this.sweepingLength = 0.05;
    }
    public List<Crease> sweepingProcess(Tree tree){
      (List<Crease> creases, List<PolygonEdge> edges)= axialCreasesAndMarkers(tree);  // create creases and edges between the circles
      foreach(var e in edges)                                                  // copy edges to inputEdges
        inputEdges.Add(new PolygonEdge(e));
      tree.calculateTreeDistances();                                           // builds a matrix of all distances in the tree
      sweep(tree, creases, edges, inputEdges);                                 // the actual sweeping of the polygon
      return creases;
    }
    (List<Crease> cr, List<PolygonEdge> e) axialCreasesAndMarkers(Tree tree){         // create edges and creases that connect the circles
    List<Crease> creases = new List<Crease>();
    List<PolygonEdge> edges = new List<PolygonEdge>();
    PolygonEdge newEdge;
      for(int i=0; i<tree.circles.Count-1; i++){                               // add a crease and edge between all circles on the outside
        creases.Add(new Crease(tree.circles[i].getCenter(), tree.circles[i+1].getCenter(), axialCreaseColor));
        newEdge = new PolygonEdge(tree.circles[i].getCenter(), i, tree.circles[i+1].getCenter(), i+1);
        edges.Add(newEdge);
        if(tree.leafNodes[i].relatedNode != tree.leafNodes[i+1].relatedNode)
           addMarker(newEdge, tree.leafNodes[i], tree.leafNodes[i+1]);
      }
      creases.Add(new Crease(tree.circles[0].getCenter(), tree.circles.Last().getCenter(), axialCreaseColor)); //connect the last circle with an edge to the first one to close the polygon
      newEdge = new PolygonEdge(tree.circles.Last().getCenter(), tree.circles.Count-1, tree.circles[0].getCenter(), 0);
      edges.Add(newEdge);
      if(tree.leafNodes[0].relatedNode != tree.leafNodes.Last().relatedNode)
        addMarker(newEdge, tree.leafNodes.Last(), tree.leafNodes[0]);
      return (creases,edges);
    }

    void addMarker(PolygonEdge edge, LeafNode node1, LeafNode node2){
      if(node1.relatedNode != node2.relatedNode)
        addMarker(edge, node1.relatedNode, node2);
    }

    bool addMarker(PolygonEdge edge, InteriorNode inNode, LeafNode node2, InteriorNode lastChecked=null){
      if(inNode == node2.relatedNode){
        edge.setMarker(edge.p2-edge.vec*node2.size);
        return true;
      }
      foreach(var next in inNode.relatedInteriorNodes.Keys){
        if(next == lastChecked) continue;
        if(addMarker(edge, next, node2, inNode)){
          edge.setMarker(edge.markers.Last()-edge.vec*inNode.relatedInteriorNodes[next]);
          return true;
        }
      }
      return false;
    }

    public void sweep(Tree tree, List<Crease> creases, List<PolygonEdge> polygon, List<PolygonEdge> initialEdges){
      bool again = true;
      while(again){
        sweepEdges(polygon, sweepingLength);          // sweep every edge by sweepingLength
        updateVerticesandMarkers(polygon);            // update vertices of polygon
        addCreases(creases, polygon, initialEdges);   // add all new creases
        if (VISUAL) debugExport(tree,creases,"snapshot"+counter+".svg");counter++;  // take a visual snapshot
        for(int i=0; i<polygon.Count; i++){
          PolygonEdge edge = polygon[i];
          if(edge.getLength() < 2*sweepingLength){  //contraction event (next iteration we would have 0 length)
            polygon.Remove(edge);
          }
          if(polygon.Count<3){                        // rabit ear molecule, close in with triangular crease
            for(int z=0; z<polygon.Count-1; z++)
              if(polygon[z].getLength() > 2*sweepingLength)
                creases.Add(new Crease(polygon[z].p1, polygon[z].p2, creaseColor));
            return;
          }
          if(linedUp(polygon)) return;                  // colinear edges make no sense--polygon is death now stop sweeping this polygon
          if(polygon.Count > 3){                        // no contractions found, check for splitting events
            for(int j=i; j<polygon.Count; j++){         // compare to all other edges in the polygon
              if(i==0 && j==polygon.Count-1) continue;  // cannot split the first with the last edge
              PolygonEdge secondEdge = polygon[j];      // the other edge to check for splitting
              if(skipOddCases(polygon,edge,secondEdge)) continue; // check for odd cases
              if(isSplitEvent(tree, edge,secondEdge,inputEdges,polygon)){
                  again = false;
                  if (DEBUG) Console.WriteLine("split between " + edge.index1 + " and " + secondEdge.index1 + " with length: \r\n" + edge.p1.getDistance(secondEdge.p1));
                  tree.removeFromDistancesMatrix(edge,secondEdge);                        //avoid splitting same edges twice
                  creases.Add(new Crease(edge.p1, secondEdge.p1, internalCreaseColor));   // make the internal crease before splitting the actual polygon
                  (List<PolygonEdge> splitOffPoly, List<PolygonEdge> otherPoly) = splitOffPolygon(polygon, i, j);
                  sweep(tree, creases, splitOffPoly.ConvertAll(x => new PolygonEdge(x)), splitOffPoly); //recursively call the main algorithm to sweep the new spin-offs for cut off
                  sweep(tree, creases, otherPoly.ConvertAll(x => new PolygonEdge(x)), otherPoly); // and the same for the other side
              }
            }
          }
        }
      }
    }
    (List<PolygonEdge> a, List<PolygonEdge> b) splitOffPolygon(List<PolygonEdge> polygon, int indexFrom, int indexTo){
      PolygonEdge edge = polygon[indexFrom];
      PolygonEdge secondEdge = polygon[indexTo];
      List<PolygonEdge> splitOffPoly = new List<PolygonEdge>(), otherPoly = new List<PolygonEdge>();
      PolygonEdge splittingEdge = new PolygonEdge(secondEdge.p1, secondEdge.index1, edge.p1, edge.index1);  // define the splitting edge for the A side
      PolygonEdge splittingEdgeOther = new PolygonEdge(edge.p1, edge.index1, secondEdge.p1, secondEdge.index1); // same but reversed splitting edge for the B side
      for (int k =0; k<polygon.Count;k++){  //loop through all edges on the polygon
        PolygonEdge currentEdge = polygon[k];
        if (k < indexFrom){                         // copy edges from 0 to the split polygon
          otherPoly.Add(currentEdge);
          addMarkersToSplittingEdge(splittingEdgeOther, currentEdge,(indexFrom+polygon.Count-indexTo <3));
        }else if (k < indexTo) {                  // copy edges of i until j (the split-off polygon)
          splitOffPoly.Add(currentEdge);
          addMarkersToSplittingEdge(splittingEdge, currentEdge,(indexTo-indexFrom <3));
        }else{                              //copy all other edges (before the split) into the other polygon
          otherPoly.Add(currentEdge);
          addMarkersToSplittingEdge(splittingEdgeOther, currentEdge,(polygon.Count-indexTo <3));
        }
      }
      cloneMarkers(splittingEdge,splittingEdgeOther);
      splitOffPoly.Insert(splitOffPoly.Count,new PolygonEdge(splittingEdge)); //insert the splitting edge into the split off polygon
      otherPoly.Insert(indexFrom,new PolygonEdge(splittingEdgeOther));        // insert the copy of the splitting edge into the old polygon
      return (splitOffPoly, otherPoly);
    }
    bool isSplitEvent(Tree tree, PolygonEdge edge, PolygonEdge secondEdge, List<PolygonEdge> input, List<PolygonEdge> es){
      double equationSolution = Int64.MaxValue;
      if(secondEdge.vec == input[secondEdge.index1].vec && edge.vec == input[edge.index1].vec){
        equationSolution = solveEquation(edge, input[secondEdge.index1],input[secondEdge.index1].p1,secondEdge, secondEdge.p1, secondEdge.p1);
      }else{ //the according edge was already splitted so we try the other edge of this vertex
        PolygonEdge altSecondEdge = (secondEdge.index1!=0) ? es[secondEdge.index1-1] : es.Last();
        PolygonEdge altInputEdge = (secondEdge.index1!=0) ? inputEdges[secondEdge.index1-1] : inputEdges.Last();
        equationSolution = solveEquation(edge, altInputEdge,altInputEdge.p2,altSecondEdge, altSecondEdge.p2, secondEdge.p1);
      }
      return (equationSolution < tree.distances[edge.index1, secondEdge.index1]);
    }
    double solveEquation(PolygonEdge edge, PolygonEdge firstEdge, Point2D p1, PolygonEdge secondEdge, Point2D p2, Point2D p3){
      double AA_, CC_;
      Point2D A_, C_;
      C_ = Geometry.findIntersection(firstEdge.vec, p1, secondEdge.vec.getNormalRight(), p2);
      CC_ = p1.getDistance(C_);
      A_ = Geometry.findIntersection(inputEdges[edge.index1].vec, inputEdges[edge.index1].p1, edge.vec.getNormalRight(), edge.p1);
      AA_ = inputEdges[edge.index1].p1.getDistance(A_);
      return Math.Round(edge.p1.getDistance(p3) + AA_ + CC_, 2);
    }
    void sweepEdges(List<PolygonEdge> edges, double sweepingLength){
      foreach(var edge in edges)
        edge.parallelSweep(sweepingLength);
    }
    void addCreases(List<Crease> creases, List<PolygonEdge> edges, List<PolygonEdge> initialEdges){
      for(int l=0; l<edges.Count; l++){
          PolygonEdge edge = edges[l];
          for(int k=0; k<edge.markers.Count; k++){
            if(!(edge.markers[k] == null)){
              if(k>initialEdges[l].markers.Count-1)continue;
              creases.Add(new Crease(initialEdges[l].markers[k], edge.markers[k], riverColor));
              //creases.Add(newCrease); //TODO detect that a crease is colinear and remove the old one
              /*List<Crease> colinear = creases.FindAll(
              delegate(Crease x){
                return x.isColinearWith(newCrease);
              });
              Console.WriteLine("found {0} colinear creases", colinear.Count);

              foreach (var cr in colinear)
                creases.Remove(cr);*/
            }
          }
          creases.Add(new Crease(edge.p1, initialEdges[l].p1, creaseColor)); //TODO detect that a crease is colinear and remove the old one
      }
    }

    void addMarkersToSplittingEdge(PolygonEdge splittingEdge, PolygonEdge edge, bool condition){
      if (condition){
        Vector splitVector = new Vector(splittingEdge.p2, splittingEdge.p1).normalized();
        foreach(var marker in edge.markers){
          if(!(marker == null)){
            if(edge.p2 == splittingEdge.p1){
              double d = edge.p2.getDistance(marker);
              splittingEdge.setMarker(splittingEdge.p1+splitVector.getReverse()*d);
            }else{
              double d = edge.p1.getDistance(marker);
              splittingEdge.setMarker(splittingEdge.p2-splitVector.getReverse()*d);
            }
          }
        }
      }
    }
    //less exciting helper methods
    bool skipOddCases(List<PolygonEdge> p, PolygonEdge e1, PolygonEdge e2){
      return ((e2==null) ||
        (Math.Abs(e1.index1 - e2.index1) <= 1) ||
        (e1.index1 - e2.index1 == p.Count) ||
        (e1.vec == e2.vec));
    }
    void insertSplitEdge(int index, List<PolygonEdge> polygon, List<PolygonEdge> splitPolygon, PolygonEdge splitEdge){
      polygon.Insert(index,new PolygonEdge(splitEdge)); // insert a copy of the splitEdge into the polygon
      splitPolygon.Insert(index,new PolygonEdge(splitEdge));             // add the original splitEdge to the other polygon
    }
    void cloneMarkers (PolygonEdge edge1, PolygonEdge edge2){
      foreach(var marker in edge1.markers)
        edge2.setMarker(marker);
      foreach(var marker in edge2.markers)
        edge1.setMarker(marker);
    }
    void debugExport(Tree t, List<Crease> cr, string s){
      FileHandler f = new FileHandler(DEBUG, t.getPaperSizeX(), zoom);
      f.exportSVG(s, t, cr);
    }
    bool linedUp(List<PolygonEdge> edges){
      for(int i=1; i<edges.Count; i++)
        if(edges[i].vec != edges[i-1].vec && edges[i].vec != edges[i-1].vec.getReverse())
          return false;
      return true;
    }

    void updateVerticesandMarkers(List<PolygonEdge> edges){
      edges[0].updateVertices(edges.Last(), edges[1]);
      edges[0].updateMarkers();
      for(int i = 1; i<edges.Count-1; i++){
        edges[i].updateVertices(edges[i-1], edges[i+1]);
        edges[i].updateMarkers();
      }
      edges.Last().updateVertices(edges[edges.Count-2], edges[0]);
      edges.Last().updateMarkers();
      //return edges;
    }


}
}
