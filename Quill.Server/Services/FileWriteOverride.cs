namespace Quill.Server.Services;

public class FileWriteOverride : IDisposable
{
    private readonly string _filepath;
    public FileWriteOverride(string filepath)
    {
        _filepath = filepath;
        if (File.Exists(_filepath))
        {
            FileAttributes attributes = File.GetAttributes(_filepath);

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(filepath, attributes & ~FileAttributes.ReadOnly);
            }
        }
    }

    public void Dispose()
    {
        if (File.Exists(_filepath))
        {
            File.SetAttributes(_filepath, File.GetAttributes(_filepath) | FileAttributes.ReadOnly);
        }
    }
}
