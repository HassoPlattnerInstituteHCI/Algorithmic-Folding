using System;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking{

  public class Node{
    public bool middle; //not "mirrored" for symmetry
    public int index;
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
    public Circle circle;
    public LeafNode(int index, double size, InteriorNode relatedNode, Tree tree, Circle c, bool middle = false) : base(index, tree, middle) {
      this.relatedNode = relatedNode;
      this.size = size;
      relatedNode.addLeafNode(this);
      this.assignCircle(c);
    }
    public void assignCircle(Circle c){
      this.circle = c;
      c.node = this;
    }
    public LeafNode getCenterNeighbor(){
      foreach(var leafNodePair in relatedNode.relatedLeafNodes)
        if(leafNodePair.Value.middle)
          return leafNodePair.Value;
      return null;
    }
    public double getTreeDistanceTo(LeafNode other){ //recursively walks the tree until other node is reached
      return (other==this)?0:this.size+this.relatedNode.getTreeDistanceTo(other);
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
      public List<Node> treeNodes = new List<Node>();
      public double drawingOffset=0;
      public void addNode(Node node){
        treeNodes.Add(node);
      }
      public List<LeafNode> getLeafNodes(){
        return treeNodes.OfType<LeafNode>().ToList();
      }
      public List<Circle> getCircles(){
        List<Circle> c = new List<Circle>();
        foreach (var node in getLeafNodes())
          c.Add(node.circle);
        return c;
      }
      public double getPaperSize(){
        return 2*this.drawingOffset;
      }
      public void setDrawingOffset(List<Circle> list){
        double x=0;
        foreach (var c in list)
          x = (c.getCenter().x > x)?c.getCenter().x:x;
        this.drawingOffset = x;
      }
      void assignCircleOrder(List<Circle> c){
        for (int i=0; i<c.Count-1;i++)
          c[i].next = c[i+1];
        c.Last().next = c[0];
      }

      //hardcoded tree assignments
      public static Tree exampleBeetleTree(){
        Console.WriteLine("loading the beetle");
        Tree tree = new Tree();
        List<Circle> circles = Positioning.calculateCirclePositioningBeetle();
        int id = 0;
        InteriorNode inNode1 = new InteriorNode(id, tree);
        InteriorNode inNode2 = new InteriorNode(++id, tree);
        InteriorNode inNode3 = new InteriorNode(++id, tree);
        InteriorNode inNode4 = new InteriorNode(++id, tree);
        InteriorNode inNode5 = new InteriorNode(++id, tree);
        InteriorNode inNode6 = new InteriorNode(++id, tree);

        LeafNode antenna = new LeafNode(++id, 4, inNode1, tree, circles[0]);
        inNode1.addInteriorNode(inNode2, 1);
        LeafNode middle1 = new LeafNode(++id, 1, inNode2, tree, circles[6],true);
        inNode2.addInteriorNode(inNode3, 1);
        LeafNode legs1 = new LeafNode(++id, 4, inNode3,  tree,circles[1]);
        inNode3.addInteriorNode(inNode4, 1);
        LeafNode middle2 = new LeafNode(++id, 1, inNode4, tree, circles[5],true);
        inNode4.addInteriorNode(inNode5, 1);
        LeafNode legs2 = new LeafNode(++id, 6, inNode5, tree,circles[2]);
        inNode5.addInteriorNode(inNode6, 2);
        LeafNode legs3 = new LeafNode(++id, 8, inNode6,tree, circles[3]);
        LeafNode tail = new LeafNode(++id, 4, inNode6, tree,circles[4], true);
        tree.setDrawingOffset(circles);
        circles[4].node.size = circles[4].getRadius();
        tree.assignCircleOrder(circles);
        return tree;
      }
      public static Tree simpleThreeNodes(){
        Console.WriteLine("loading the threenodes");
        Tree tree = new Tree();
        int id = 0;
        List<Circle> threeCircles = new List<Circle>();
        threeCircles.Add(new Circle(20,0, 10));
        threeCircles.Add(new Circle(0,0, 10));
        threeCircles.Add(new Circle(10,18, 10));

        InteriorNode inNode1 = new InteriorNode(id, tree);
        LeafNode upperMid = new LeafNode(++id, 10, inNode1, tree,threeCircles[0], true);
        LeafNode upperLeft = new LeafNode(++id, 10, inNode1, tree, threeCircles[1]);
        LeafNode lowerMid = new LeafNode(++id, 10, inNode1, tree, threeCircles[2],true);
        tree.setDrawingOffset(threeCircles);
        tree.assignCircleOrder(threeCircles);
        return tree;
      }
      public static Tree exampleLongAntennaTree(){
        Console.WriteLine("loading a beetle with long antennas");
        Tree tree = new Tree();

        List<Circle> circles = new List<Circle>();

        circles.Add(new Circle(6, 5, 3.38));
        circles.Add(new Circle(4, 0, 2));
        circles.Add(new Circle(0, 0, 2));
        circles.Add(new Circle(0, 4, 2));
        circles.Add(new Circle(0, 12, 6));

        int id=0;
        InteriorNode inNode1 = new InteriorNode(id, tree);
        LeafNode head = new LeafNode(++id, 3.38, inNode1, tree, circles[id-1],true);
        LeafNode arm1 = new LeafNode(++id, 2, inNode1, tree, circles[id-1]);
        LeafNode arm2 = new LeafNode(++id, 2, inNode1, tree, circles[id-1]);
        LeafNode arm3 = new LeafNode(++id, 2, inNode1, tree, circles[id-1]);
        LeafNode leg = new LeafNode(++id, 6, inNode1, tree, circles[id-1]);

        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return tree;
      }
      public static Tree exampleLizardTree(){
        Console.WriteLine("loading a lizard");
        Tree tree = new Tree();
        int index = 0;
        List<Circle> lizardCircles = new List<Circle>();
        lizardCircles.Add(new Circle(1.87,0.71, 1));
        lizardCircles.Add(new Circle(0, 0, 1));
        lizardCircles.Add(new Circle(0, 3.03, 1));
        lizardCircles.Add(new Circle(1.87,3.74, 1));

        InteriorNode frontNode = new InteriorNode(index++,tree);
        InteriorNode backNode = new InteriorNode(index++,tree);
        frontNode.addInteriorNode(backNode,1);
        LeafNode head = new LeafNode(index++,1,frontNode,tree, lizardCircles[0],true);
        LeafNode foreLeg1 = new LeafNode(index++,1,frontNode,tree,lizardCircles[1]);
        LeafNode hindLeg1 = new LeafNode(index++,1,backNode,tree, lizardCircles[2]);
        LeafNode tail = new LeafNode(index++,1,backNode,tree,lizardCircles[3], true);

        tree.setDrawingOffset(lizardCircles);
        tree.assignCircleOrder(lizardCircles);
        return tree;
      }

    }
}
