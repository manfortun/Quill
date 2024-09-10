using HtmlAgilityPack;
using Quill.Server.DTOs;
using Quill.Server.Interfaces;
using static Quill.Server.Services.HtmlExtension;

namespace Quill.Server.Services;

public class TableOfContentService
{

    public TOCLayer GetTOC(INote note)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(note.Content);

        // create first layer, which is the document
        TOCLayer toc = new TOCLayer
        {
            Title = note.Title,
            Level = HTML.doc,
            Children = new List<TOCLayer>()
        };

        foreach (var childNode in htmlDoc.DocumentNode.ChildNodes)
        {
            if (childNode is null) continue;

            HTML level = childNode.GetHtmlType();
            if (level != HTML.Ignored)
            {
                var newToc = new TOCLayer
                {
                    Title = childNode.InnerText,
                    Id = childNode.Id,
                    Level = level,
                    Children = new List<TOCLayer>()
                };

                toc.AddChild(newToc);
            }

        }

        return toc;
    }
}
