using System.Collections.Generic;
using System;

namespace inClassHacking{

  class HamiltonianRefinement{

    List<DualgraphNode> triangles = new List<DualgraphNode>(); //all imported triangles
    Dictionary<DualgraphNode, bool> processed = new Dictionary<DualgraphNode, bool>();
    public void addTriangle(Triangle triangle){
      DualgraphNode DualgraphNode = new DualgraphNode(triangle);
      triangles.Add(DualgraphNode);
      processed.Add(DualgraphNode, false);
    }
    public void triangulate(){
      foreach(DualgraphNode node in triangles){
        node.triangulate();
      }
    }

    public void createDualGraph(){ //finding triangles next to each other in right order so we can "walk around" them in toStrip()
      for(int i = 0; i<triangles.Count; i++){
        findNeighbors(triangles[i]);
      }
    }
    private void isAdjacentTo(DualgraphNode thisNode, DualgraphNode otherNode){
      for (int j =0; j <=2; j++){
        for (int i = 0; i<=2; i++){
          if (thisNode.edges[j] == otherNode.edges[i]){
            thisNode.neighbors.sides[j] = otherNode;
            otherNode.neighbors.sides[i] = thisNode;
          }
        }
      }
    }
    public void findNeighbors(DualgraphNode thisTriangle){ //add triangle with correct edge in this Triangles neighbor attribute, if they share an edge
      for(int i = 1; i<triangles.Count; i++){
        DualgraphNode other = triangles[i];
        if(thisTriangle == other) continue; //don't check against itself
        isAdjacentTo(thisTriangle,other);
      }
    }

    public void toStrip(Strip strip){ //starts recursion, called in main()
      toStrip(strip, triangles[0], null);
    }

    void toStrip(Strip strip, DualgraphNode triangle, DualgraphNode triangleFrom){
      if (processed[triangle]) return;
      processed[triangle] = true;//set processed to true

      int startingIndex = triangle.getStartPoint(triangleFrom);
      if(startingIndex == DualgraphNode.UNDEF) return;

      for(int i = 0; i<6; i++){
        if((startingIndex+i)%6 == 1){
          if(triangle.neighbors.sides[1] != null){
            toStrip(strip, triangle.neighbors.sides[1], triangle);
          }
        }
        if((startingIndex+i)%6 == 3){
          if(triangle.neighbors.sides[0] != null){
            toStrip(strip, triangle.neighbors.sides[0], triangle);
          }
        }
        if((startingIndex+i)%6 == 5){
          if(triangle.neighbors.sides[2] != null){
            toStrip(strip, triangle.neighbors.sides[2], triangle);
          }
        }
        strip.addTriangle(triangle.triangulation[(startingIndex+i)%6]);
      }
    }
  }
}
