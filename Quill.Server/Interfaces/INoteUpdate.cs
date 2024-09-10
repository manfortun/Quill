namespace Quill.Server.Interfaces
{
    public interface INoteUpdate : INote
    {
        string? OldTitle { get; set; }
    }
}
