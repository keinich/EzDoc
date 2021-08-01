using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.DocuGeneration {

  class DocuTreeNode {  

    public DocuTreeNode Parent { get; set; }
    public List<DocuTreeNode> Children { get; set; } = new List<DocuTreeNode>();
    public string Identifier { get; internal set; }
    public string Summary { get; internal set; }
    public DocuTreeNodeType Type { get; internal set; }

  }
}