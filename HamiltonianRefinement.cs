using System.Collections.Generic;
using System;

namespace inClassHacking{

  class HamiltonianRefinement{

    List<DualgraphTriangle> triangles = new List<DualgraphTriangle>(); //all imported triangles
    Dictionary<DualgraphTriangle, bool> processed = new
      Dictionary<DualgraphTriangle, bool>();

    public void addTriangle(Triangle triangle){
      DualgraphTriangle dualgraphTriangle = new DualgraphTriangle(triangle);
      triangles.Add(dualgraphTriangle);
      processed.Add(dualgraphTriangle, false);
    }

    public void createDualGraph(){ //finding triangles next to each other in right order so we can "walk around" them in toStrip()
      for(int i = 0; i<triangles.Count; i++){
        findNeighbor(triangles[i]);
      }
    }
    private void isAdjacentTo(DualgraphTriangle t1, DualgraphTriangle t2){
      for (int j =0; j <=2; j++){
        for (int i = 0; i<=2; i++){
          if (t1.edges[j] == t2.edges[i]){
            t1.neighbor.sides[j] = t2;
            t2.neighbor.sides[i] = t1;
          }
        }
      }
    }
    public void findNeighbor(DualgraphTriangle thisTriangle){ //add triangle with correct edge in this Triangles neighbor attribute, if they share an edge

      for(int i = 1; i<triangles.Count; i++){
        DualgraphTriangle other = triangles[i];
        if(thisTriangle == other) continue; //don't check against itself
        isAdjacentTo(thisTriangle,other);
      }
    }

    public void toStrip(Strip strip){ //starts recursion, called in main()
      toStrip(strip, triangles[0], null);
    }

    void toStrip(Strip strip, DualgraphTriangle triangle, DualgraphTriangle triangleFrom){
      if (processed[triangle]) return;
      processed[triangle] = true;//set processed to true

      int startingIndex = triangle.getStartPoint(triangleFrom);
      if(startingIndex == DualgraphTriangle.UNDEF) return;

      for(int i = 0; i<6; i++){
        if((startingIndex+i)%6 == 1){
          if(triangle.neighbor.sides[1] != null){
            toStrip(strip, triangle.neighbor.sides[1], triangle);
          }
        }
        if((startingIndex+i)%6 == 3){
          if(triangle.neighbor.sides[0] != null){
            toStrip(strip, triangle.neighbor.sides[0], triangle);
          }
        }
        if((startingIndex+i)%6 == 5){
          if(triangle.neighbor.sides[2] != null){
            toStrip(strip, triangle.neighbor.sides[2], triangle);
          }
        }
        strip.addTriangle(triangle.triangulation[(startingIndex+i)%6]);
      }
    }
  }
}
