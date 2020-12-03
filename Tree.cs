using System;
using System.Collections.Generic;
using System.Linq;

namespace inClassHacking{

  public class Node{
    public int index;
    public Node(int index, Tree tree){
      this.index = index;
      tree.addNode(this);
    }
    public Node(){}
  }

  public class LeafNode: Node{
    public InteriorNode relatedNode;
    public double size;
    public Circle circle;
    public LeafNode(int index,InteriorNode relatedNode, Tree tree, Circle c) : base(index, tree) {
      this.relatedNode = relatedNode;
      this.size = c.getRadius();
      relatedNode.addLeafNode(this);
      this.assignCircle(c);
    }
    public void assignCircle(Circle c){
      this.circle = c;
      c.node = this;
    }
    public double getTreeDistanceTo(LeafNode other){ //recursively walks the tree until other node is reached
      return (other==this)?0:this.size+this.relatedNode.getTreeDistanceTo(other);
    }
  }
  public class InteriorNode: Node{
    public Dictionary<int, LeafNode> relatedLeafNodes = new Dictionary<int, LeafNode>();
    public Dictionary<InteriorNode, double> relatedInteriorNodes = new Dictionary< InteriorNode, double>();

    public InteriorNode(int index, Tree tree) : base(index, tree){}

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
      public static List<Tree> exampleBeetleTree(){
        Console.WriteLine("loading the beetle");
        List<Tree> langPolygons = new List<Tree>();
        langPolygons.Add(beetle7polySimple());
      /*  langPolygons.Add(beetle7poly(new List<double>(){
          29.26-9.63,0,4,
          29.26-0,2.7,4,
          29.26-0,14.7,6,
          29.26-6.63,29.26,8,
          29.26-14.63,17.55,6.24,
          29.26-14.63,7.32,1,
          29.26-14.63,3.32,1
        }));*/
        return langPolygons;
      }
      public static List<Circle> generateCircles(List<double> data){
        List<Circle> list = new List<Circle>();
        for (int i =0; i <(data.Count/3);i++)
          list.Add(new Circle(data[0+i*3],data[1+i*3],data[2+i*3]));
        return list;
      }
      public static List<InteriorNode> generateInteriorNodes(int n, Tree tree){
        List<InteriorNode> list = new List<InteriorNode>();
        for (int i=0; i<n;i++)
          list.Add(new InteriorNode(i,tree));
        return list;
      }
      public static Tree beetle7polySimple(){
        Tree tree = new Tree();
        List<Circle> circles = generateCircles(new List<double>(){
          9.63,0,4,
          0,2.7,4,
          0,14.7,6,
          6.63,29.26,8,
          14.63,17.55,6.24,
          14.63,7.32,1,
          14.63,3.32,1
        });
        List<InteriorNode> iNodes = generateInteriorNodes(1, tree);
        int id = 1;
        int c =0;
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //antenna
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //legs1
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //legs2
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //legs3
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //tail
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //middle2
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //middle1
        //iNodes[0].addInteriorNode(iNodes[1], 2);
        /*iNodes[1].addInteriorNode(iNodes[2], 1);
        iNodes[2].addInteriorNode(iNodes[3], 1);
        iNodes[3].addInteriorNode(iNodes[4], 1);
        iNodes[4].addInteriorNode(iNodes[5], 2);*/
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return tree;
      }
      public static Tree beetle7poly(){
        Tree tree = new Tree();
        List<Circle> circles = generateCircles(new List<double>(){
          9.63,0,4,
          0,2.7,4,
          0,14.7,6,
          6.63,29.26,8,
          14.63,17.55,6.24,
          14.63,7.32,1,
          14.63,3.32,1
        });
        List<InteriorNode> iNodes = generateInteriorNodes(6, tree);
        int id = 6;
        int c =0;
        new LeafNode(id++,iNodes[0],tree, circles[c++]);  //antenna
        new LeafNode(id++,iNodes[2],tree, circles[c++]);  //legs1
        new LeafNode(id++,iNodes[4],tree, circles[c++]);  //legs2
        new LeafNode(id++,iNodes[5],tree, circles[c++]);  //legs3
        new LeafNode(id++,iNodes[5],tree, circles[c++]);  //tail
        new LeafNode(id++,iNodes[3],tree, circles[c++]);  //middle2
        new LeafNode(id++,iNodes[1],tree, circles[c++]);  //middle1
        iNodes[0].addInteriorNode(iNodes[1], 1);
        iNodes[1].addInteriorNode(iNodes[2], 1);
        iNodes[2].addInteriorNode(iNodes[3], 1);
        iNodes[3].addInteriorNode(iNodes[4], 1);
        iNodes[4].addInteriorNode(iNodes[5], 2);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return tree;
      }
      public static List<Tree> simpleThreeNodes(){
        Console.WriteLine("loading the threenodes");
        Tree tree = new Tree();
        int id = 0;
        List<Circle> circles = generateCircles(new List<double>(){
          20,0,10,
          0,0,10,
          10,18,10
        });

        InteriorNode inNode1 = new InteriorNode(id, tree);
        LeafNode upperMid = new LeafNode(++id, inNode1, tree,circles[0]);
        LeafNode upperLeft = new LeafNode(++id, inNode1, tree, circles[1]);
        LeafNode lowerMid = new LeafNode(++id, inNode1, tree, circles[2]);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return new List<Tree>(){tree};
      }
      public static List<Tree> exampleLongAntennaTree(){
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
        LeafNode head = new LeafNode(++id, inNode1, tree, circles[id-1]);
        LeafNode arm1 = new LeafNode(++id, inNode1, tree, circles[id-1]);
        LeafNode arm2 = new LeafNode(++id, inNode1, tree, circles[id-1]);
        LeafNode arm3 = new LeafNode(++id, inNode1, tree, circles[id-1]);
        LeafNode leg = new LeafNode(++id, inNode1, tree, circles[id-1]);

        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return new List<Tree>(){tree};
      }
      public static List<Tree> exampleLizardTree(){
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
        LeafNode head = new LeafNode(index++,frontNode,tree, lizardCircles[0]);
        LeafNode foreLeg1 = new LeafNode(index++,frontNode,tree,lizardCircles[1]);
        LeafNode hindLeg1 = new LeafNode(index++,backNode,tree, lizardCircles[2]);
        LeafNode tail = new LeafNode(index++,backNode,tree,lizardCircles[3]);

        tree.setDrawingOffset(lizardCircles);
        tree.assignCircleOrder(lizardCircles);
        return new List<Tree>(){tree};
      }

    }
}
