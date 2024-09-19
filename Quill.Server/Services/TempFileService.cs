using System.Text;

namespace Quill.Server.Services;

public class TempFileService
{
    private readonly string _tempFileLocation;

    public TempFileService(IConfiguration config)
    {
        _tempFileLocation = config["TempFileLocation"] ?? string.Empty;
    }

    public bool CheckTempFileExists(string filename)
    {
        string tempFilename = this.ToTempFileName(filename);

        return File.Exists(tempFilename);
    }

    public async Task<string?> GetContent(string filename)
    {
        string tempFilename = this.ToTempFileName(filename);

        if (!File.Exists(tempFilename))
        {
            return null;
        }

        using (FileStream fs = new FileStream(tempFilename, FileMode.Open, FileAccess.Read))
        {
            return await this.ReadAsync(fs);
        }
    }

    public async Task<bool> CreateTempFileAsync(string fileName, string? content)
    {
        string tempFileName = this.ToTempFileName(fileName);

        using (FileStream fs = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            string prevContent = await this.ReadAsync(fs);

            if (prevContent != content)
            {
                await this.WriteAsync(fs, content);

                return true;
            }
        }

        return false;
    }

    public void DeleteTempFile(string fileName)
    {
        string tempFileName = this.ToTempFileName(fileName);

        if (File.Exists(tempFileName))
        {
            File.Delete(tempFileName);
        }
    }

    private string ToTempFileName(string fileName)
    {
        return Path.Combine(_tempFileLocation, $"_{fileName}.{NoteExtension.EXTENSION}.tmp");
    }

    private async Task<string> ReadAsync(FileStream fs)
    {
        StringBuilder content = new StringBuilder();
        using (StreamReader sr = new StreamReader(fs, leaveOpen: true))
        {
            while (!sr.EndOfStream)
            {
                content.Append(await sr.ReadLineAsync());
            }
        }

        return content.ToString();
    }

    private async Task WriteAsync(FileStream fs, string? content)
    {
        fs.Position = 0;
        fs.SetLength(0);

        using (StreamWriter sw = new StreamWriter(fs, leaveOpen: true))
        {
            await sw.WriteLineAsync(content);
        }
    }
}
