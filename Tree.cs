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
      public bool mirrored = false;
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
        langPolygons.Add(beetlePoly());
        langPolygons.Add(beetleBottomTriangle());
        langPolygons.Add(beetleTopTriangle());
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
      public static Tree beetlePoly(){
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
        id = 0;
        iNodes[id].addInteriorNode(iNodes[++id], 1);
        iNodes[id].addInteriorNode(iNodes[++id], 1);
        iNodes[id].addInteriorNode(iNodes[++id], 1);
        iNodes[id].addInteriorNode(iNodes[++id], 1);
        iNodes[id].addInteriorNode(iNodes[++id], 2);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        tree.mirrored = true;
        return tree;
      }
      public static Tree beetleBottomTriangle(){
        Tree tree = new Tree();
        List<Circle> circles = generateCircles(new List<double>(){
          14.63,17.55,6.24,
          6.63,29.26,8,
          22.63,29.26,8
        });
        InteriorNode inNode = new InteriorNode(0, tree);
        int c =0;
        new LeafNode(c,inNode,tree, circles[c++]);
        new LeafNode(c,inNode,tree, circles[c++]);
        new LeafNode(c,inNode,tree, circles[c++]);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return tree;
      }
      public static Tree beetleTopTriangle(){
        Tree tree = new Tree();
        List<Circle> circles = generateCircles(new List<double>(){
          19.63,0,4,
          14.63,0,1,
          9.63,0,4,
          14.63,3.32,1
        });
        InteriorNode inNode = new InteriorNode(0, tree);
        InteriorNode inNode2 = new InteriorNode(0, tree);
        int c =0;
        new LeafNode(c,inNode,tree, circles[c++]);
        new LeafNode(c,inNode,tree, circles[c++]);
        new LeafNode(c,inNode,tree, circles[c++]);
        new LeafNode(c,inNode2,tree, circles[c++]);
        inNode.addInteriorNode(inNode2,1);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return tree;
      }

      public static List<Tree> simpleThreeNodes(){
        Console.WriteLine("loading the threenodes");
        Tree tree = new Tree();
        int id = 0;
        int scaled = 10;
        List<Circle> circles = generateCircles(multiply(new List<double>(){
          2,0,1,
          0,0,1,
          1,1.74,1
        },scaled));
        InteriorNode inNode = new InteriorNode(id, tree);
        new LeafNode(++id, inNode, tree, circles[id-1]);
        new LeafNode(++id, inNode, tree, circles[id-1]);
        new LeafNode(++id, inNode, tree, circles[id-1]);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return new List<Tree>(){tree};
      }
      public static List<Tree> exampleLongAntennaTree(bool flipped=false){
        Console.WriteLine("loading a beetle with long antennas");
        List<Tree> langPolygons = new List<Tree>();
        langPolygons.Add(longAntennaPentagon());
        langPolygons.Add(longAntennaPentagon(true));
        return langPolygons;
      }
      public static Tree longAntennaPentagon(bool flipped=false){
        Tree tree = new Tree();
        int scaled = 10;
        List<Circle> circles = (flipped)?generateCircles(multiply(shiftX(new List<double>(){
          3,2,1,
          3,0,1,
          1,0,1,
          0,2.5,1.69,
          3,6.12,3
        },3),scaled)):generateCircles(multiply(new List<double>(){
          3,2.5,1.69,
          2,0,1,
          0,0,1,
          0,2,1,
          0,6.12,3
        },scaled));
        int id=0;
        InteriorNode inNode = new InteriorNode(id, tree);
        new LeafNode(++id, inNode, tree, circles[id-1]);//head
        new LeafNode(++id, inNode, tree, circles[id-1]);//arm1
        new LeafNode(++id, inNode, tree, circles[id-1]);//arm2
        new LeafNode(++id, inNode, tree, circles[id-1]);//arm3
        new LeafNode(++id, inNode, tree, circles[id-1]);//leg

        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        return tree;
      }
      public static List<Tree> exampleLizardTree(){
        Console.WriteLine("loading a lizard");
        List<Tree> langPolygons = new List<Tree>();
        langPolygons.Add(parallelogramLizard());
        //langPolygons.Add(parallelogramLizard(true));
        return langPolygons;
      }
      public static List<double> shiftX(List<double> data, double x){
        for (int i=0; i<data.Count;i+=3)
          data[i] += x;
        return data;
      }
      public static List<double> multiply(List<double> data, int n){
        List<double> result = new List<double>();
        foreach (var d in data)
          result.Add(d*n);
        return result;
      }
      public static Tree parallelogramLizard(bool flipped=false){
        Tree tree = new Tree();
        int scaled = 10;
        List<Circle> circles = (flipped)?generateCircles(multiply(shiftX(new List<double>(){
          1.908,0,1,
          0,0.6,1,
          0,3.6,1,
          1.908,3,1
        },1.908),scaled)):generateCircles(multiply(new List<double>(){
          1.908,0.6,1,
          0,0,1,
          0,3,1,
          1.908,3.6,1
        },scaled));
        List<InteriorNode> iNodes = generateInteriorNodes(2, tree);
        int id = 2;
        int c= 0;
        new LeafNode(id++,iNodes[0],tree, circles[c++]);
        new LeafNode(id++,iNodes[0],tree, circles[c++]);
        new LeafNode(id++,iNodes[1],tree, circles[c++]);
        new LeafNode(id++,iNodes[1],tree, circles[c++]);
        iNodes[0].addInteriorNode(iNodes[1],scaled);
        tree.setDrawingOffset(circles);
        tree.assignCircleOrder(circles);
        tree.mirrored=true;
        return tree;
      }
    }
}
