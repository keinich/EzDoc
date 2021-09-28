using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.HtmlDoucBuilding {
  class TableOfContent {
    public class Entry {
      public string Text { get; set; }
      public string Href { get; set; }
      public List<Entry> Items { get; internal set; } = new List<Entry>();
    }
    public List<Entry> Entries { get; set; } = new List<Entry>();
  }
}
