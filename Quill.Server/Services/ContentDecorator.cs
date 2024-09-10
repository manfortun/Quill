using HtmlAgilityPack;

namespace Quill.Server.Services;

public static class ContentDecorator
{
    public static string? SetHeaderIds(this string? content)
    {
        if (string.IsNullOrEmpty(content)) return content;

        HtmlDocument htmlDocument = content.AsHtmlDocument();

        foreach (var node in htmlDocument.DocumentNode.ChildNodes)
        {
            var type = node.GetHtmlType();

            if (type != HtmlExtension.HTML.Ignored && type != HtmlExtension.HTML.doc)
            {
                node.Id = Guid.NewGuid().ToString();
            }
        }

        return htmlDocument.DocumentNode.InnerHtml;
    }

    private static HtmlDocument AsHtmlDocument(this string content)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(content);
        return htmlDocument;
    }

    private static string Enclose(string component, string content, params string[] classes)
    {
        return $"<{component} {(classes.Length > 0 ? $"class=\"{string.Join(" ", classes)}\"" : "")}>{content}</{component}>";
    }
}
