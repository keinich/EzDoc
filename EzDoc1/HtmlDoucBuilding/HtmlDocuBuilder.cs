using EzDoc.DocuGeneration;
using EzDoc.PlacehoderResolving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace EzDoc.HtmlDoucBuilding {
  class HtmlDocuBuilder {

    public static async Task BuildHtmlDocu(string path, DocuTree navTree, string frameworkName) {

      ConvertLinks(navTree.RootNode);

      
      path = Path.Combine(path, "EzDoc");
      string outputPath = Path.Combine(path, "_out");
      EnsureEmptyDirExists(outputPath);
      CreateStaticFiles(outputPath);

      string navbarFilename = "navbar_content.yaml";
      string navbarFilepath = System.IO.Path.Combine(path, navbarFilename);
      EnsureNavbarFileExists(navbarFilepath);
      TableOfContent toc = ReadTableOfContent(navbarFilepath);
      foreach (TableOfContent.Entry entry in toc.Entries) {
        if (entry.Items.Count == 0) {
          await BuildPageAsync(path, outputPath, toc, entry, navTree, frameworkName);
        } else {
          foreach (TableOfContent.Entry item in entry.Items) {
            await BuildPageAsync(path, outputPath, toc, item, navTree, frameworkName);
          }
        }
      }

    }

    private static void EnsureNavbarFileExists(string navbarFilepath) {
      if (!File.Exists(navbarFilepath)) { 
        string navbarDefault = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.navbar_default.yaml");        
        File.WriteAllText(navbarFilepath, navbarDefault);
      }
   }

    private static void EnsureEmptyDirExists(string path) {
      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
        while (!Directory.Exists(path)) { }
      } else {
        foreach (string file in Directory.EnumerateFiles(path)) {
          File.Delete(file);
        }
      }
    }

    private static void ConvertLinks(DocuTreeNode node) {
      node.Summary = ConvertLinks(node.Summary);
      foreach (DocuTreeNode cn in node.Children) {
        ConvertLinks(cn);
      }
    }

    private static string ConvertLinks(string text) {
      if (string.IsNullOrEmpty(text)) {
        return text;
      }
      string result = ConvertCRefs(text);
      result = ConvertHRefs(result);
      return result;
    }

    private static string ConvertCRefs(string text) {
      string result = text;
      string searchPattern = "<see cref=\"";
      int currentStartIndex = text.IndexOf(searchPattern);
      while (currentStartIndex >= 0) {
        int currentEndIndex = text.IndexOf('"', currentStartIndex + searchPattern.Length);
        int linkLength = currentEndIndex - (currentStartIndex + searchPattern.Length);
        string linkNameFull = text.Substring(currentStartIndex + searchPattern.Length, linkLength);
        string linkName = linkNameFull;
        int lastIndexOfDot = linkNameFull.LastIndexOf(".");
        if (lastIndexOfDot >= 0) {
          linkName = linkNameFull.Substring(lastIndexOfDot + 1);
        }
        string linkFull = text.Substring(currentStartIndex, currentEndIndex - currentStartIndex + 4);
        string innerLink = $"Api.html?elementId={linkName}";
        string newLink = $"<a href=\"" + innerLink + "\">" + linkName + "</a>";
        result = text.Replace(linkFull, newLink);
        currentStartIndex = text.IndexOf(searchPattern, currentStartIndex + 1);
      }

      return result;
    }

    private static string ConvertHRefs(string text) {
      string result = text;
      string searchPattern = "<see href=\"";
      int currentStartIndex = text.IndexOf(searchPattern);
      while (currentStartIndex >= 0) {
        int currentEndIndex = text.IndexOf('"', currentStartIndex + searchPattern.Length);
        int linkLength = currentEndIndex - (currentStartIndex + searchPattern.Length);
        string linkNameFull = text.Substring(currentStartIndex + searchPattern.Length, linkLength);
        string linkName = linkNameFull;
        int lastIndexOfDot = linkNameFull.LastIndexOf(".");
        if (lastIndexOfDot >= 0) {
          linkName = linkNameFull.Substring(lastIndexOfDot + 1);
        }
        int firstIndedOfDot = linkNameFull.IndexOf(".");
        string pageName = linkNameFull;
        if (firstIndedOfDot >= 0) {
          pageName = linkNameFull.Substring(0, firstIndedOfDot);
        }
        string linkFull = text.Substring(currentStartIndex, currentEndIndex - currentStartIndex + 4);
        string innerLink = $"{pageName}.html?elementId={linkName}";
        string newLink = $"<a href=\"" + innerLink + "\">" + linkName + "</a>";
        result = text.Replace(linkFull, newLink);
        currentStartIndex = text.IndexOf(searchPattern, currentStartIndex + 1);
      }

      return result;
    }

    private static void CreateStaticFiles(string outputPath) {
      string navTreeCss = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.css.navtree.css");
      string styleCss = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.css.style.css");
      string treejsMinCss = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.css.treejs.min.css");

      string outputPathCss = Path.Combine(outputPath, "css");
      EnsureEmptyDirExists(outputPathCss);
      File.WriteAllText(Path.Combine(outputPathCss, "navtree.css"), navTreeCss);
      File.WriteAllText(Path.Combine(outputPathCss, "style.css"), styleCss);
      File.WriteAllText(Path.Combine(outputPathCss, "treejs.min.css"), treejsMinCss);

      string outputPathJs = Path.Combine(outputPath, "js");
      EnsureEmptyDirExists(outputPathJs);
      string navTreeJs = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.js.navtree.js");
      string treeMinJs = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.js.tree.min.js");
      File.WriteAllText(Path.Combine(outputPathJs, "navtree.js"), navTreeJs);
      File.WriteAllText(Path.Combine(outputPathJs, "tree.min.js"), treeMinJs);
    }

    private static async Task BuildPageAsync(
      string inputPath, string outputPath,
      TableOfContent toc,
      TableOfContent.Entry entry,
      DocuTree navTree,
      string frameworkName
    ) {
      string link = entry.Href;

      // navbar
      string navbarContent = BuildNavbarHtml(toc, frameworkName);

      // nav-tree + content
      string filename = Path.Combine(outputPath, link);
      filename += ".html";
      PageStructure pageStructure;
      if (entry.Href == "Api") {
        pageStructure = BuildPageStructureFromApi(navTree);
      } else {
        pageStructure = BuildPageStructureFromFolder(Path.Combine(inputPath, link));
      }
      StringBuilder content = new StringBuilder();
      string navTreeHtml = CreateNavTreeHtml(frameworkName);
      string navTreeJs = CreateNavTreeJsAndContentHtml(pageStructure.Nodes, content);

      // Create the final html
      string baseTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.base_template.html");
      string result = PlaceholderResolver.Resolve(
        baseTemplate,
        Rule.Get("navtreejs", navTreeJs),
        Rule.Get("navtree", navTreeHtml),
        Rule.Get("content", content.ToString()),
        Rule.Get("navbar", navbarContent)
      );
      if (File.Exists(filename)) {
        File.Delete(filename);
      }
      //while (!File.Exists(filename)) { }
      File.WriteAllText(filename, result);
    }

    private static string CreateNavTreeHtml(string frameworkName) {
      string navtreeTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navtree_template.html");

      return PlaceholderResolver.Resolve(
        navtreeTemplate,
        Rule.Get("title", frameworkName)
      );
    }

    private static string CreateNavTreeJsAndContentHtml(List<PageStructure.Node> nodes, StringBuilder contentItems) {
      string navtreeTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navtree_js_template.html"); 
      StringBuilder navTreeItems = new StringBuilder();
      foreach (PageStructure.Node node in nodes) {
        CreateNavTreeItem(node, navTreeItems, contentItems);
        string navTreeChildTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navtree_js_child_template.html"); 
        string childHtml = PlaceholderResolver.Resolve(
          navTreeChildTemplate,
          Rule.Get("parentVariable", "root"),
          Rule.Get("childVariable", node.Name)
        );
        navTreeItems.Append(childHtml);
      }

      return PlaceholderResolver.Resolve(
        navtreeTemplate,
        Rule.Get("items", navTreeItems.ToString())
      );
    }

    private static void CreateNavTreeItem(PageStructure.Node node, StringBuilder navTreeItems, StringBuilder contentItems) {
      string navTreeItemTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navtree_js_item_template.html");
      string navTreeItem = PlaceholderResolver.Resolve(
        navTreeItemTemplate,
        Rule.Get("nodeName", node.Name)
      );
      navTreeItems.Append(navTreeItem);

      foreach (PageStructure.Node childNode in node.Children) {
        CreateNavTreeItem(childNode, navTreeItems, contentItems);
        string navTreeChildTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navtree_js_child_template.html");
        string childHtml = PlaceholderResolver.Resolve(
          navTreeChildTemplate,
          Rule.Get("parentVariable", node.Name),
          Rule.Get("childVariable", childNode.Name)
        );
        navTreeItems.Append(childHtml);
      }

      string contentItemTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.content_entry_template.html"); 
      string content = node.Content;
      string contentEntry = PlaceholderResolver.Resolve(contentItemTemplate, Rule.Get("name", node.Name), Rule.Get("content", content));
      contentItems.Append(contentEntry);

    }

    private static PageStructure BuildPageStructureFromFolder(string path) {
      PageStructure result = new PageStructure();
      foreach (string file in Directory.EnumerateFiles(path, "*.md")) {
        string filename = Path.GetFileName(file);
        string fileContentMd = File.ReadAllText(file);
        string fileContentHtml = MdConverter.ConvertToHtml(fileContentMd);
        List<string> parts = filename.Split('.').ToList();
        parts.Remove(parts[parts.Count - 1]);

        result.InsertNode(parts, fileContentHtml);

      }
      return result;
    }

    private static PageStructure BuildPageStructureFromApi(DocuTree navTree) {
      PageStructure result = new PageStructure();
      result.Nodes = CreateChildren(navTree.RootNode.Children);
      return result;
    }

    private static List<PageStructure.Node> CreateChildren(List<DocuTreeNode> nodes) {
      List<PageStructure.Node> result = new List<PageStructure.Node>();
      foreach (DocuTreeNode node in nodes) {
        PageStructure.Node psNode = new PageStructure.Node() { Name = node.Identifier };
        if (node.Type == DocuTreeNodeType.Namespace) {
          psNode.Children = CreateChildren(node.Children);
        } else {
          psNode.Content = BuildContent(node);
        }
        result.Add(psNode);
      }
      return result;
    }

    private static string BuildContent(DocuTreeNode node) {
      StringBuilder result = new StringBuilder();
      BuildSummary(result, node);
      BuildPropertiesTable(result, node);
      BuildMethodsTable(result, node);
      return result.ToString();
    }

    private static void BuildSummary(StringBuilder result, DocuTreeNode node) {
      string summaryTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.summary_template.html");
      string summary = PlaceholderResolver.Resolve(summaryTemplate, Rule.Get("name", node.Identifier), Rule.Get("content", node.Summary));
      result.Append(summary);
    }

    private static string BuildNavbarHtml(TableOfContent toc, string frameworkName) {
      string navbarTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navbar_template.html");
      string navbarDropdownItemTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navbar_dropdownitem_template.html");
      string navbarDropdownItemEntryTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navbar_dropdownitem_entry_template.html");
      string navbarItemTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navbar_item_template.html");

      StringBuilder navbarItems = new StringBuilder();
      foreach (TableOfContent.Entry entry in toc.Entries) {
        if (entry.Items.Count == 0) {
          string navbarItem = PlaceholderResolver.Resolve(
            navbarItemTemplate, Rule.Get("text", entry.Text), Rule.Get("link", entry.Href + ".html")
          );
          navbarItems.Append(navbarItem);
        } else {
          StringBuilder navbarDropdownItemEntries = new StringBuilder();
          foreach (TableOfContent.Entry item in entry.Items) {
            navbarDropdownItemEntries.Append(
              PlaceholderResolver.Resolve(
                navbarDropdownItemEntryTemplate,
                Rule.Get("text", item.Text), Rule.Get("link", item.Href + ".html")
              )
            );
          }
          string navbarDropdownItem = PlaceholderResolver.Resolve(
            navbarDropdownItemTemplate, 
            Rule.Get("entries", navbarDropdownItemEntries.ToString()), 
            Rule.Get("text", entry.Text)
          );
          navbarItems.Append(navbarDropdownItem);
        }
      }
      return PlaceholderResolver.Resolve(
        navbarTemplate, 
        Rule.Get("items", navbarItems.ToString()),
        Rule.Get("name", frameworkName)
      );
    }

    private static TableOfContent ReadTableOfContent(string navbarFilepath) {
      TableOfContent result = new TableOfContent();
      using (var reader = new StreamReader(navbarFilepath)) {
        var yaml = new YamlStream();
        yaml.Load(reader);
        var x = 0;
        foreach (YamlDocument item in yaml) {
          YamlNode rootNode = item.RootNode;
          if (rootNode is YamlSequenceNode) {
            YamlSequenceNode yamlSequenceNode = (YamlSequenceNode)rootNode;
            foreach (var childNode in yamlSequenceNode.Children) {
              TableOfContent.Entry entry = GetTocEntry(childNode);
              result.Entries.Add(entry);
            }
          }
        }

      }
      return result;
    }

    private static TableOfContent.Entry GetTocEntry(YamlNode childNode) {
      TableOfContent.Entry result = new TableOfContent.Entry();
      YamlMappingNode keyValuePairs = (YamlMappingNode)childNode;
      result.Text = keyValuePairs.Where(
        x => x.Key.ToString() == "name"
      ).Select(x => x.Value.ToString()).Single();
      result.Href = keyValuePairs.Where(
        x => x.Key.ToString() == "href"
      ).Select(x => x.Value.ToString()).Single();
      YamlNode itemsNode = keyValuePairs.Where(x => x.Key.ToString() == "items").Select(x => x.Value).FirstOrDefault();
      if (!(itemsNode is null)) {
        YamlSequenceNode itemsList = (YamlSequenceNode)itemsNode;
        foreach (YamlNode item in itemsList) {
          result.Items.Add(GetTocEntry(item));
        }
      }
      return result;
    }

    private static void BuildFieldsTable(StringBuilder contentResult, DocuTreeNode node) {
      IEnumerable<DocuTreeNode> fieldNodes = node.Children.Where(x => x.Type == DocuTreeNodeType.Field);
      if (!fieldNodes.Any()) {
        return;
      }
      contentResult.AppendLine("<h4>Fields</h4>");
      contentResult.AppendLine("<table>");
      contentResult.AppendLine("<tbody>");
      foreach (DocuTreeNode propertyNode in fieldNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + propertyNode.Identifier + "</td>");
        string propertySummary = propertyNode.Summary;
        contentResult.AppendLine("<td>" + propertySummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</tbody>");
      contentResult.AppendLine("</table>");
    }

    private static void BuildPropertiesTable(StringBuilder contentResult, DocuTreeNode node) {
      IEnumerable<DocuTreeNode> propertyNodes = node.Children.Where(x => x.Type == DocuTreeNodeType.Property);
      if (!propertyNodes.Any()) {
        return;
      }
      contentResult.AppendLine("<h4>Properties</h4>");
      contentResult.AppendLine("<table class=\"table table-bordered table-striped table-dark\">");
      contentResult.AppendLine("<tbody>");
      foreach (DocuTreeNode propertyNode in propertyNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + propertyNode.Identifier + "</td>");
        string propertySummary = propertyNode.Summary;
        contentResult.AppendLine("<td>" + propertySummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</tbody>");
      contentResult.AppendLine("</table>");
    }

    private static void BuildMethodsTable(StringBuilder contentResult, DocuTreeNode node) {
      IEnumerable<DocuTreeNode> methodNodes = node.Children.Where(x => x.Type == DocuTreeNodeType.Method);
      if (!methodNodes.Any()) {
        return;
      }
      contentResult.AppendLine("<h4>Methods</h4>");
      contentResult.AppendLine("<table class=\"table table-bordered table-striped table-dark\">");
      contentResult.AppendLine("<tbody>");
      foreach (DocuTreeNode methodNode in methodNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + methodNode.Identifier + "</td>");
        string methodSummary = methodNode.Summary;
        contentResult.AppendLine("<td>" + methodSummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</tbody>");
      contentResult.AppendLine("</table>");
    }

  }
}
