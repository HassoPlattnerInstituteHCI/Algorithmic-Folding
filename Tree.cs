using System;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking{

  public class Node{
    //double undef = -1;
    public bool middle; //not "mirrored" for symmetry
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

    public LeafNode():base(){}

    public double getSize(){ return size;}

    public LeafNode getCenterNeighbor(){
      foreach(var leafNodePair in relatedNode.relatedLeafNodes){
        if(leafNodePair.Value.middle){
          return leafNodePair.Value;
        }
      }
      return null;
    }

    public double getTreeDistanceTo(LeafNode other){ //recursively walks the tree until other node is reached
      if(other == this) return 0;
      return this.size+this.relatedNode.getTreeDistanceTo(other);
    }
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
  }



    public class Tree{
      public bool DEBUG;
      public List<Node> treeNodes = new List<Node>();
      public List<LeafNode> leafNodes = new List<LeafNode>();
      public List<Circle> circles = new List<Circle>();
      public double [,] distances;
      public double drawingOffsetX=0;
      public double drawingOffsetY=0;
      public double maxY=0;
      public Tree(bool debug = false) {
        if(debug)
         this.DEBUG=debug;
      }
      public void addNode(Node node){
        treeNodes.Add(node);
      }
      public void setLeafNodes(){
        List<LeafNode> n = new List<LeafNode>();
        foreach(var circle in circles){
          n.Add(circle.node);
        }
        leafNodes = n;
      }
      public void removeFromDistancesMatrix(PolygonEdge fromEdge, PolygonEdge toEdge){  //remove distances from matrix
        distances[fromEdge.index1, toEdge.index1] = -1;
        distances[toEdge.index1, fromEdge.index1] = -1;
      }
      public void calculateTreeDistances(){                                             // builds a matrix of all distances in the tree
          double[,] d = new double[leafNodes.Count, leafNodes.Count];
          for(int i=0; i<leafNodes.Count; i++)
            for(int j=0; j<leafNodes.Count; j++)
              d[i, j] = leafNodes[i].getTreeDistanceTo(leafNodes[j]);
          distances = d;
      }
      public List<Circle> calculateCirclePositioning(){

        List<Circle> ret = new List<Circle>();
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
        Circle outerNodesCircle, retCircle;

        if(outerNodes[0].getCenterNeighbor() != null){

          // for(int i=middleNodes.Count-1; i>=0; i--){
          //     if (i!=middleNodes.Count-1){
          //       startPosition.y += middleNodes[i].getTreeDistanceTo(middleNodes[i+1])-middleNodes[i].size-middleNodes[i+1].size;
          //     }
            startPosition.y += middleNodes[0].size;
            retCircle = new Circle(new Point2D(startPosition), middleNodes[0].size);
            retCircle.node = middleNodes[0];
            ret.Add(retCircle);
            startPosition.y += middleNodes[0].size;
          // }

          LeafNode neighbor = outerNodes[0].getCenterNeighbor();
          thisCircleCenter = new Point2D(ret[0].getCenter());
          thisCircleCenter.x -= (neighbor.size+outerNodes[0].size);
          if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;
          outerNodesCircle = new Circle(thisCircleCenter, outerNodes[0].size);
          outerNodesCircle.node = outerNodes[0];
          ret.Add(outerNodesCircle);
        }
        else{
          thisCircleCenter = new Point2D(startPosition);
          thisCircleCenter.x -= (outerNodes[0].size);
          if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;
          outerNodesCircle = new Circle(thisCircleCenter, outerNodes[0].size);
          outerNodesCircle.node = outerNodes[0];
          ret.Add(outerNodesCircle);
        }
          // for(int i=outerNodes.Count-1; i>0; i--){
            for(int i=1; i<outerNodes.Count; i++){
              thisCircleCenter = new Point2D(ret.Last().getCenter());
              thisCircleCenter.y += outerNodes[i].getTreeDistanceTo(outerNodes[i-1]);
              if(thisCircleCenter.y < drawingOffsetY) drawingOffsetY = thisCircleCenter.y;

              if(outerNodes[i].getCenterNeighbor() == null){
                thisCircleCenter.x = -outerNodes[i].size;
              }else{
                thisCircleCenter.x = -outerNodes[i].size - outerNodes[i].getCenterNeighbor().size;
              }
              if(thisCircleCenter.x < drawingOffsetX) drawingOffsetX = thisCircleCenter.x;

              outerNodesCircle = new Circle(thisCircleCenter, outerNodes[i].size);
              outerNodesCircle.node = outerNodes[i];
              ret.Add(outerNodesCircle);
            }
          if(outerNodes.Last().getCenterNeighbor() != null){
            Circle middleCircle;
            Point2D lowerPosition = new Point2D(ret.Last().getCenter());
            lowerPosition.x = 0;
            for(int i=middleNodes.Count-1; i>0; i--){
              middleCircle = new Circle(new Point2D(lowerPosition), middleNodes[i].size);
              middleCircle.node = middleNodes[i];
              ret.Add(middleCircle);

              if(i!=1){
                if (DEBUG){Console.WriteLine("change lower y");
                Console.WriteLine(lowerPosition);}
                lowerPosition.y -= middleNodes[i].getTreeDistanceTo(middleNodes[i-1]);
                if (DEBUG)
                  Console.WriteLine(lowerPosition);
              }

            }
          }
        // }
        drawingOffsetX = -drawingOffsetX+2;
        drawingOffsetY = -drawingOffsetY+2;
        for(int i=0; i<ret.Count; i++){
          ret[i].setCenter(new Point2D(ret[i].getCenter().x+drawingOffsetX, ret[i].getCenter().y+drawingOffsetY));
          if(ret[i].getCenter().y > maxY) maxY = ret[i].getCenter().y;
        }
        return ret;
      }

      public double getPaperSizeX(){
        if (DEBUG)
          Console.WriteLine("papersize: " + drawingOffsetX);
        return 2*this.drawingOffsetX;
      }

      public double getPaperSizeY(){
        return 2*this.drawingOffsetY;
      }

      public List<Circle> exampleBeetleCircles(){

        List<LeafNode> nodes = new List<LeafNode>();
        // List<InteriorNode> inNodes = new List<InteriorNode>();
        // int i=0;

        foreach(var n in this.treeNodes){
          if(n.GetType() == typeof(LeafNode)){
              LeafNode Lnode = (LeafNode) n;
              // i++;
              nodes.Add(Lnode);
              Lnode.circle = new Circle(new Point2D(0, 0), 0);
            }else{
              // InteriorNode Inode = (InteriorNode) n;
              // inNodes.Add(Inode);
            }
        }

        List<Circle> circles = Positioning.calculateCirclePositioningBeetle();

        circles[0].node = nodes[0];
        circles[1].node = nodes[2];
        circles[2].node = nodes[4];
        circles[3].node = nodes[5];
        circles[4].node = nodes[6];
        circles[5].node = nodes[3];
        circles[6].node = nodes[1];

        circles[4].node.size = circles[4].getRadius();

        return circles;
      }

      public static Tree exampleBeetleTree(){
        Console.WriteLine("loading the beetle");
        Tree tree = new Tree();

        InteriorNode inNode1 = new InteriorNode(0, tree);
        InteriorNode inNode2 = new InteriorNode(1, tree);
        InteriorNode inNode3 = new InteriorNode(2, tree);
        InteriorNode inNode4 = new InteriorNode(3, tree);
        InteriorNode inNode5 = new InteriorNode(4, tree);
        InteriorNode inNode6 = new InteriorNode(5, tree);
        // LeafNode head = new LeafNode(10, 1, inNode1, tree, true);
        LeafNode antenna = new LeafNode(11, 4, inNode1, tree);
        inNode1.addInteriorNode(inNode2, 1);
        LeafNode middle1 = new LeafNode(12, 1, inNode2, tree, true);
        inNode2.addInteriorNode(inNode3, 1);
        LeafNode legs1 = new LeafNode(13, 4, inNode3, tree);
        inNode3.addInteriorNode(inNode4, 1);
        LeafNode middle2 = new LeafNode(14, 1, inNode4, tree, true);
        inNode4.addInteriorNode(inNode5, 1);
        LeafNode legs2 = new LeafNode(15, 6, inNode5, tree);
        inNode5.addInteriorNode(inNode6, 2);
        LeafNode legs3 = new LeafNode(16, 8, inNode6, tree);
        LeafNode tail = new LeafNode(17, 4, inNode6, tree, true);

        tree.drawingOffsetX = 29.23/2;
        tree.drawingOffsetY = 29.23/2;
        tree.circles = tree.exampleBeetleCircles();
        return tree;
      }
      public static Tree exampleLongAntennaTree(){
        Console.WriteLine("loading a beetle with long antennas");
        Tree tree = new Tree();

        InteriorNode inNode1 = new InteriorNode(0, tree);
        // LeafNode head = new LeafNode(10, 1, inNode1, tree, true);
        LeafNode head = new LeafNode(11, 3.38, inNode1, tree, true);
        LeafNode arm1 = new LeafNode(12, 2, inNode1, tree);
        LeafNode arm2 = new LeafNode(13, 2, inNode1, tree);
        LeafNode arm3 = new LeafNode(14, 2, inNode1, tree);
        LeafNode leg = new LeafNode(15, 6, inNode1, tree);

        tree.drawingOffsetX = 12/2;
        tree.drawingOffsetY = 12/2;
        tree.circles = tree.exampleLongAntennaCircles();
        return tree;
      }
      public static Tree exampleLizardTree(){
        Console.WriteLine("loading a lizard");
        Tree tree = new Tree();
        int index = 0;
        InteriorNode frontNode = new InteriorNode(index++,tree);
        InteriorNode backNode = new InteriorNode(index++,tree);
        frontNode.addInteriorNode(backNode,1);
        LeafNode head = new LeafNode(index++,1,frontNode,tree, true);
        LeafNode foreLeg1 = new LeafNode(index++,1,frontNode,tree);
        //LeafNode foreLeg2 = new LeafNode(index++,1,frontNode,tree);
        LeafNode hindLeg1 = new LeafNode(index++,1,backNode,tree);
        //LeafNode hindLeg2 = new LeafNode(index++,1,backNode,tree);
        LeafNode tail = new LeafNode(index++,1,backNode,tree, true);

        tree.drawingOffsetX = 3.74/2;
        tree.drawingOffsetY = 3.74/2;
        tree.circles = tree.exampleLizardCircles();
        return tree;
      }
      public List<Circle> exampleLizardCircles(){
        List<LeafNode> nodes = new List<LeafNode>();
        foreach(var n in this.treeNodes){
          if(n.GetType() == typeof(LeafNode)){
              LeafNode Lnode = (LeafNode) n;
              nodes.Add(Lnode);
              Lnode.circle = new Circle(new Point2D(0, 0), 0);
            }
        }
        List<Circle> lizardCircles = new List<Circle>();
        lizardCircles.Add(new Circle(new Point2D(1.87,0.71), 1));
        lizardCircles.Add(new Circle(new Point2D(0, 0), 1));
        //lizardCircles.Add(new Circle(new Point2D(3.74, 0), 1));
        lizardCircles.Add(new Circle(new Point2D(0, 3.03), 1));
        //lizardCircles.Add(new Circle(new Point2D(3.74, 3.03), 1));
        lizardCircles.Add(new Circle(new Point2D(1.87,3.74), 1));
        for(int i=0; i<lizardCircles.Count(); i++)
          lizardCircles[i].node = nodes[i];
        return lizardCircles;
      }
      double pythagorean(double b, double c){
        return Math.Sqrt(c*c - b*b);
      }

      public static List<Circle> exampleDeerCircles(){
        List<Circle> deerCircles = new List<Circle>();
        deerCircles.Add(new Circle(new Point2D(0.5, 0.1635), 0.1126));
        deerCircles.Add(new Circle(new Point2D(0.3266, 0), 0.04422));
        deerCircles.Add(new Circle(new Point2D(0.2775, 0.1635), 0.04422));
        deerCircles.Add(new Circle(new Point2D(0.2281, 0), 0.04422));
        deerCircles.Add(new Circle(new Point2D(0.0698, 0.0698), 0.04422));
        deerCircles.Add(new Circle(new Point2D(0, 0.2594), 0.0761));
        deerCircles.Add(new Circle(new Point2D(0.2775, 0.5), 0.2205));

        foreach(var circle in deerCircles){
          LeafNode node = new LeafNode();
          circle.node = node;
        }
        return deerCircles;
      }
      public List<Circle> exampleLongAntennaCircles(){
        List<LeafNode> nodes = new List<LeafNode>();
        foreach(var n in this.treeNodes){
          if(n.GetType() == typeof(LeafNode)){
              LeafNode Lnode = (LeafNode) n;
              nodes.Add(Lnode);
              Lnode.circle = new Circle(new Point2D(0, 0), 0);
            }
        }

        List<Circle> circles = new List<Circle>();

        circles.Add(new Circle(new Point2D(6, 5), 3.38));
        circles.Add(new Circle(new Point2D(4, 0), 2));
        circles.Add(new Circle(new Point2D(0, 0), 2));
        circles.Add(new Circle(new Point2D(0, 4), 2));
        circles.Add(new Circle(new Point2D(0, 12), 6));

        circles[0].node = nodes[0];
        circles[1].node = nodes[1];
        circles[2].node = nodes[2];
        circles[3].node = nodes[3];
        circles[4].node = nodes[4];
        return circles;
      }

    }
}
