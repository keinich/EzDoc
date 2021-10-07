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

      await HtmlDocuBuilder.BuildHtmlDocu(projectPath, navTree, GetFrameworkNameFromFile(filenameXmlDocu));
    }
    
    private static string GetFrameworkNameFromFile(string filenameXmlDocu) {
      string result = Path.GetFileNameWithoutExtension(filenameXmlDocu);
      int indexOfLastDot = result.LastIndexOf(".");
      if (indexOfLastDot < 0) {
        return result;
      }
      return result.Substring(indexOfLastDot + 1, result.Length - indexOfLastDot - 1);
    }
  }
}
