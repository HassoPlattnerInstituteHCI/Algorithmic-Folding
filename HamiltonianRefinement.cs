using System.Collections.Generic;
using System;

namespace inClassHacking{

  class HamiltonianRefinement{
    List<Tuple<DualgraphTriangle, bool>> input = new List<Tuple<DualgraphTriangle, bool>>(); //tuples of DualgraphTriangle and if its processed by toStrip()

    public void addTriangle(Triangle triangle){
      DualgraphTriangle dualgraphTriangle = new DualgraphTriangle(triangle, input.Count);
      input.Add(new Tuple<DualgraphTriangle, bool>(dualgraphTriangle, false));
    }

    public void createDualGraph(){
      for(int i = 0; i<input.Count; i++){
        findNeighbor(i);
      }
    }

    public void findNeighbor(int thisIndex){ //add triangle with correct edge in this Triangles neighbor attribute, if they share an edge
      List<int> returnList = new List<int>();
      DualgraphTriangle thisTriangle = input[thisIndex].Item1;
     
      for(int i = 1; i<input.Count; i++){ 
        if(i==thisIndex) continue;
        DualgraphTriangle other = input[i].Item1;
        
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

    public void toStrip(Strip strip){ //starting point, called in main()
      toStrip(strip, input[0].Item1, -1);
    }
    
    void toStrip(Strip strip, DualgraphTriangle triangle, int indexFrom){ 
      if (input[triangle.index].Item2) return;
      input[triangle.index] = new Tuple<DualgraphTriangle, bool>(input[triangle.index].Item1, true);

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
      }
    }
  }
}