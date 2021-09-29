using EzDoc.HtmlDoucBuilding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace EzDoc.DocuGeneration {

  class XmlDocuParser {

    private static Config ReadConfig(string path) {
      path = Path.Combine(path, "EzDoc");
      string configFilename = "EzDoc.json";
      string configFilepath = Path.Combine(path, configFilename);
      EnsureConfigFileExists(configFilepath);
      using (StreamReader r = new StreamReader(configFilepath)) {
        string json = r.ReadToEnd();
        Config res = JsonConvert.DeserializeObject<Config>(json);
        return res;
      }
    }

    private static void EnsureConfigFileExists(string configFilepath) {
      if (!File.Exists(configFilepath)) {
        string configDefault = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.EzDoc.json");
        File.WriteAllText(configFilepath, configDefault);
      }
    }

    public static DocuTree CreateDocuTree(string filename, string path) {
           
      Config config = ReadConfig(path);

      XmlTextReader reader = new XmlTextReader(filename);
      DocuTree navTree = new DocuTree();
      try {
        reader.WhitespaceHandling = WhitespaceHandling.None;

        DocuTreeNode currentNode = null;
        string currentNodeType = "";

        while (reader.Read()) {
          HandleXmlNode(reader, navTree, ref currentNode, ref currentNodeType, config);
        }
      } finally {
        if (reader != null)
          reader.Close();
      }

      return navTree;
    }

    private static void HandleXmlNode(
      XmlTextReader reader, DocuTree navTree, ref DocuTreeNode currentNode, ref string currentNodeType,
      Config config
    ) {
      switch (reader.NodeType) {
        case XmlNodeType.Element:
          currentNodeType = "other";
          switch (reader.Name) {
            case "member":
              string name = reader.GetAttribute("name");
              if (name.StartsWith("T:")) {
                currentNode = HandleTypeOrProperty(reader, name, navTree, DocuTreeNodeType.Type, config);
                currentNodeType = "type";
              } else if (name.StartsWith("P:")) {
                currentNode = HandleTypeOrProperty(reader, name, navTree, DocuTreeNodeType.Property, config);
                currentNodeType = "property";
              } else if (name.StartsWith("F:")) {
                currentNode = HandleTypeOrProperty(reader, name, navTree, DocuTreeNodeType.Field, config);
                currentNodeType = "property";
              } else if (name.StartsWith("M:")) {
                currentNode = HandleMethod(reader, name, navTree);
                currentNodeType = "property";
              }
              break;
            case "summary":
              currentNodeType = "summary";
              string innerXml = reader.ReadInnerXml();
              currentNode.Summary = innerXml;
              HandleXmlNode(reader, navTree, ref currentNode, ref currentNodeType, config);
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
      DocuTreeNodeType t,
      Config config
    ) {

      List<string> typeNames = name.Substring(2).Split('.').ToList();
            
      if (!config.Include(name.Substring(2))) {
        return new DocuTreeNode { Identifier = "", Type = t };
      }

      DocuTreeNode node = navStuff.GetOrCreateNode(typeNames);
      node.Type = t;
      return node;

    }

    private static string GetNamespace(string v) {
      int indexOfLastDot = v.LastIndexOf('.');
      if (indexOfLastDot < 0 ) {
        return "";
      }
      return v.Substring(0, indexOfLastDot);
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


      List<string> typeNames = name.Substring(2, indexOfBracket - 2).Split('.').ToList();

      DocuTreeNode node = navStuff.GetOrCreateNode(typeNames);
      node.Type = DocuTreeNodeType.Method;
      return node;

    }

  }
}