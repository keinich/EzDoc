using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc {
  class Utils {

    public static void EnsureEmptyDirExists(string path) {
      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
        while (!Directory.Exists(path)) { }
      } else {
        foreach (string file in Directory.EnumerateFiles(path)) {
          File.Delete(file);
        }
      }
    }
  }
}
