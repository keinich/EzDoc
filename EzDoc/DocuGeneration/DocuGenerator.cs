using EzDoc.HtmlDoucBuilding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.DocuGeneration {
  class DocuGenerator {

    internal static async Task ConvertAsync(string[] categories) {
      string filename = "D:/Raftek/EzDoc/Zusagenframework/Zusagenframework.xml";

      DocuTree navTree = XmlDocuParser.CreateDocuTree(categories, filename);

      await HtmlDocuBuilder.BuildHtmlDocu("D:/Raftek/EzDoc/Zusagenframework", navTree);
    }


  }
}
