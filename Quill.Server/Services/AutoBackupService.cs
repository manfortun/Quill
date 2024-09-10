using Microsoft.Win32.TaskScheduler;

namespace Quill.Server.Services;

public class AutoBackupService : BackupService
{
    private static readonly string TASK_NAME = "Quill-AutoBackup";

    public AutoBackupService(IConfiguration config) : base(config) { }

    public void ExecuteAutoBackup()
    {
        using (TaskService ts = new TaskService())
        {
            var existingTask = ts.FindTask(TASK_NAME);
            if (existingTask != null)
            {
                ts.RootFolder.DeleteTask(TASK_NAME);
            }

            TaskDefinition taskDefinition = ts.NewTask();
            taskDefinition.RegistrationInfo.Description = "Autobackup is executed when a user logs off.";

            taskDefinition.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.Today.AddHours(8) });

            taskDefinition.Settings.DisallowStartIfOnBatteries = false;
            taskDefinition.Settings.StopIfGoingOnBatteries = false;

            string powershell = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            string arguments = base.CreateScript();
            taskDefinition.Actions.Add(new ExecAction(powershell, arguments, null));

            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

            ts.RootFolder.RegisterTaskDefinition(TASK_NAME, taskDefinition);
        }
    }
}
