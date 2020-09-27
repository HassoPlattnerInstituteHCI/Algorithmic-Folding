using System;
using System.Collections.Generic;


namespace inClassHacking{

  class Node{
    double undef = -1;
    public bool middle; //not "mirrored" for symmetrie
    public int index;

    public Node(int index, bool middle){
      this.index = index;
      this.middle = middle;
    }

  }

  class LeafNode: Node{
    public InteriorNode relatedNode;
    public double size;

    public LeafNode(int index, double size, InteriorNode relatedNode, bool middle = false) : base(index, middle) {
      this.relatedNode = relatedNode;
      this.size = size;
      relatedNode.addLeafNode(this);
    }
  }

  class InteriorNode: Node{
    public Dictionary<int, LeafNode> relatedLeafNodes = new Dictionary<int, LeafNode>();
    public Dictionary<InteriorNode, double> relatedInteriorNodes = new Dictionary< InteriorNode, double>();

    public InteriorNode(int index, LeafNode[] relatedLeafNodes): base(index, true){
      foreach(var node in relatedLeafNodes){
        this.relatedLeafNodes[node.index] = node;
      }
    }

    public InteriorNode(int index) : base(index, true){}

    public void addLeafNode(LeafNode node){
      this.relatedLeafNodes[node.index] = node;
    }

    public void addInteriorNode(InteriorNode node, double distance){
      this.relatedInteriorNodes[node] = distance;
      node.relatedInteriorNodes[this] = distance;
    }

    
  }

    class Tree{
      public List<Node> treeNodes = new List<Node>();

      public void addNode(Node node){
        treeNodes.Add(node);
      }

      public List<Circle> calculateCirclePositioning(){
        List<Circle> circles = new List<Circle>();
        LeafNode startingNode;
        foreach(var node in treeNodes){
          if(node.GetType() == typeof(InteriorNode)){
            Console.WriteLine(node.index);
            // circles.Add();
          }
        }

        return circles;
      }

      // Node nextStep(Node node, Node )

    }


  
}