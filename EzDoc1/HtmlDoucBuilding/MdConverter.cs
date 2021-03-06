
using Markdig;

namespace EzDoc.HtmlDoucBuilding {
  internal class MdConverter {
    public static string ConvertToHtml(string fileContentMd) {
      MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
      var r = Markdig.Markdown.ToHtml(fileContentMd, pipeline);
      return r;
    }
  }
}