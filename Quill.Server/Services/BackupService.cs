using System.Globalization;
using System.Management.Automation;

namespace Quill.Server.Services;

public class BackupService
{
    protected readonly string _scriptPath;
    protected readonly string _backupLocation;
    protected readonly string _backupSource;
    protected readonly string _backupLog;

    public BackupService(IConfiguration config)
    {
        _scriptPath = config["BackupScriptPath"] ?? throw new NotImplementedException("No backup script path set in application settings.");
        _backupLocation = config["BackupLocation"] ?? throw new NotImplementedException("No backup location set in application settings.");
        _backupSource = config["Directory"] ?? throw new NotImplementedException("No backup source set in application settings.");
        _backupLog = config["Logging:BackupLog"];
        if (!File.Exists(_scriptPath))
        {
            //this.Test();
            //throw new FileNotFoundException("The backup script file was either moved or deleted.");
        }
    }

    private void Test(string root = "/")
    {
        try
        {
            if (root.StartsWith("/proc") || root.StartsWith("/sys")) return;
            string[] directories = Directory.GetDirectories(root);
            string[] files = Directory.GetFiles(root);

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            foreach (var directory in directories)
            {
                Console.WriteLine(directory);
                this.Test(directory);
            }
        }
        catch (Exception ex) { }
    }

    protected string CreateScript()
    {
        return $"-NoProfile -ExecutionPolicy Bypass " +
            $"{this.CreateParameter("File", _scriptPath)} " +
            $"{this.CreateParameter(nameof(_backupSource), _backupSource)} " +
            $"{this.CreateParameter(nameof(_backupLocation), _backupLocation)} " +
            $"{(string.IsNullOrEmpty(_backupLog) ? string.Empty : this.CreateParameter(nameof(_backupLog), _backupLog))}";
    }

    private string CreateParameter(string name, object value)
    {
        return $"-{name} \"{value}\"";
    }

    public async Task<bool> BackupAsync()
    {
        using (PowerShell powerShell = PowerShell.Create())
        {
            string script = $"powershell {this.CreateScript()}";
            powerShell.AddScript(script);

            var results = await powerShell.InvokeAsync();

            if (results.Count > 0 && int.TryParse(results.LastOrDefault()?.ToString(), out int result))
            {
                return result == 1;
            }

            return false;
        }
    }

    public DateTime GetLastSuccessfulBackupDate()
    {
        if (!File.Exists(_backupLog)) return DateTime.MinValue;

        var logLines = File.ReadAllLines(_backupLog);

        var lastSuccessDate = logLines.LastOrDefault(line => line.Contains("Success"));

        if (string.IsNullOrEmpty(lastSuccessDate)) return DateTime.MinValue;

        return this.ExtractDate(lastSuccessDate);
    }

    private DateTime ExtractDate(string line)
    {
        var startIndex = 0;
        var endIndex = line.LastIndexOf(':');
        string date = line.Substring(startIndex, endIndex - startIndex);

        if (DateTime.TryParseExact(date, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        return DateTime.MinValue;
    }
}
