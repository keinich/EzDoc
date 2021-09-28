using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.PlacehoderResolving {

  class Rule {
    public string Key { get; set; }
    public string Value { get; set; }
    public static Rule Get(string key, string value) {
      return new Rule { Key = key, Value = value };
    }
  }

  class PlaceholderResolver {

    internal static string Resolve(string template,params Rule[] rules) {
      string result = template;
      foreach (Rule rule in rules) {
        result = result.Replace("{{" + rule.Key + "}}", rule.Value);
      }
      return result;
    }

  }
}
