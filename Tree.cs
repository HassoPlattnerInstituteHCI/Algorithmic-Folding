using System;
using System.Collections.Generic;

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

        List<Circle> circles = new List<Circle>();
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
        
        if(outerNodes[0].getCenterNeighbor() != null){

          for(int i=middleNodes.Count-1; i>=0; i--){
              if (i!=middleNodes.Count-1){
                startPosition.y += middleNodes[i].getTreeDistanceTo(middleNodes[i+1])-middleNodes[i].size-middleNodes[i+1].size;
              }
            startPosition.y += middleNodes[i].size;
            middleNodes[i].circle = new Circle(new Point2D(startPosition), middleNodes[i].size);
            circles.Add(middleNodes[i].circle);
            ret.Add(middleNodes[i]);
            startPosition.y += middleNodes[i].size;
          }
          LeafNode neighbor = outerNodes[0].getCenterNeighbor();
          Point2D thisCircleCenter = new Point2D(neighbor.circle.getCenter());
          thisCircleCenter.x -= (neighbor.size+outerNodes[0].size);
          if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;
          Circle outerNodesCircle = new Circle(thisCircleCenter, outerNodes[0].size);
          circles.Add(outerNodesCircle);
          outerNodes[0].circle = outerNodesCircle;
          ret.Add(outerNodes[0]);
          
          // for(int i=outerNodes.Count-1; i>0; i--){
            for(int i=1; i<outerNodes.Count; i++){
            thisCircleCenter = new Point2D(outerNodes[i-1].circle.getCenter());
            thisCircleCenter.y -= outerNodes[i].getTreeDistanceTo(outerNodes[i-1]);
            if(thisCircleCenter.y < drawingOffsetY) drawingOffsetY = thisCircleCenter.y;
            outerNodesCircle = new Circle(thisCircleCenter, outerNodes[i].size);
            circles.Add(outerNodesCircle);
            outerNodes[i].circle = outerNodesCircle;
            ret.Add(outerNodes[i]);
          }
        }

        drawingOffsetX = -drawingOffsetX+2;
        drawingOffsetY = -drawingOffsetY+2;
        for(int i=0; i<circles.Count; i++){
          circles[i].setCenter(new Point2D(circles[i].getCenter().x+drawingOffsetX, circles[i].getCenter().y+drawingOffsetY));
          if(circles[i].getCenter().y > maxY) maxY = circles[i].getCenter().y;
        }
        return ret;
      }

      public double getPaperSizeX(){
        // if(drawingOffsetX>drawingOffsetY && drawingOffsetX>maxY) return 2*drawingOffsetX;
        // if(drawingOffsetY>maxY) return 2*drawingOffsetY;
        // return maxY;
        return 2*drawingOffsetX;
      }

      public double getPaperSizeY(){
        return maxY;
      }

    }
}