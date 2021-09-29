using EzDoc.HtmlDoucBuilding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.DocuGeneration {
  
  /// <summary>
  /// Creates Html-Docu from xml-Docu
  /// </summary>
  public class DocuGenerator {

    public static async Task ConvertAsync(string filenameXmlDocu, string projectPath) {

      DocuTree navTree = XmlDocuParser.CreateDocuTree(filenameXmlDocu, projectPath);
            
      await HtmlDocuBuilder.BuildHtmlDocu(projectPath, navTree, Path.GetFileNameWithoutExtension(filenameXmlDocu));
    }


  }
}
