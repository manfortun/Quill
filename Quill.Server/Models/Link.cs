namespace Quill.Server.Models;

public class Link
{
    public string Ref { get; set; } = default!;
    public string Href => $"{this.BaseUrl}/{this.Route}/{this.Url}";
    public string Method { get; set; } = default!;
    public string BaseUrl { get; set; } = default!;
    public string Route { get; set; } = default!;
    public string Url { get; set; } = default!;
}
