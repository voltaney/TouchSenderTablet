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

        var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
        return target.FileName.Render(logEventInfo);
    }
}
