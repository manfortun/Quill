using Quill.Server.Interfaces;
using Quill.Server.Models;

namespace Quill.Server.DTOs;

public class ReadNoteDto : INote, IFile, IHypermedia
{
    public string Title { get; set; } = default!;
    public string? Content { get; set; }
    public string Path { get; set; } = default!;
    public DateTime LastWrite { get; set; }
    public string LastWriteString { get; set; } = default!;
    public List<Link> Links { get; set; } = default!;
}
