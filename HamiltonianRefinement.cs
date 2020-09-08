using System.Collections.Generic;
using System;

namespace inClassHacking{

  class HamiltonianRefinement{
    List<Tuple<DualgraphTriangle, bool>> input = new List<Tuple<DualgraphTriangle, bool>>(); //tuples of DualgraphTriangle and if its already part of the dualgraph 
    List<bool> done = new List<bool>();
    int count = 0; //debugging

    public void addTriangle(Triangle triangle){
      DualgraphTriangle dualgraphTriangle = new DualgraphTriangle(triangle, input.Count);
      input.Add(new Tuple<DualgraphTriangle, bool>(dualgraphTriangle, false));
      done.Add(false);
    }

    public int getNumberOfImportedTriangles(){
      return count;
    }

    public void createDualGraph(){
      for(int i = 0; i<input.Count; i++){
        findNeighbor(i);
      }
    }

    public void findNeighbor(int thisIndex){ //returns index of Triangles in input-list that share an edge and add neighbor with correct edge in this Triangles neighbor attribute
      List<int> returnList = new List<int>();
      DualgraphTriangle thisTriangle = input[thisIndex].Item1;
     
      for(int i = 1; i<input.Count; i++){ 
        if(i==thisIndex) continue;
        DualgraphTriangle other = input[i].Item1;
        if(other.neighbor.aSide == thisIndex || other.neighbor.bSide == thisIndex || other.neighbor.cSide == thisIndex) continue;
        if(thisTriangle.centerOfEdgeA == other.centerOfEdgeA){
          thisTriangle.neighbor.aSide = i;
          other.neighbor.aSide = thisIndex; 
          returnList.Add(i);
        } else if(thisTriangle.centerOfEdgeA == other.centerOfEdgeB){
          thisTriangle.neighbor.aSide = i;
          other.neighbor.bSide = thisIndex; 
          returnList.Add(i);
        } else if(thisTriangle.centerOfEdgeA == other.centerOfEdgeC){
          thisTriangle.neighbor.aSide = i;
          other.neighbor.cSide = thisIndex; 
          returnList.Add(i);
        }

        else if(thisTriangle.centerOfEdgeB == other.centerOfEdgeA){
          thisTriangle.neighbor.bSide = i;
          other.neighbor.aSide = thisIndex; 
          returnList.Add(i);
        } else if(thisTriangle.centerOfEdgeB == other.centerOfEdgeB){
          thisTriangle.neighbor.bSide = i;
          other.neighbor.bSide = thisIndex; 
          returnList.Add(i);
        } else if(thisTriangle.centerOfEdgeB == other.centerOfEdgeC){
          thisTriangle.neighbor.bSide = i;
          other.neighbor.cSide = thisIndex; 
          returnList.Add(i);
        }

        else if(thisTriangle.centerOfEdgeC == other.centerOfEdgeA){
          thisTriangle.neighbor.cSide = i;
          other.neighbor.aSide = thisIndex; 
          returnList.Add(i);
        } else if(thisTriangle.centerOfEdgeC == other.centerOfEdgeB){
          thisTriangle.neighbor.cSide = i;
          other.neighbor.bSide = thisIndex; 
          returnList.Add(i);
        }else if(thisTriangle.centerOfEdgeC == other.centerOfEdgeC){
          thisTriangle.neighbor.cSide = i;
          other.neighbor.cSide = thisIndex; 
          returnList.Add(i);
        } 
      }
    }

    public void toStrip(Strip strip){
      toStrip(strip, input[0].Item1, -1);
    }
    
    void toStrip(Strip strip, DualgraphTriangle triangle, int indexFrom){ 
      if (done[triangle.index]) return;
      done[triangle.index] = true;

      int startingIndex = triangle.getStartPoint(indexFrom);
      if(startingIndex == -1) return;

      for(int i = 0; i<6; i++){
        if((startingIndex+i)%6 == 1){
          if(triangle.neighbor.bSide != -1){
            toStrip(strip, input[triangle.neighbor.bSide].Item1, triangle.index);
          }
        }
        if((startingIndex+i)%6 == 3){
          if(triangle.neighbor.aSide != -1){
            toStrip(strip, input[triangle.neighbor.aSide].Item1, triangle.index);
          }
        }
        if((startingIndex+i)%6 == 5){
          if(triangle.neighbor.cSide != -1){
            toStrip(strip, input[triangle.neighbor.cSide].Item1, triangle.index);
          }
        }
        strip.addTriangle(triangle.triangulation[(startingIndex+i)%6]);
        count++;
      }
    }
  }
}