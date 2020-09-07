using System.Collections.Generic;
using System;

namespace inClassHacking{

  class HamiltonianRefinement{
    public List<Tuple<DualgraphTriangle, bool>> input = new List<Tuple<DualgraphTriangle, bool>>(); //tuples of Triangle and if its already part of the dualgraph 
    public List<Tuple<int, int>> dualGraph = new List<Tuple<int, int>>();

    public void addTriangle(Triangle triangle){
      DualgraphTriangle dualgraphTriangle = new DualgraphTriangle(triangle, input.Count);
      input.Add(new Tuple<DualgraphTriangle, bool>(dualgraphTriangle, false));
    }

    public void createDualGraph(){
      for(int i = 0; i<input.Count; i++){
        List<int> neighbors = findNeighbor(i);
        foreach(var neighbor in neighbors){
          dualGraph.Add(new Tuple<int, int>(i, neighbor));
        }
      }
    }

    List<int> findNeighbor(int thisIndex){ //returns index of Triangles in input-list that share an edge and add neighbor with correct edge 
      List<int> returnList = new List<int>();
      DualgraphTriangle thisTriangle = input[thisIndex].Item1;
     
      for(int i = 1; i<input.Count; i++){ //skip 0 as its the first node and dont has to be found as a neighbor
        if(i==thisIndex) continue;
        if(input[i].Item2) continue;
        DualgraphTriangle other = input[i].Item1;

        if(thisTriangle.a == other.a){
          if(thisTriangle.b == other.b){
            thisTriangle.neighbor.cSide = i;
            other.neighbor.cSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if (thisTriangle.b == other.c){
            thisTriangle.neighbor.cSide = i;
            other.neighbor.bSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.b){
            thisTriangle.neighbor.bSide = i;
            other.neighbor.cSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.c){
            thisTriangle.neighbor.bSide = i;
            other.neighbor.bSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }
        }
        if(thisTriangle.a == other.b){
          if(thisTriangle.b == other.a){
            thisTriangle.neighbor.cSide = i;
            other.neighbor.cSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.b == other.c){
            thisTriangle.neighbor.cSide = i;
            other.neighbor.aSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.a){
            thisTriangle.neighbor.bSide = i;
            other.neighbor.cSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.c){
            thisTriangle.neighbor.bSide = i;
            other.neighbor.aSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }
        }
        if(thisTriangle.a == other.c){
          if(thisTriangle.b == other.a){
            thisTriangle.neighbor.cSide = i;
            other.neighbor.bSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.b == other.b){
            thisTriangle.neighbor.cSide = i;
            other.neighbor.aSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.a){
            thisTriangle.neighbor.bSide = i;
            other.neighbor.bSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.b){
            thisTriangle.neighbor.bSide = i;
            other.neighbor.aSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }
        }
        if(thisTriangle.b == other.a){
          if(thisTriangle.c == other.b){
            thisTriangle.neighbor.aSide = i;
            other.neighbor.cSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.c){
            thisTriangle.neighbor.aSide = i;
            other.neighbor.bSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }
        }
        if(thisTriangle.b == other.b){
          if(thisTriangle.c == other.a){
            thisTriangle.neighbor.aSide = i;
            other.neighbor.cSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.c){
            thisTriangle.neighbor.aSide = i;
            other.neighbor.aSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }
        }
        if(thisTriangle.b == other.c){
          if(thisTriangle.c == other.a){
            thisTriangle.neighbor.aSide = i;
            other.neighbor.bSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }else if(thisTriangle.c == other.b){
            thisTriangle.neighbor.aSide = i;
            other.neighbor.aSide = thisIndex;
            returnList.Add(i);
            input[i] = new Tuple<DualgraphTriangle, bool>(other, true);
            continue;
          }
        }
      }
      return returnList;
    }

    void walkAround(Strip strip, DualgraphTriangle triangle, int indexFrom){ 
      int startingIndex = triangle.getStartPoint(indexFrom);
      for(int i = 0; i<6; i++){
        if((startingIndex+i)%6 == 1){
          if(triangle.neighbor.bSide != -1){
            walkAround(strip, input[triangle.neighbor.bSide].Item1, triangle.index);
          }
        }
        if((startingIndex+i)%6 == 3){
          if(triangle.neighbor.aSide != -1){
            walkAround(strip, input[triangle.neighbor.aSide].Item1, triangle.index);
          }
        }
        if((startingIndex+i)%6 == 5){
          if(triangle.neighbor.cSide != -1){
            walkAround(strip, input[triangle.neighbor.cSide].Item1, triangle.index);
          }
        }
        strip.addTriangle(triangle.triangulation[(startingIndex+i)%6]);
        
      }
    }
    
    public void walkAround(Strip strip){
      walkAround(strip, input[0].Item1, -1);
    }
  }
}