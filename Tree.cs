using System;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking{

  public class Node{
    double undef = -1;
    public bool middle; //not "mirrored" for symmetrie
    public int index;
    public Circle circle;

    public Node(int index, Tree tree, bool middle){
      this.index = index;
      this.middle = middle;
      tree.addNode(this);
    }

    public Node(){}
  }

  public class LeafNode: Node{
    public InteriorNode relatedNode;
    public double size;

    public LeafNode(int index, double size, InteriorNode relatedNode, Tree tree, bool middle = false) : base(index, tree, middle) {
      this.relatedNode = relatedNode;
      this.size = size;
      relatedNode.addLeafNode(this);
    }
    public LeafNode(LeafNode l){
      this.relatedNode = l.relatedNode;
      this.size = l.size;
    }

    public double getSize(){ return size;}

    public LeafNode getCenterNeighbor(){
      foreach(var leafNodePair in relatedNode.relatedLeafNodes){
        if(leafNodePair.Value.middle){
          return leafNodePair.Value;
        }
      }
      return null;
    }

    public double getTreeDistanceTo(LeafNode other){
      if(other == this) return 0;
      return this.size+this.relatedNode.getTreeDistanceTo(other);
    }

    // public double getMarkerDistances(LeafNode other, List<double> distances){
    //   if(other.index == this.index) return distances;
    //   distances.Add(this.size);
    //   distances.Add(this.relatedNode.getMarkerDistances(other, distances));
    //   return distances;
    // }
  }

  public class InteriorNode: Node{
    public Dictionary<int, LeafNode> relatedLeafNodes = new Dictionary<int, LeafNode>();
    public Dictionary<InteriorNode, double> relatedInteriorNodes = new Dictionary< InteriorNode, double>();

    public InteriorNode(int index, Tree tree) : base(index, tree, true){}

    public void addLeafNode(LeafNode node){
      this.relatedLeafNodes[node.index] = node;
    }

    public void addInteriorNode(InteriorNode node, double distance){
      this.relatedInteriorNodes[node] = distance;
      node.relatedInteriorNodes[this] = distance;
    }

    public double getTreeDistanceTo(LeafNode other, InteriorNode lastChecked=null){
      foreach(var leafNode in this.relatedLeafNodes.Values){
        if(other == leafNode) return leafNode.size;
      }
      foreach(var interiorNode in this.relatedInteriorNodes.Keys){
        if(interiorNode == lastChecked) continue;
        double d = interiorNode.getTreeDistanceTo(other, this);
        if(d!=-1) return d+this.relatedInteriorNodes[interiorNode];
      }
      return -1;
    } 

    // public double getMarkerDistances(LeafNode other, List<double> distances){
    //   if(this == other.relatedNode){
    //      return distances;
    //   }
    //   distances.Add(this.size);
    //   distances.Add(this.relatedNode.getMarkerDistances(other, distances));
    //   return distances;
    // }
  }

    public class Tree{
      public List<Node> treeNodes = new List<Node>();

      public double drawingOffsetX=0;
      public double drawingOffsetY=0;
      public double maxY=0;

      public void addNode(Node node){
        treeNodes.Add(node);
      }

      public List<LeafNode> calculateCirclePositioning(){

        List<LeafNode> ret = new List<LeafNode>();
        Point2D startPosition = new Point2D(0, 0);

        List<LeafNode> middleNodes = new List<LeafNode>();
        List<LeafNode> outerNodes = new List<LeafNode>();
        foreach(var node in treeNodes){
          if(node.GetType() == typeof(LeafNode)){
            LeafNode Lnode = (LeafNode) node;
            if(node.middle){
              middleNodes.Add(Lnode);
            } else{
              outerNodes.Add(Lnode);
            }
          }
        }

        Point2D thisCircleCenter;
        Circle outerNodesCircle;

        
        if(outerNodes[0].getCenterNeighbor() != null){

          // for(int i=middleNodes.Count-1; i>=0; i--){
          //     if (i!=middleNodes.Count-1){
          //       startPosition.y += middleNodes[i].getTreeDistanceTo(middleNodes[i+1])-middleNodes[i].size-middleNodes[i+1].size;
          //     }
            startPosition.y += middleNodes[0].size;
            middleNodes[0].circle = new Circle(new Point2D(startPosition), middleNodes[0].size);
            ret.Add(middleNodes[0]);
            startPosition.y += middleNodes[0].size;
          // }
          LeafNode neighbor = outerNodes[0].getCenterNeighbor();
          thisCircleCenter = new Point2D(neighbor.circle.getCenter());
          thisCircleCenter.x -= (neighbor.size+outerNodes[0].size);
          if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;
          outerNodesCircle = new Circle(thisCircleCenter, outerNodes[0].size);
          outerNodes[0].circle = outerNodesCircle;
          ret.Add(outerNodes[0]);
        }
        else{ //NEEDS TO BE TESTED
          thisCircleCenter = new Point2D(startPosition);
          thisCircleCenter.x -= (outerNodes[0].size);
          if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;
          outerNodesCircle = new Circle(thisCircleCenter, outerNodes[0].size);
          outerNodes[0].circle = outerNodesCircle;
          ret.Add(outerNodes[0]);
        }
          // for(int i=outerNodes.Count-1; i>0; i--){
            for(int i=1; i<outerNodes.Count; i++){
              thisCircleCenter = new Point2D(outerNodes[i-1].circle.getCenter());
              thisCircleCenter.y += outerNodes[i].getTreeDistanceTo(outerNodes[i-1]);
              if(thisCircleCenter.y < drawingOffsetY) drawingOffsetY = thisCircleCenter.y;

              if(outerNodes[i].getCenterNeighbor() == null){
                thisCircleCenter.x = -outerNodes[i].size;
              }else{
                thisCircleCenter.x = -outerNodes[i].size - outerNodes[i].getCenterNeighbor().size;
              }
              if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;

              outerNodesCircle = new Circle(thisCircleCenter, outerNodes[i].size);
              outerNodes[i].circle = outerNodesCircle;
              ret.Add(outerNodes[i]);
            }

          if(outerNodes.Last().getCenterNeighbor() != null){
            Circle middleCircle;
            Point2D lowerPosition = new Point2D(outerNodes.Last().circle.getCenter());
            lowerPosition.x = 0;
            for(int i=middleNodes.Count-1; i>0; i--){
              middleCircle = new Circle(new Point2D(lowerPosition), middleNodes[i].size);
              middleNodes[i].circle = middleCircle;
              ret.Add(middleNodes[i]);

              if(i!=1){
                Console.WriteLine("change lower y");
                Console.WriteLine(lowerPosition);
                lowerPosition.y -= middleNodes[i].getTreeDistanceTo(middleNodes[i-1]);
                Console.WriteLine(lowerPosition);
              }

            }
          }
        // }

        drawingOffsetX = -drawingOffsetX+2;
        drawingOffsetY = -drawingOffsetY+2;
        for(int i=0; i<ret.Count; i++){
          ret[i].circle.setCenter(new Point2D(ret[i].circle.getCenter().x+drawingOffsetX, ret[i].circle.getCenter().y+drawingOffsetY));
          if(ret[i].circle.getCenter().y > maxY) maxY = ret[i].circle.getCenter().y;
        }
        return ret;
      }

      public double getPaperSizeX(){
        return 2*drawingOffsetX;
      }

      public double getPaperSizeY(){
        return maxY;
      }

    }
}