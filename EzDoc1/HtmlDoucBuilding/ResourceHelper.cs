using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.HtmlDoucBuilding {
  class ResourceHelper {

    public static string LoadResource(string name) {
      var assembly = Assembly.GetExecutingAssembly();
      var resourceName = name;
      string result;
      using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      using (StreamReader reader = new StreamReader(stream)) {
        result = reader.ReadToEnd();
      }
      return result;
    }

  }
}
