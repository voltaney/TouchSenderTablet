using Windows.Storage;
using Windows.Storage.Search;

namespace TouchSenderTablet.GUI.Helpers;

public class LogFileHelper
{
    private static string NLogTargetName { get; } = "file";

    public static async Task<string?> FindLogFile()
    {
        var nlogPath = NLogHelper.GetFileTargetFileName(NLogTargetName)?.Replace('/', Path.DirectorySeparatorChar);
        if (File.Exists(nlogPath))
        {
            // 既にAppDataにフォルダがある場合は仮想化されないため
            // PackagedでもUnpackagedでもファイルが見つかった場合はパスを返す
            return nlogPath;
        }
        if (RuntimeHelper.IsMSIX)
        {
            // ファイル名のみを取得
            var logFileName = Path.GetFileName(nlogPath);
            if (string.IsNullOrEmpty(logFileName))
            {
                return null;
            }
            // .logファイルを検索
            var queryOpitons = new QueryOptions(CommonFileQuery.OrderByName, ["." + logFileName.Split(".")[^1]]);
            // ローカルキャッシュフォルダから検索
            var result = ApplicationData.Current.LocalCacheFolder.CreateFileQueryWithOptions(queryOpitons);
            // ファイルが見つかった場合はパスを返す
            return (await result.GetFilesAsync()).Where(f => f.Name.Equals(logFileName)).FirstOrDefault()?.Path;
        }
        else
        {
            return null;
        }
    }
}
