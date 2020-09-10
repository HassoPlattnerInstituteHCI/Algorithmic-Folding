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

    public void findNeighbor(DualgraphTriangle thisTriangle){ //add triangle with correct edge in this Triangles neighbor attribute, if they share an edge
     
      for(int i = 1; i<triangles.Count; i++){ 
        DualgraphTriangle other = triangles[i];
        if(thisTriangle == other) continue; //don't check against itself

        if(thisTriangle.centerOfEdgeA == other.centerOfEdgeA){
          thisTriangle.neighbor.aSide = other;
          other.neighbor.aSide = thisTriangle; 
        } else if(thisTriangle.centerOfEdgeA == other.centerOfEdgeB){
          thisTriangle.neighbor.aSide = other;
          other.neighbor.bSide = thisTriangle; 
        } else if(thisTriangle.centerOfEdgeA == other.centerOfEdgeC){
          thisTriangle.neighbor.aSide =other;
          other.neighbor.cSide = thisTriangle; 
        }

        else if(thisTriangle.centerOfEdgeB == other.centerOfEdgeA){
          thisTriangle.neighbor.bSide = other;
          other.neighbor.aSide = thisTriangle; 
        } else if(thisTriangle.centerOfEdgeB == other.centerOfEdgeB){
          thisTriangle.neighbor.bSide = other;
          other.neighbor.bSide = thisTriangle; 
        } else if(thisTriangle.centerOfEdgeB == other.centerOfEdgeC){
          thisTriangle.neighbor.bSide = other;
          other.neighbor.cSide = thisTriangle; 
        }

        else if(thisTriangle.centerOfEdgeC == other.centerOfEdgeA){
          thisTriangle.neighbor.cSide =other;
          other.neighbor.aSide = thisTriangle; 
        } else if(thisTriangle.centerOfEdgeC == other.centerOfEdgeB){
          thisTriangle.neighbor.cSide =other;
          other.neighbor.bSide = thisTriangle; 
        }else if(thisTriangle.centerOfEdgeC == other.centerOfEdgeC){
          thisTriangle.neighbor.cSide =other;
          other.neighbor.cSide = thisTriangle; 
        } 
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
          if(triangle.neighbor.bSide != null){
            toStrip(strip, triangle.neighbor.bSide, triangle);
          }
        }
        if((startingIndex+i)%6 == 3){
          if(triangle.neighbor.aSide != null){
            toStrip(strip, triangle.neighbor.aSide, triangle);
          }
        }
        if((startingIndex+i)%6 == 5){
          if(triangle.neighbor.cSide != null){
            toStrip(strip, triangle.neighbor.cSide, triangle);
          }
        }
        strip.addTriangle(triangle.triangulation[(startingIndex+i)%6]);
      }
    }
  }
}