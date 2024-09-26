using Microsoft.VisualBasic.FileIO;
using Quill.Server.Interfaces;
using Quill.Server.Models;
using Quill.Server.Services;

namespace Quill.Server.DataAccess;

public class NoteRepository
{
    private readonly ILogger<NoteRepository> _logger;
    public readonly string BaseDirectory;

    public NoteRepository(IConfiguration config, ILogger<NoteRepository> logger)
    {
        _logger = logger;
        BaseDirectory = config.GetValue<string>("Directory") ?? throw new InvalidOperationException("No directory specified in application settings.");
    }

    public virtual IEnumerable<Note> GetAll(bool includeContent = true)
    {
        _logger.LogTrace("Obtaining all notes.");
        string[] identifiers = this.GetExistingIdentifiers();
        _logger.LogTrace("Found {0} notes.", identifiers.Length);

        foreach (string identifier in identifiers)
        {
            Note? note = this.Get(identifier, includeContent);

            if (note is not null)
            {
                yield return note;
            }
        }
    }

    public virtual Note? Get(string identifier, bool includeContent = true)
    {
        string filepath = this.GetFilePath(identifier.AsFileName(BaseDirectory, NoteExtension.EXTENSION));
        _logger.LogTrace("Obtaining {0}", filepath);

        if (!File.Exists(filepath))
        {
            _logger.LogError("File not exists.");
            return default!;
        }

        using (StreamReader reader = new StreamReader(filepath))
        {
            string content = includeContent ? reader.ReadToEnd() : string.Empty;
            Note note = new Note()
            {
                Title = Path.GetFileNameWithoutExtension(filepath),
                Content = content,
                LastWrite = File.GetLastWriteTime(filepath),
                Path = filepath,
            };

            _logger.LogTrace("File obtained.");
            return note;
        }
    }

    public virtual Enums.SaveResult Create(INote note, out string identifier)
    {
        _logger.LogTrace("Creating new note.");
        string filepath = this.GetFilePath(note.Title);
        identifier = note.Title.AsIdentifer();

        if (File.Exists(filepath))
        {
            return Enums.SaveResult.WithDuplicateTitle;
        }

        return this.Write(filepath, note.Content);
    }

    public virtual Enums.SaveResult Update(INoteUpdate note, out string identifier)
    {
        _logger.LogTrace("Updating note for {0}", note.Title);
        string oldFilepath = this.GetFilePath(note.OldTitle ?? note.Title);
        string newFilepath = this.GetFilePath(note.Title);

        return this.Update(oldFilepath, newFilepath, note.Content, out identifier);
    }

    public virtual Enums.SaveResult Update(string oldFilePath, string newFilePath, string? content, out string identifier)
    {
        identifier = Path.GetFileNameWithoutExtension(newFilePath).AsIdentifer();

        // the path does not exist
        if (!File.Exists(oldFilePath))
        {
            return Enums.SaveResult.FileNotExists;
        }

        if (oldFilePath != newFilePath)
        {
            // the filename was changed/updated, but the new filename is already existing
            if (File.Exists(newFilePath))
            {
                return Enums.SaveResult.WithDuplicateTitle;
            }
            else
            {
                string oldIdentifier = Path.GetFileNameWithoutExtension(oldFilePath).AsIdentifer();
                var result = this.Delete(oldIdentifier, hardDelete: true);

                if (result != Enums.SaveResult.Success)
                {
                    return result;
                }
            }
        }

        return this.Write(newFilePath, content);
    }

    public virtual Enums.SaveResult Delete(INote note)
    {
        string filepath = this.GetFilePath(note.Title);
        return this.Delete(filepath);
    }

    public virtual Enums.SaveResult Delete(string identifier, bool hardDelete = false)
    {
        _logger.LogTrace("Deleting note for {0}", identifier);
        string filepath = this.GetFilePath(identifier.AsFileName());

        if (!File.Exists(filepath)) return Enums.SaveResult.FileNotExists;

        try
        {
            using (FileWriteOverride fwo = new FileWriteOverride(filepath))
            {
                FileSystem.DeleteFile(filepath, UIOption.OnlyErrorDialogs, hardDelete ? RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin);
            }
        }
        catch (Exception)
        {
            return Enums.SaveResult.Failed;
        }

        return Enums.SaveResult.Success;
    }

    public virtual Enums.SaveResult Write(string filepath, string? content)
    {
        try
        {
            using (FileWriteOverride fwo = new FileWriteOverride(filepath))
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                string? appliedContent = content
                    .SetHeaderIds();

                writer.Write(appliedContent);
                _logger.LogTrace("File written in {0}", filepath);
            }
        }
        catch (Exception)
        {
            _logger.LogError("Unable to write file in {0}", filepath);
            return Enums.SaveResult.Failed;
        }

        return Enums.SaveResult.Success;
    }

    public virtual string GetFilePath(string title)
    {
        _logger.LogTrace("Test: {0}", title);
        return Path.Combine(BaseDirectory, title.TitleWithExtension());
    }

    public string[] GetExistingIdentifiers(string? extension = null)
    {
        string[] filepaths = Directory.GetFiles(BaseDirectory, $"*.{extension ?? NoteExtension.EXTENSION}");
        
        return filepaths
            .Select(path => Path.GetFileNameWithoutExtension(path).AsIdentifer())
            .ToArray();
    }
}
