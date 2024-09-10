using HtmlAgilityPack;

namespace Quill.Server.Services;

public static class HtmlExtension
{
    public enum HTML
    {
        Ignored = 0,
        h4 = 1,
        h3 = 2,
        h2 = 3,
        h1 = 4,
        doc = 5,
    }

    public static HTML GetHtmlType(this HtmlNode node)
    {
        foreach (var htmlType in Enum.GetValues<HTML>())
        {
            if (htmlType.ToString() == node.Name)
            {
                return htmlType;
            }
        }

        return HTML.Ignored;
    }
}
