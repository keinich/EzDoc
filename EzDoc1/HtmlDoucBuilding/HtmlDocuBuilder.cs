using EzDoc.DocuGeneration;
using EzDoc.PlacehoderResolving;
using EzDoc1.HtmlDoucBuilding;
using Newtonsoft.Json;
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
      CreateStaticFiles(outputPath);

      string navbarFilename = "navbar_content.yaml";
      string navbarFilepath = System.IO.Path.Combine(path, navbarFilename);
      EnsureNavbarFileExists(navbarFilepath);
      TableOfContent toc = ReadTableOfContent(navbarFilepath);
      foreach (TableOfContent.Folder folder in toc.Folders) {
        await BuildPageAsync(path, outputPath, toc, folder, navTree, frameworkName);
      }

    }

    private static void EnsureNavbarFileExists(string navbarFilepath) {
      if (!File.Exists(navbarFilepath)) {
        string navbarDefault = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.navbar_default.yaml");
        File.WriteAllText(navbarFilepath, navbarDefault);
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
      result = ConvertEzLinks(result);
      return result;
    }

    private static string ConvertEzLinks(string text) {
      string result = text;
      string searchPattern = "[EzLink \"";
      int currentStartIndex = text.IndexOf(searchPattern);
      while (currentStartIndex >= 0) {
        int indexOfLinkStart = currentStartIndex + searchPattern.Length;
        int indexOfLinkEnd = text.IndexOf('"', indexOfLinkStart);
        int linkLength = indexOfLinkEnd - indexOfLinkStart;
        string patternFull = text.Substring(currentStartIndex, indexOfLinkEnd - currentStartIndex + 2);
        string linkFullOriginal = text.Substring(indexOfLinkStart, linkLength);
        int lastIndexOfDot = linkFullOriginal.LastIndexOf(".");
        if (lastIndexOfDot >= 0) {
          string site = linkFullOriginal.Substring(0, lastIndexOfDot);
          string elemendId = linkFullOriginal.Substring(
            lastIndexOfDot + 1, linkLength - lastIndexOfDot - 1
          );

          string innerLink = $"{site}.html?elementId={elemendId}";
          string newLink = $"<a href=\"" + innerLink + "\">" + elemendId + "</a>";
          result = result.Replace(patternFull, newLink);
          currentStartIndex = text.IndexOf(searchPattern, currentStartIndex + 1);
        } else {
          currentStartIndex = -1;
        }

      }

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
        int offset = 4;
        if (currentStartIndex + currentEndIndex - currentStartIndex + 4 > text.Length) {
          offset = 3;
        }
        string linkFull = text.Substring(currentStartIndex, currentEndIndex - currentStartIndex + offset);
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
      Utils.EnsureEmptyDirExists(outputPathCss);
      File.WriteAllText(Path.Combine(outputPathCss, "navtree.css"), navTreeCss);
      File.WriteAllText(Path.Combine(outputPathCss, "style.css"), styleCss);
      File.WriteAllText(Path.Combine(outputPathCss, "treejs.min.css"), treejsMinCss);

      string outputPathJs = Path.Combine(outputPath, "js");
      Utils.EnsureEmptyDirExists(outputPathJs);
      string navTreeJs = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.js.navtree.js");
      string treeMinJs = ResourceHelper.LoadResource("EzDoc1.HtmlStaticFiles.js.tree.min.js");
      File.WriteAllText(Path.Combine(outputPathJs, "navtree.js"), navTreeJs);
      File.WriteAllText(Path.Combine(outputPathJs, "tree.min.js"), treeMinJs);
    }

    private static async Task BuildPageAsync(
      string inputPath, string outputPath,
      TableOfContent toc,
      TableOfContent.Folder folder,
      DocuTree navTree,
      string frameworkName
    ) {
      string link = folder.FolderName;

      // navbar
      string navbarContent = BuildNavbarHtml(toc, frameworkName);

      // nav-tree + content
      string filename = Path.Combine(outputPath, link);
      filename += ".html";
      PageStructure pageStructure;
      if (folder.FolderName == "Api") {
        pageStructure = BuildPageStructureFromApi(navTree);
      } else {
        pageStructure = BuildPageStructureFromFolder(Path.Combine(inputPath, link), folder);
      }
      StringBuilder content = new StringBuilder();
      string navTreeHtml = CreateNavTreeHtml(folder.FolderName);
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
          Rule.Get("childVariable", node.Name.Replace(" ",""))
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
        Rule.Get("nodeName", node.Name.MakeCodeReady())
      );
      navTreeItems.Append(navTreeItem);

      foreach (PageStructure.Node childNode in node.Children) {
        CreateNavTreeItem(childNode, navTreeItems, contentItems);
        string navTreeChildTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.navtree_js_child_template.html");
        string childHtml = PlaceholderResolver.Resolve(
          navTreeChildTemplate,
          Rule.Get("parentVariable", node.Name.MakeCodeReady()),
          Rule.Get("childVariable", childNode.Name.MakeCodeReady())
        );
        navTreeItems.Append(childHtml);
      }

      string contentItemTemplate = ResourceHelper.LoadResource("EzDoc1.HtmlTemplates.content_entry_template.html");
      string content = node.Content;
      string contentEntry = PlaceholderResolver.Resolve(contentItemTemplate, Rule.Get("name", node.Name), Rule.Get("content", content));
      contentItems.Append(contentEntry);

    }

    private static PageStructure BuildPageStructureFromFolder(string path, TableOfContent.Folder folder) {
      PageStructure result = new PageStructure();
      FillPageStructurePages(new List<string>(), folder.Pages, path, result);
      foreach (TableOfContent.Node node in folder.Nodes) {
        List<string> parentNodes = new List<string>() { node.Name };
        FillPageStructure(parentNodes, node, path, result);
      }

      return result;
    }

    private static void FillPageStructure(
      List<string> parentNodes, TableOfContent.Node node, string path, PageStructure result) {
      FillPageStructurePages(parentNodes, node.Pages, path, result);
      foreach (TableOfContent.Node subNode in node.Nodes) {
        List<string> newNodes = new List<string>();
        foreach (string parentNode in parentNodes) {
          newNodes.Add(parentNode);
        }
        newNodes.Add(subNode.Name);
        FillPageStructure(newNodes, subNode, path, result);
      }
    }

    private static void FillPageStructurePages(List<string> parentNodes, List<string> pages, string path, PageStructure result) {
      foreach (string page in pages) {
        string filename = Path.Combine(path, page + ".md");
        string fileContentMd = File.ReadAllText(filename);
        fileContentMd = ConvertLinks(fileContentMd);
        string fileContentHtml = MdConverter.ConvertToHtml(fileContentMd);
        List<string> nodes = new List<string>();
        foreach (string parentNode in parentNodes) {
          nodes.Add(parentNode);
        }
        nodes.Add(Path.GetFileName(page));
        result.InsertNode(nodes, fileContentHtml);
      }
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
      foreach (TableOfContent.Folder folder in toc.Folders) {
        if (folder.Folders.Count == 0) {
          string navbarItem = PlaceholderResolver.Resolve(
            navbarItemTemplate, Rule.Get("text", folder.TabName), Rule.Get("link", folder.FolderName + ".html")
          );
          navbarItems.Append(navbarItem);
        } else {
          StringBuilder navbarDropdownItemEntries = new StringBuilder();
          foreach (TableOfContent.Folder subFolder in folder.Folders) {
            navbarDropdownItemEntries.Append(
              PlaceholderResolver.Resolve(
                navbarDropdownItemEntryTemplate,
                Rule.Get("text", subFolder.TabName), Rule.Get("link", subFolder.FolderName + ".html")
              )
            );
          }
          string navbarDropdownItem = PlaceholderResolver.Resolve(
            navbarDropdownItemTemplate,
            Rule.Get("entries", navbarDropdownItemEntries.ToString()),
            Rule.Get("text", folder.TabName)
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
      using (var reader = new StreamReader(navbarFilepath, Encoding.UTF7)) {
        var yaml = new YamlStream();
        yaml.Load(reader);
        var x = 0;
        foreach (YamlDocument item in yaml) {
          YamlNode rootNode = item.RootNode;
          if (rootNode is YamlSequenceNode) {
            YamlSequenceNode yamlSequenceNode = (YamlSequenceNode)rootNode;
            foreach (var childNode in yamlSequenceNode.Children) {
              TableOfContent.Folder folder = GetTocFolder(childNode);
              result.Folders.Add(folder);
            }
          }
        }

      }
      return result;
    }

    private static TableOfContent.Folder GetTocFolder(YamlNode childNode) {
      TableOfContent.Folder result = new TableOfContent.Folder();
      YamlMappingNode keyValuePairs = (YamlMappingNode)childNode;
      result.TabName = keyValuePairs.Where(
        x => x.Key.ToString() == "name"
      ).Select(x => x.Value.ToString()).Single();
      result.FolderName = keyValuePairs.Where(
        x => x.Key.ToString() == "folder"
      ).Select(x => x.Value.ToString()).Single();

      // Pages
      YamlNode pagesNode = keyValuePairs.Where(
        x => x.Key.ToString() == "pages"
      ).Select(x => x.Value).FirstOrDefault();
      if (!(pagesNode is null)) {
        YamlSequenceNode pagesList = (YamlSequenceNode)pagesNode;
        foreach (string pageNode in pagesList) {
          result.Pages.Add(pageNode);
        }
      }

      // Nodes
      YamlNode nodesNode = keyValuePairs.Where(
        x => x.Key.ToString() == "nodes"
      ).Select(x => x.Value).FirstOrDefault();
      if (!(nodesNode is null)) {
        YamlSequenceNode nodesList = (YamlSequenceNode)nodesNode;
        foreach (YamlNode nodeNode in nodesList) {
          result.Nodes.Add(GetTocNode(nodeNode));
        }
      }
      return result;
    }

    private static TableOfContent.Node GetTocNode(YamlNode childNode) {
      TableOfContent.Node result = new TableOfContent.Node();
      YamlMappingNode keyValuePairs = (YamlMappingNode)childNode;
      result.Name = keyValuePairs.Where(
        x => x.Key.ToString() == "name"
      ).Select(x => x.Value.ToString()).Single();

      // Pages
      YamlNode pagesNode = keyValuePairs.Where(
        x => x.Key.ToString() == "pages"
      ).Select(x => x.Value).FirstOrDefault();
      if (!(pagesNode is null)) {
        YamlSequenceNode pagesList = (YamlSequenceNode)pagesNode;
        foreach (string pageNode in pagesList) {
          result.Pages.Add(pageNode);
        }
      }

      // Nodes
      YamlNode nodesNode = keyValuePairs.Where(
        x => x.Key.ToString() == "nodes"
      ).Select(x => x.Value).FirstOrDefault();
      if (!(nodesNode is null)) {
        YamlSequenceNode nodesList = (YamlSequenceNode)nodesNode;
        foreach (YamlNode nodeNode in nodesList) {
          result.Nodes.Add(GetTocNode(nodeNode));
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
      contentResult.AppendLine("<table style=\"width: 70vw; \">");

      contentResult.AppendLine("<tr>");
      contentResult.AppendLine($"<th>Name</th>");
      contentResult.AppendLine($"<th>Beschreibung</th>");
      contentResult.AppendLine("</tr>");
      foreach (DocuTreeNode propertyNode in propertyNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + propertyNode.Identifier + "</td>");
        string propertySummary = propertyNode.Summary;
        contentResult.AppendLine("<td>" + propertySummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</table>");
    }

    private static void BuildMethodsTable(StringBuilder contentResult, DocuTreeNode node) {
      IEnumerable<DocuTreeNode> methodNodes = node.Children.Where(x => x.Type == DocuTreeNodeType.Method);
      if (!methodNodes.Any()) {
        return;
      }
      contentResult.AppendLine("<h4>Methods</h4>");
      contentResult.AppendLine("<table style=\"width: 70vw; \">");

      contentResult.AppendLine("<tr>");
      contentResult.AppendLine($"<th>Name</th>");
      contentResult.AppendLine($"<th>Beschreibung</th>");
      contentResult.AppendLine("</tr>");

      foreach (DocuTreeNode methodNode in methodNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + methodNode.Identifier + "</td>");
        string methodSummary = methodNode.Summary;
        contentResult.AppendLine("<td>" + methodSummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</table>");
    }

  }
}
