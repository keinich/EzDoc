using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.HtmlDoucBuilding {

  class PageStructure {

    public class Node {
      public string Name { get; set; }
      public List<Node> Children { get; set; } = new List<Node>();
      internal bool IsLeaf() {
        return this.Children.Count == 0;
      }
      public string Content { get; set; }

    }

    public List<Node> Nodes { get; set; } = new List<Node>();

    internal void InsertNode(IEnumerable<string> nodeNames, string content) {
      InsertNodeInternal(this.Nodes, ref nodeNames, content);
    }

    private void InsertNodeInternal(List<Node> nodes, ref IEnumerable<string> nodeNames, string content) {
      if (nodeNames.Count() == 0) {
        return;
      }
      string nodeName = nodeNames.First();
      nodeNames = nodeNames.Skip(1);
      Node currentNode = this.Nodes.FirstOrDefault(x => x.Name == nodeName);
      if (currentNode is null) {
        currentNode = new Node() { Name = nodeName };
        nodes.Add(currentNode);
      }
      if (nodeNames.Count() == 0) {
        currentNode.Content = content;
      }
      else
        InsertNodeInternal(currentNode.Children, ref nodeNames, content);
    }
  }
}

