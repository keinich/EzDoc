using EzDoc.DocuGeneration;
using EzDoc.PlacehoderResolving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace EzDoc.HtmlDoucBuilding {
  class HtmlDocuBuilder {

    public static async Task BuildHtmlDocu(string path, DocuTree navTree) {

      string navbarFilename = "EzDoc/navbar_content.yaml";
      string navbarFilepath = System.IO.Path.Combine(path, navbarFilename);
      path = Path.Combine(path, "EzDoc");
      string outputPath = Path.Combine(path, "_out");
      if (!Directory.Exists(outputPath)) {
        Directory.CreateDirectory(outputPath);
        while (!Directory.Exists(outputPath)) { }
      }
      else {
        Directory.Delete(outputPath, true);
        while (Directory.Exists(outputPath)) ;
        Directory.CreateDirectory(outputPath);
        while (!Directory.Exists(outputPath)) { }
      }
      CreateStaticFiles(outputPath);


      TableOfContent toc = ReadTableOfContent(navbarFilepath);
      string navbarContent = BuildNavbarContent(toc);
      foreach (TableOfContent.Entry entry in toc.Entries) {
        if (entry.Items.Count == 0) {
          await BuildPageAsync(path, outputPath, navbarContent, entry, navTree);
        }
        else {
          foreach (TableOfContent.Entry item in entry.Items) {
            await BuildPageAsync(path, outputPath, navbarContent, item, navTree);
          }
        }
      }

      StringBuilder navStuff = new StringBuilder();
      StringBuilder contentStuff = new StringBuilder();
      BuildNavAndContentHtml(navTree, navStuff, contentStuff);

      StringBuilder result = new StringBuilder();
      BuildFinalHtml(navStuff, contentStuff, result);
      await File.WriteAllTextAsync("Result.html", result.ToString());
    }

    private static void CreateStaticFiles(string outputPath) {
      File.WriteAllText(Path.Combine(outputPath,"navtree.css"), Properties.Resources.navtree_css);
      File.WriteAllText(Path.Combine(outputPath, "navtree.js"), Properties.Resources.navtree_js);
    }

    private static async Task BuildPageAsync(
      string inputPath, string outputPath,
      string navbarContent,
      TableOfContent.Entry entry,
      DocuTree navTree
    ) {
      string link = entry.Href;

      // Create the nav-tree
      string filename = Path.Combine(outputPath, link);
      filename += ".html";
      PageStructure pageStructure;
      if (entry.Href == "Api") {
        pageStructure = BuildPageStructureFromApi(Path.Combine(inputPath, link), navTree);
      }
      else {
        pageStructure = BuildPageStructureFromFolder(Path.Combine(inputPath, link));
      }
      StringBuilder contentItems = new StringBuilder();
      string navTreeHtml = CreateNavTreeAndContents(pageStructure.Nodes, contentItems);
      navTreeHtml = PlaceholderResolver.Resolve(navTreeHtml, Rule.Get("title", entry.Text));

      string contentTemplate = Properties.Resources.content_template;
      string content = PlaceholderResolver.Resolve(contentTemplate, Rule.Get("items", contentItems.ToString()));

      // Create the final html
      string baseTemplate = Properties.Resources.base_template;
      string body = navbarContent + navTreeHtml + content;
      string result = PlaceholderResolver.Resolve(baseTemplate, Rule.Get("body", body));
      if (File.Exists(filename)) {
        File.Delete(filename);
      }
      //while (!File.Exists(filename)) { }
      await File.WriteAllTextAsync(filename, result);
    }

    private static string CreateNavTreeAndContents(List<PageStructure.Node> nodes, StringBuilder contentItems) {
      string navtreeTemplate = Properties.Resources.navtree_template;
      StringBuilder navTreeItems = CreateNavTreeItems(nodes, contentItems);

      return PlaceholderResolver.Resolve(
        navtreeTemplate, 
        Rule.Get("items", navTreeItems.ToString())
      );
    }

    private static StringBuilder CreateNavTreeItems(List<PageStructure.Node> nodes, StringBuilder contentItems) {
      StringBuilder navTreeItems = new StringBuilder();
      foreach (PageStructure.Node node in nodes) {
        if (node.IsLeaf()) {
          string navTreeContentLinkTemplate = Properties.Resources.navtree_contentLink_template;
          string navTreeContentLink = PlaceholderResolver.Resolve(navTreeContentLinkTemplate, Rule.Get("name", node.Name));
          navTreeItems.Append(navTreeContentLink);
          string contentItemTemplate = Properties.Resources.content_entry_template;
          string content = node.Content;
          string contentEntry = PlaceholderResolver.Resolve(contentItemTemplate, Rule.Get("name", node.Name), Rule.Get("content", content));
          contentItems.Append(contentEntry);
        }
        else {
          string navTreeTogglerTemplate = Properties.Resources.navtree_toggler_template;
          string children = CreateNavTreeItems(node.Children, contentItems).ToString();
          string navTreeToggler = PlaceholderResolver.Resolve(
            navTreeTogglerTemplate, Rule.Get("name", node.Name), Rule.Get("children", children)
          );
          navTreeItems.Append(navTreeToggler);
        }
      }

      return navTreeItems;
    }

    private static PageStructure BuildPageStructureFromFolder(string path) {
      PageStructure result = new PageStructure();
      foreach (string file in Directory.EnumerateFiles(path, "*.md")) {
        string filename = Path.GetFileName(file);
        string fileContentMd = File.ReadAllText(file);
        string fileContentHtml = MdConverter.ConvertToHtml(fileContentMd);
        IEnumerable<string> parts = filename.Split(".").SkipLast(1);

        result.InsertNode(parts, fileContentHtml);

      }
      return result;
    }

    private static PageStructure BuildPageStructureFromApi(string v, DocuTree navTree) {
      PageStructure result = new PageStructure();
      return result;
    }

    private static string BuildNavbarContent(TableOfContent toc) {
      string navbarTemplate = EzDoc.Properties.Resources.navbar_template;
      string navbarDropdownItemTemplate = Properties.Resources.navbar_dropdownitem_template;
      string navbarDropdownItemEntryTemplate = Properties.Resources.navbar_dropdownitem_entry_template;
      string navbarItemTemplate = EzDoc.Properties.Resources.navbar_item_template;
      StringBuilder navbarItems = new StringBuilder();
      foreach (TableOfContent.Entry entry in toc.Entries) {
        if (entry.Items.Count == 0) {
          string navbarItem = PlaceholderResolver.Resolve(
            navbarItemTemplate, Rule.Get("text", entry.Text), Rule.Get("link", entry.Href + ".html")
          );
          navbarItems.Append(navbarItem);
        }
        else {
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
            navbarDropdownItemTemplate, Rule.Get("entries", navbarDropdownItemEntries.ToString()), Rule.Get("text", entry.Text)
          );
          navbarItems.Append(navbarDropdownItem);
        }
      }
      return PlaceholderResolver.Resolve(navbarTemplate, Rule.Get("items", navbarItems.ToString()));
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
      if (itemsNode is not null) {
        YamlSequenceNode itemsList = (YamlSequenceNode)itemsNode;
        foreach (YamlNode item in itemsList) {
          result.Items.Add(GetTocEntry(item));
        }
      }
      return result;
    }

    private static void BuildNavAndContentHtml(
      DocuTree navTree, StringBuilder navStuff, StringBuilder contentStuff
    ) {
      BuildNavAndContentHtml(
        navTree.RootNode.Children, navStuff, contentStuff, navTree
      );
    }



    private static void BuildNavAndContentHtml(List<DocuTreeNode> nodes, StringBuilder navResult, StringBuilder contentResult, DocuTree tree) {
      foreach (DocuTreeNode child in nodes.OrderBy(n => n.Children.Where(c => c.Type == DocuTreeNodeType.Namespace || c.Type == DocuTreeNodeType.Type).ToList().Count)) {
        if (child.Type == DocuTreeNodeType.Namespace) {
          navResult.AppendLine("<li><span class=\"caret\">" + child.Identifier + "</span>");
          navResult.AppendLine("<ul class=\"nested\">");
          BuildNavAndContentHtml(child.Children, navResult, contentResult, tree); // Recursion
          navResult.AppendLine("</ul>");
          navResult.AppendLine("</li>");
        }
        else if (child.Type == DocuTreeNodeType.Type) {
          navResult.AppendLine("<li id=\"" + child.Identifier + "-link\" class=\"content-link\">" + child.Identifier + "</li>");
          CreateContentDiv(contentResult, child, tree);
        }
      }
    }

    private static void BuildFinalHtml(StringBuilder navStuff, StringBuilder contentStuff, StringBuilder result) {
      result.AppendLine("<!DOCTYPE html>");
      result.AppendLine("<html>");
      result.AppendLine();
      result.AppendLine("<head>");
      result.AppendLine("<link rel=\"stylesheet\" href=\"style.css\">");
      result.AppendLine("</head>");
      result.AppendLine();
      result.AppendLine("<body>");
      result.AppendLine();
      result.AppendLine("<div class=\"treeview\">");
      result.AppendLine("<p class=\"mainhead\">Zusagenframework</p>");
      result.AppendLine("<hr>");
      result.AppendLine("<ul id=\"myUL\">");
      result.Append(navStuff);
      result.AppendLine("</ul>");
      result.AppendLine();
      result.AppendLine("</div>");

      result.AppendLine("<div class=\"content-container\">");
      result.Append(contentStuff);
      result.AppendLine("</div>");

      result.AppendLine("<script src=\"program.js\"></script>");
      result.AppendLine("</body>");
      result.AppendLine();
      result.AppendLine("</html>");

    }


    private static void CreateContentDiv(StringBuilder contentResult, DocuTreeNode node, DocuTree tree) {
      contentResult.AppendLine("<div id=\"" + node.Identifier + "\" class=\"content\">");
      contentResult.AppendLine("<h3>" + node.Identifier + "</h3>");
      contentResult.AppendLine("<hr>");
      contentResult.AppendLine("<h4>Description</h4>");
      string nodeSummary = CreateSummary(node.Summary, tree);
      contentResult.AppendLine(nodeSummary);
      BuildPropertiesTable(contentResult, node, tree);
      BuildFieldsTable(contentResult, node, tree);
      BuildMethodsTable(contentResult, node, tree);
      contentResult.AppendLine("</div>");
    }

    private static void BuildFieldsTable(StringBuilder contentResult, DocuTreeNode node, DocuTree tree) {
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
        string propertySummary = CreateSummary(propertyNode.Summary, tree);
        contentResult.AppendLine("<td>" + propertySummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</tbody>");
      contentResult.AppendLine("</table>");
    }

    private static void BuildPropertiesTable(StringBuilder contentResult, DocuTreeNode node, DocuTree tree) {
      IEnumerable<DocuTreeNode> propertyNodes = node.Children.Where(x => x.Type == DocuTreeNodeType.Property);
      if (!propertyNodes.Any()) {
        return;
      }
      contentResult.AppendLine("<h4>Properties</h4>");
      contentResult.AppendLine("<table>");
      contentResult.AppendLine("<tbody>");
      foreach (DocuTreeNode propertyNode in propertyNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + propertyNode.Identifier + "</td>");
        string propertySummary = CreateSummary(propertyNode.Summary, tree);
        contentResult.AppendLine("<td>" + propertySummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</tbody>");
      contentResult.AppendLine("</table>");
    }

    private static string CreateSummary(string summary, DocuTree tree) {
      DocuTreeNode rootNode = tree.RootNode;
      Dictionary<string, string> replacements = new Dictionary<string, string>();
      CreateSummary1(ref summary, rootNode, replacements);
      foreach (KeyValuePair<string, string> replacement in replacements) {
        summary = summary.Replace(replacement.Key, replacement.Value);
      }
      return summary;
    }

    private static void CreateSummary1(ref string summary, DocuTreeNode rootNode, Dictionary<string, string> replacements) {
      if (summary is null) {
        return;
      }
      foreach (DocuTreeNode node in rootNode.Children) {
        if (node.Type == DocuTreeNodeType.Type) {
          bool containsIdentifier = summary.Contains(node.Identifier);
          if (containsIdentifier) {
            if (!replacements.Keys.Any(r => r.Contains(node.Identifier))) {
              replacements.Add(node.Identifier, "<p class=\"content-link-inline\">" + node.Identifier + "</p>");
            }
            IEnumerable<string> smallerOnes = replacements.Where(r => node.Identifier != r.Key && node.Identifier.Contains(r.Key)).Select(r => r.Key);
            foreach (string smallerOne in smallerOnes) {
              replacements.Remove(smallerOne);
            }
          }
        }
        CreateSummary1(ref summary, node, replacements);
      }
    }

    private static void BuildMethodsTable(StringBuilder contentResult, DocuTreeNode node, DocuTree tree) {
      IEnumerable<DocuTreeNode> methodNodes = node.Children.Where(x => x.Type == DocuTreeNodeType.Method);
      if (!methodNodes.Any()) {
        return;
      }
      contentResult.AppendLine("<h4>Methods</h4>");
      contentResult.AppendLine("<table>");
      contentResult.AppendLine("<tbody>");
      foreach (DocuTreeNode methodNode in methodNodes) {
        contentResult.AppendLine("<tr>");
        contentResult.AppendLine("<td>" + methodNode.Identifier + "</td>");
        string methodSummary = CreateSummary(methodNode.Summary, tree);
        contentResult.AppendLine("<td>" + methodSummary + "</td>");
        contentResult.AppendLine("</tr>");
      }
      contentResult.AppendLine("</tbody>");
      contentResult.AppendLine("</table>");
    }

  }
}
