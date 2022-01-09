using System.Collections.Generic;

namespace EzDoc.HtmlDoucBuilding {
  class TableOfContent {
    public class Folder {
      public string TabName { get; set; }
      public string FolderName{ get; set; }
      public List<Node> Nodes { get; internal set; } = new List<Node>();
      public List<string> Pages { get; internal set; } = new List<string>();
      public List<Folder> Folders { get; set; } = new List<Folder>();
    }
    public class Node {
      public string Name { get; set; }
      public List<Node> Nodes { get; internal set; } = new List<Node>();
      public List<string> Pages { get; internal set; } = new List<string>();
    }
   
    public List<Folder> Folders { get; set; } = new List<Folder>();
  }
}
