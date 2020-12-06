using System;
using System.Linq;
using System.Collections.Generic;
using SparseCollections;

namespace inClassHacking{

  public class LangsAlgorithm{
    public bool DEBUG;
    public bool VISUAL;
    public int zoom;

    int counter=0;
    List<PolygonEdge> inputEdges = new List<PolygonEdge>();

    public LangsAlgorithm(Tree tree, bool debug=false, bool visual=false, int zoomFactor=90){
      this.DEBUG=debug;
      this.VISUAL=visual;
      this.zoom = zoomFactor;
    }
    public List<Crease> sweepingProcess(Tree tree, double sweepingLength){
      (List<Crease> creases, List<PolygonEdge> edges)= constructPolygon(tree);  // create creases and edges between the circles
      inputEdges = edges.ConvertAll(x => new PolygonEdge(x));                         // copy edges to inputEdges (without markers)
      sweep(tree, creases, edges, inputEdges, sweepingLength);                        // the actual sweeping of the polygon
      return creases;
    }
    (List<Crease> cr, List<PolygonEdge> e) constructPolygon(Tree tree){         // create edges and creases that connect the circles
    List<Crease> creases = new List<Crease>();
    List<PolygonEdge> edges = new List<PolygonEdge>();
    LeafNode node = tree.getLeafNodes().First();
    int i=0;
    do {
      creases.Add(new Crease(node.circle.getCenter(), node.circle.next.getCenter(), Defaults.axialCreaseColor));
      PolygonEdge newEdge = new PolygonEdge(node, node.circle.next.node, i);
      edges.Add(newEdge);
      if(newEdge.n1.relatedNode != newEdge.n2.relatedNode)
         addMarker(newEdge, newEdge.n1, newEdge.n2);
      node = node.circle.next.node;i++;
    }
    while (node != tree.getLeafNodes().First());
    setNeighbors(edges);
    return (creases,edges);
    }
    void setNeighbors(List<PolygonEdge> polygon){
      polygon.First().left = polygon.Last();
      polygon.First().right = polygon[1];
      for (int i=1;i<polygon.Count-1;i++){
        polygon[i].left = polygon[i-1];
        polygon[i].right = polygon[i+1];
      }
      polygon.Last().left = polygon[polygon.Count-2];
      polygon.Last().right = polygon.First();
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

    public void sweep(Tree tree, List<Crease> creases, List<PolygonEdge> polygon, List<PolygonEdge> initialEdges, double sweepingLength){
      while(true){
        shrinkPolygon(polygon.First(), sweepingLength);                      // sweep every edge by sweepingLength
        addCreases(creases, polygon, initialEdges);                          // add all new creases
        if (VISUAL) debugExport(tree,creases,"snapshot"+counter+".svg");counter++;
        for(int i=0; i<polygon.Count; i++){                               // iterate over all edges in the polygon
          PolygonEdge edge = polygon[i];                                  // take the current edge
          if(edge.getLength() < 2*sweepingLength)                         // contraction event (next iteration we would have 0 length)
            { polygon.Remove(edge); removeEdge(edge); }                   // take the edge out of the polygon
          if(polygon.Count< 3){                                            // rabit ear molecule, close in with triangular crease
            rabitear(creases,polygon,sweepingLength); return;}
          if(polygon.Count > 3){                                          // no contractions found, check for splitting events
            for(int j=i; j<polygon.Count; j++){                           // compare to all other edges in the polygon
              PolygonEdge secondEdge = polygon[j];                        // the other edge to check for splitting
              if(skipOddCases(polygon,edge,secondEdge)) continue;     // check for odd cases
              if(isSplitEvent(tree, edge,secondEdge,inputEdges,polygon)){ // was inputedges
                  if (DEBUG) Console.WriteLine("split between " + edge.index + " and " + secondEdge.index + " with length: \r\n" + edge.p1.getDistance(secondEdge.p1));
                  creases.Add(new Crease(edge.p1, secondEdge.p1, Defaults.internalCreaseColor));   // make the internal crease before splitting the actual polygon
                  (List<PolygonEdge> splitOffPoly, List<PolygonEdge> otherPoly) = splitOffPolygon(polygon, i, j);
                  sweep(tree, creases, splitOffPoly,splitOffPoly.ConvertAll(x => new PolygonEdge(x)), sweepingLength); //recursively call the main algorithm to sweep the new spin-offs for cut off
                  sweep(tree, creases, otherPoly, otherPoly.ConvertAll(x => new PolygonEdge(x)), sweepingLength); // and the same for the other side
                  return;
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
      PolygonEdge splittingEdge = new PolygonEdge(secondEdge.p1, secondEdge, edge.p1);  // define the splitting edge for the A side
      PolygonEdge splittingEdgeOther = new PolygonEdge(edge.p1, edge, secondEdge.p1); // same but reversed splitting edge for the B side
      for (int k =0; k<polygon.Count;k++){                  //loop through all edges on the polygon
        PolygonEdge currentEdge = polygon[k];               // the currently selected edge
        if (k < indexFrom){                                 // copy edges from 0 to the split polygon
          addMarkersToSplittingEdge(splittingEdgeOther, currentEdge,(indexFrom+polygon.Count-indexTo <3));
          otherPoly.Add(currentEdge);                       // assign the selected edge the other polygon
        }else if (k < indexTo) {                            // copy edges of i until j (the split-off polygon)
          addMarkersToSplittingEdge(splittingEdge, currentEdge,(indexTo-indexFrom <3));
          splitOffPoly.Add(currentEdge);                    // assign the selected edge to the split-off polygon
        }else{                                              //copy all other edges (before the split) into the other polygon as well
          addMarkersToSplittingEdge(splittingEdgeOther, currentEdge,(polygon.Count-indexTo <3));
          otherPoly.Add(currentEdge);                       // assign the selected edge to the other polgon
        }
      }
      setNeighbors(splitOffPoly);
      setNeighbors(otherPoly);
      cloneMarkers(splittingEdge,splittingEdgeOther);

      insertEdge(splitOffPoly.Last(), splittingEdge);
      splitOffPoly.Insert(splitOffPoly.Count,splittingEdge);

      insertEdge(otherPoly[indexFrom].left,splittingEdgeOther);
      otherPoly.Insert(indexFrom,splittingEdgeOther);
      return (splitOffPoly, otherPoly);
    }
    void displayPolygon(List<PolygonEdge> polygon){
      foreach(var p in polygon) Console.WriteLine("edge index {0}<-{1}->{2}",p.left.index,p.index,p.right.index);
    }
    void displayPolygon(PolygonEdge edge){
      int i = edge.index;
      do {
        Console.WriteLine("edge index {0}<-{1}->{2}",edge.left.index,edge.index,edge.right.index);
        edge = edge.right;
      } while (edge.index !=i);
    }
    void appendEdge(PolygonEdge last, PolygonEdge p, PolygonEdge first){
      last.right = p;
      p.left = last;
      p.right = first;
      first.left = p;
    }
    void removeEdge(PolygonEdge p){
      p.right.left = p.left;
      p.left.right = p.right;
    }
    void insertEdge(PolygonEdge p, PolygonEdge n){
      n.right = p.right;
      p.right = n;
      n.left = p;
      n.right.left = n;
    }
    void verifyList(List<PolygonEdge> l){
      PolygonEdge e = l.First();
      int c=0;
      do {
        c++;
        e=e.right;
      }while(e!=l.First());
        Console.WriteLine(((l.Count != c))?"something's fishy: {0}":"all good", c-l.Count);
    }
    void rabitear(List<Crease> creases, List<PolygonEdge> polygon,double sweepingLength){
      foreach (var e in polygon)
        if(e.getLength() > 2*sweepingLength)
          insertOrExtendCrease(creases,new Crease(e.p1, e.p2, Defaults.creaseColor));
    }
    bool isSplitEvent(Tree tree, PolygonEdge edge, PolygonEdge secondEdge, List<PolygonEdge> input, List<PolygonEdge> poly){
      double equationSolution = Int64.MaxValue;
      if(secondEdge.vec == input.Find(x => x.index == secondEdge.index).vec && edge.vec == input.Find(x=>x.index==edge.index).vec){ //vectors are still the same as originally
        equationSolution = solveEquation(poly,input,edge,secondEdge);
      }else{                                                                       //the according edge was already splitted so we try the other edge of this vertex
        equationSolution = solveEquation(poly,input,edge,secondEdge,input.Find(x=> x.index == secondEdge.left.index),secondEdge.left);
        }
      return (equationSolution <= edge.n1.getTreeDistanceTo(secondEdge.n1));//tree.distances[edge.index, secondEdge.index]); // if the distance on paper is smaller than the distance in the tree we need to split
    }
    double solveEquation(List<PolygonEdge> poly, List<PolygonEdge> input, PolygonEdge a_, PolygonEdge c_, PolygonEdge b = null, PolygonEdge b_ = null){
      double AA_, CC_;
      Point2D A,C,A_, C_;
      PolygonEdge a, c;
      a = input.Find(x => x.index == a_.index);                                 // find reference edge for a' in the original polygon
      c = input.Find(x => x.index == c_.index);                                 // find reference edge for c' in the original polygon
      A = a.p1;                                                                 // A and C are the opposite corners in the original polygon
      C = c.p1;
      A_ = Geometry.findIntersection(a.vec, A, a_.vec.perpendicular(),a_.p1);   // projection of the corner of a_ on the original edge a
      AA_ = A.getDistance(A_);                                                  // distance between the projected point A_ and A
      C_ =  (b!=null)?Geometry.findIntersection(b.vec,C, b_.vec.perpendicular(),b_.p2):Geometry.findIntersection(c.vec,C, c_.vec.perpendicular(),c_.p1); // similar projection on either c or b (if defined)
      CC_ = C.getDistance(C_);                                                  // distance between the projected point C_ and C
      return a_.p1.getDistance(c_.p1) + AA_ + CC_;                              // the distance we need to check for splitting
    }
    void addCreases(List<Crease> creases, List<PolygonEdge> polygon, List<PolygonEdge> initialEdges){
      for(int l=0; l<polygon.Count; l++){
          PolygonEdge edge = polygon[l];
          for(int k=0; k<edge.markers.Count; k++)
            if(!(edge.markers[k] == null))
              insertOrExtendCrease(creases, new Crease(initialEdges[l].markers[k], edge.markers[k], Defaults.riverColor));
          insertOrExtendCrease(creases,new Crease(edge.p1, initialEdges[l].p1, Defaults.creaseColor));
      }
    }
    void insertOrExtendCrease(List<Crease> creases, Crease c){
      List<Crease> colinear = creases.FindAll(delegate(Crease x){return x.isColinearWith(c);}); // find all colinear creases
      if (colinear.Count == 0)                  // there was no colinear crease yet, we add c to the crease list
        creases.Add(c);
      else{
        foreach (var crease in colinear){       // this should just be a single colinear crease--we stretch it
          Point2D p2 = (crease.p1 == c.p1 || crease.p2 == c.p1)?c.p2:new Point2D(crease.p2);
          Point2D p1 = (crease.p1 == c.p2 || crease.p2 == c.p2)?c.p1:new Point2D(crease.p1);
          crease.p1 = p1;
          crease.p2 = p2;
        }
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
    bool skipOddCases(List<PolygonEdge> p, PolygonEdge e1, PolygonEdge e2){
      return ((e2==null) ||
        (Math.Abs(e1.index - e2.index) <= 1) ||
        (e1.index - e2.index == p.Count) ||
        (e1.vec == e2.vec)||
        (p.First()==e1 && p.Last()==e2));
    }

    void cloneMarkers (PolygonEdge edge1, PolygonEdge edge2){
      List<Point2D> temp = new List<Point2D>(edge2.markers);
      foreach(var marker in edge1.markers)
        edge2.setMarker(marker);
      foreach(var marker in temp)
        edge1.setMarker(marker);
    }
    void debugExport(Tree t, List<Crease> cr, string s){
      FileHandler f = new FileHandler(DEBUG, t.getPaperSize(), zoom);
      f.exportSVG(s, new List<Tree>(){t}, new List<List<Crease>>(){cr});
    }
    void shrinkPolygon(PolygonEdge current, double sweepingLength){
      int index = current.index;
      do{
        current.parallelSweep(sweepingLength);                            // sweep the edges parallel inwards
        current = current.right;
      }
      while(current.index != index);

      do{
        current.removeOverlap(current.left,current.right,sweepingLength);  //remove overlapping edges
        current = current.right;
      }while(current.index != index);
    }
  }
}
