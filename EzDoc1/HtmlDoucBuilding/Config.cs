using System;
using System.Collections.Generic;

namespace EzDoc.HtmlDoucBuilding {

  internal class Filter {
    public List<string> Include { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();
  }

  internal class Config {
    public Filter Filter { get; set; } = new Filter();

    internal bool Include(string v) {
      
      // Check Includes
      foreach (string includeString in Filter.Include) {
        if (includeString.EndsWith(".*")) {
          int indexOfPattern = includeString.LastIndexOf('.');
          if (v.StartsWith(includeString.Substring(indexOfPattern))) {
            return true;
          }
        } else {
          if (includeString.ToLower() == v.ToLower()) {
            return true;
          }
        }
      }

      // Check Excludes
      foreach (string excludeString in Filter.Exclude) {
        if (excludeString.EndsWith(".*")) {
          int indexOfPattern = excludeString.LastIndexOf('.');
          if (v.StartsWith(excludeString.Substring(0, indexOfPattern))) {
            return false;
          }
        } else {
          if (excludeString.ToLower() == v.ToLower()) {
            return false;
          }
        }
      }
      return true;
    }
  }
}