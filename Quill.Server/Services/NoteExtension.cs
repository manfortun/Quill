using Quill.Server.Interfaces;

namespace Quill.Server.Services;

public static class NoteExtension
{
    private const string DATE_TIME_FORMAT = "MMM dd, yyyy hh:mm tt";
    public const string EXTENSION = "html";

    public static string GetLastWriteDateTimeString(this IFile note)
    {
        var duration = note.Duration();

        return duration.GetLastWriteDateTimeString(note.LastWrite);
    }

    public static string GetLastWriteDateTimeString(this TimeSpan duration, DateTime fallback)
    {
        if (duration.TotalSeconds < 60)
        {
            return "a few seconds ago";
        }
        else if (duration.TotalMinutes < 60)
        {
            return $"{(int)duration.TotalMinutes} {"minute".Plural((int)duration.TotalMinutes)} ago";
        }
        else if (duration.TotalHours < 24)
        {
            return $"{(int)duration.TotalHours} {"hour".Plural((int)duration.TotalHours)} ago";
        }
        else if (duration.TotalDays < 2)
        {
            return "yesterday";
        }
        else if (duration.TotalDays < 7)
        {
            int totalDays = (int)duration.TotalDays;
            return $"{totalDays} {"day".Plural(totalDays)} ago";
        }
        else
        {
            return fallback.ToString(DATE_TIME_FORMAT);
        }
    }

    public static TimeSpan Duration(this IFile note)
    {
        return DateTime.Now - note.LastWrite;
    }

    public static string TitleWithExtension(this string title)
    {
        return $"{title}.{EXTENSION}";
    }

    public static string Plural(this string @base, int @int, string? plural = null)
    {
        if (@int > 1)
        {
            return plural ?? $"{@base}s";
        }
        else
        {
            return @base;
        }
    }

    public static string AsIdentifer(this string filename)
    {
        return filename.ToLower().Replace(' ', '-');
    }

    public static string AsFileName(this string identifier)
    {
        return identifier.Replace('-', ' ');
    }

    public static string AsFileName(this string identifier, string directory, string extension)
    {
        identifier = identifier.AsFileName();
        string directoryName = Path.GetDirectoryName(Path.Combine(directory, identifier)) ?? Directory.GetCurrentDirectory();

        DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
        FileInfo[] files = dirInfo
            .GetFiles($"*.{extension}")
            .Where(f => string.Equals(f.Name, $"{identifier}.{extension}", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (files.Length > 0)
        {
            return Path.GetFileNameWithoutExtension(files[0].Name);
        }
        else
        {
            return identifier;
        }
    }
}
