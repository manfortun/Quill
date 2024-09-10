using Quill.Server.Interfaces;
using Quill.Server.Models;

namespace Quill.Server.Services;

public static class HypermediaHelper
{
    public static List<Link> GenerateLinksForNotes(INote note, string baseUrl)
    {
        string identifier = note.Title.AsIdentifer();
        return new List<Link>
        {
            new Self()
            {
                BaseUrl = baseUrl,
                Route = "Notes",
                Url = identifier,
            },
            new Delete()
            {
                BaseUrl= baseUrl,
                Route = "Notes",
                Url = identifier,
            },
            new Link()
            {
                BaseUrl = baseUrl,
                Route = "Contents",
                Url = identifier,
                Method = "GET",
                Ref = "toc"
            }
        };
    }
}
