using EzDoc.DocuGeneration;
using System;

namespace EzDoc {
  class Program {
    static async System.Threading.Tasks.Task Main(string[] args) {
      string[] categories = { };
      string[] namespacesToStrip = { };

      await DocuGenerator.ConvertAsync(
        "D:/Raftek/EzDoc/Zusagenframework/Zusagenframework.xml",
        "D:/Raftek/EzDoc/Zusagenframework"
      );
    }
  }
}