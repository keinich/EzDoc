using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc1.HtmlDoucBuilding {
  internal static class StringExtensions {
    public static string MakeCodeReady(this string v) {
      string result = v.Replace(" ", "");
      result = result.Replace("´", "");
      result = result.Replace("`", "");
      result = result.Replace("`", "");
      result = result.Replace("(", "");
      result = result.Replace(")", "");
      return result;
    }
  }
}
