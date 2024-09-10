namespace Quill.Server.Interfaces;

public interface IFile
{
    string Path { get; set; }
    DateTime LastWrite { get; set; }
}
