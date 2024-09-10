using static Quill.Server.Services.HtmlExtension;

namespace Quill.Server.DTOs;

public class TOCLayer
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public HTML Level { get; set; } = default!;
    public List<TOCLayer> Children { get; set; } = default!;

    public void AddChild(TOCLayer child)
    {
        if (Children?.Any() != true)
        {
            Children = new List<TOCLayer>() { child };
        }
        else if (child.Level < Children.Last().Level)
        {
            Children.Last().AddChild(child);
        }
        else
        {
            Children.Add(child);
        }
    }
}
