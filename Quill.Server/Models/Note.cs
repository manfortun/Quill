using Quill.Server.Interfaces;

namespace Quill.Server.Models;

public class Note : INote, IFile
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public required string Path { get; set; }
    public required DateTime LastWrite { get; set; }
}
