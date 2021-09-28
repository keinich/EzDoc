﻿using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EzDocVsix {
  class ProjectHelpers {

    public class Result {
      public string Message { get; set; }
      public bool Success { get; set; }

      public static Result Ok() {
        return new Result { Message = "Success", Success = true };
      }

      public static Result Ok(string message) {
        return new Result { Message = message, Success = true };
      }

      public static Result Failed(string message) {
        return new Result { Message = message, Success = false };
      }
    }

    public static Result GenerateDocu(Project project) {
      string projectFile = project.FileName;
      var config = project.ConfigurationManager;
      if (string.IsNullOrEmpty(projectFile)) {
        return Result.Failed("No Porject File");
      }
      string docuFile = TryGetDocuFile(projectFile);
      if (string.IsNullOrEmpty(docuFile)) {
        return Result.Failed("No Documentation File");
      }
      string outputPath = Path.GetDirectoryName(project.FileName);

      string cPath = "D:\\Raftek\\EzDoc\\EzDoc\\bin\\Debug\\net5.0";
      string filename = Path.Combine(cPath, "EzDoc.exe");
      string cParams = docuFile + " " + outputPath;
      var process = new System.Diagnostics.Process {
        StartInfo = { 
          FileName = filename, 
          Arguments = cParams,
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          UseShellExecute = false
        },
        EnableRaisingEvents = true
      };      
      
      process.Start();
      string output = "";
      while (!process.StandardOutput.EndOfStream) {
        output += process.StandardOutput.ReadLine();
        // do something with line
      }
      process.WaitForExit();
      int exitCode = process.ExitCode;
      if (exitCode == 0) {
        return Result.Ok($"Succesfully created to {outputPath} from {docuFile}");
      } else {
        return Result.Failed(output);
      }
    }

    private static string TryGetDocuFile(string filename) {
      XmlTextReader reader = null;

      try {

        // Load the reader with the data file and ignore all white space nodes.
        reader = new XmlTextReader(filename);
        reader.WhitespaceHandling = WhitespaceHandling.None;

        bool foundDocumentationFile = false;
        // Parse the file and display each of the nodes.
        while (reader.Read()) {
          switch (reader.NodeType) {
            case XmlNodeType.Element:
              if (reader.Name == "DocumentationFile") {
                foundDocumentationFile = true;
              }
              break;
            case XmlNodeType.Text:
              if (foundDocumentationFile) {
                return reader.Value;
              }
              break;
            default:
              break;
          }
        }
      } finally {
        if (reader != null)
          reader.Close();
      }
      return string.Empty;
    }
  }
}
