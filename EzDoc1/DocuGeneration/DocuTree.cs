using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.DocuGeneration {

  /// <summary>
  /// Tree structure for xml-comments.
  /// </summary>
  class DocuTree {

    public DocuTreeNode RootNode {
      get;
      set;
    } = new DocuTreeNode() { Identifier = "Root", Summary = "Root", Type = DocuTreeNodeType.Namespace };

    public DocuTreeNode GetOrCreateNode(List<string> path) {
      DocuTreeNode currentParent = this.RootNode;
      int remainingPath = path.Count;
      foreach (string nodeIdentifier in path) {
        bool found = false;
        foreach (DocuTreeNode child in currentParent.Children) {
          if (child.Identifier == nodeIdentifier) {
            currentParent = child;
            found = true;
            break;
          }

        }

        if (!found) {
          List<string> remainingPathList = path.GetRange(path.Count - remainingPath, remainingPath);
          return InsertNewNode(currentParent, remainingPathList);
        }

        remainingPath -= 1;
      }
      return currentParent;
    }
  
    private DocuTreeNode InsertNewNode(DocuTreeNode parent, IEnumerable<string> path) {
      DocuTreeNode currentParent = parent;
      foreach (string nodeIdentifier in path) {
        DocuTreeNode newNode = new DocuTreeNode {
          Identifier = nodeIdentifier,
          Parent = currentParent,
          Type = DocuTreeNodeType.Namespace
        }

      ;
        currentParent.Children.Add(newNode);
        currentParent = newNode;
      }
      return currentParent;
    }
  }
}
