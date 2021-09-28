
namespace EzDoc.HtmlDoucBuilding {
  internal class MdConverter {
    internal static string ConvertToHtml(string fileContentMd) {
      var r = Markdig.Markdown.ToHtml(fileContentMd);
      return r;
    }
  }
}