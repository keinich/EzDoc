using EzDoc.DocuGeneration;
using System;

namespace EzDoc {
  //class Program {
  //  static void Main(string[] args) {
  //  }
  //}
  class Program {

    static async System.Threading.Tasks.Task<int> Main(string[] args) {
      string[] categories = { };
      string[] namespacesToStrip = { };
      string docuFile = "D:/Raftek/EzDoc/EzDoc/TW.BCGer.Extensions.ZusagenFramework.xml";
      string outputPath = "D:/Raftek/EzDoc/EzDoc";
      if (args.Length == 2) {
        docuFile = args[0];
        outputPath = args[1];
      }
      try {
        await DocuGenerator.ConvertAsync(
          docuFile,
          outputPath
        );
        return 0;
      } catch (Exception ex) {
        Console.Write(ex.Message);
        Console.Write(ex.StackTrace);
        return 1;
      }
    }
  }
}