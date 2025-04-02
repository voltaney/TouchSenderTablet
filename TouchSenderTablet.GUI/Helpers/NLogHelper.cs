namespace TouchSenderTablet.GUI.Helpers;

using NLog;
using NLog.Targets;

public static class NLogHelper
{
    public static string? GetFileTargetFileName(string targetName)
    {
        var config = LogManager.Configuration;
        if (config == null)
        {
            return null;
        }

        var target = config.FindTargetByName<FileTarget>(targetName);
        if (target == null)
        {
            return null;
        }
        // https://stackoverflow.com/questions/11452645/how-to-get-path-of-current-target-file-using-nlog-in-runtime
        return target.FileName.Render(LogEventInfo.CreateNullEvent());
    }
}
