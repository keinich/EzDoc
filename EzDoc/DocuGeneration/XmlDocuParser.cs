using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace EzDoc.DocuGeneration {

  class XmlDocuParser {
    public static DocuTree CreateDocuTree(string[] categories, string filename) {

      XmlTextReader reader = new XmlTextReader(filename);
      DocuTree navTree = new DocuTree();
      try {
        reader.WhitespaceHandling = WhitespaceHandling.None;

        DocuTreeNode currentNode = null;
        string currentNodeType = "";

        while (reader.Read()) {
          HandleXmlNode(reader, navTree, ref currentNode, ref currentNodeType);
        }
      }

      finally {
        if (reader != null)
          reader.Close();
      }

      return navTree;
    }

        private static void HandleXmlNode(XmlTextReader reader, DocuTree navTree, ref DocuTreeNode currentNode, ref string currentNodeType) {
      switch (reader.NodeType) {
        case XmlNodeType.Element:
          currentNodeType = "other";
          switch (reader.Name) {
            case "member":
              string name = reader.GetAttribute("name");
              if (name.StartsWith("T:")) {
                currentNode = HandleTypeOrProperty(reader, name, navTree, DocuTreeNodeType.Type);
                currentNodeType = "type";
              }
              else if (name.StartsWith("P:")) {
                currentNode = HandleTypeOrProperty(reader, name, navTree, DocuTreeNodeType.Property);
                currentNodeType = "property";
              }
              else if (name.StartsWith("F:")) {
                currentNode = HandleTypeOrProperty(reader, name, navTree, DocuTreeNodeType.Field);
                currentNodeType = "property";
              }
              else if (name.StartsWith("M:")) {
                currentNode = HandleMethod(reader, name, navTree);
                currentNodeType = "property";
              }
              break;
            case "summary":
              currentNodeType = "summary";
              string innerXml = reader.ReadInnerXml();
              currentNode.Summary = innerXml;
              HandleXmlNode(reader, navTree,ref currentNode, ref currentNodeType);
              break;
            case "param":
              currentNodeType = "param";
              break;
            case "returns":
              currentNodeType = "returns";
              break;
            default:
              currentNodeType = "other";
              break;
          }
          break;
        case XmlNodeType.Text:
          //Console.WriteLine(reader.Value);
          switch (currentNodeType) {
            case "summary":
              currentNode.Summary = reader.Value;
              break;
          }
          break;
        case XmlNodeType.EndElement:
          //Console.Write("</{0}>", reader.Name);
          break;
      }
    }

    private static DocuTreeNode HandleTypeOrProperty(
      XmlTextReader reader,
      string name,
      DocuTree navStuff,
      DocuTreeNodeType t
    ) {
      List<string> typeNames = name[2..].Split(".").ToList();

      DocuTreeNode node = navStuff.GetOrCreateNode(typeNames);
      node.Type = t;
      return node;

    }

    private static DocuTreeNode HandleMethod(
      XmlTextReader reader,
      string name,
      DocuTree navStuff
    ) {
      int indexOfBracket = name.IndexOf("(");
      if (indexOfBracket < 0) {
        indexOfBracket = name.Length;
      }

      List<string> typeNames = name[2..indexOfBracket].Split(".").ToList();

      DocuTreeNode node = navStuff.GetOrCreateNode(typeNames);
      node.Type = DocuTreeNodeType.Method;
      return node;

    }

  }
}