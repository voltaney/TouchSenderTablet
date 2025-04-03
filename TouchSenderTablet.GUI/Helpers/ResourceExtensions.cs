using Microsoft.Windows.ApplicationModel.Resources;

namespace TouchSenderTablet.GUI.Helpers;

public static class ResourceExtensions
{
    // Default Resources.resw
    private static readonly ResourceLoader s_resourceLoader = new();

    // Unpackagedだと毎回MakePri.exeを使ってresources.priファイルを生成する必要があるので、今回はなし
    // https://learn.microsoft.com/ja-jp/windows/apps/windows-app-sdk/mrtcore/localize-strings#loading-strings-in-unpackaged-applications
    //private static readonly ResourceLoader s_resourceLoaderForErrorMessages = new("ErrorMessages");

    public static string GetLocalized(this string resourceKey) => s_resourceLoader.GetString(resourceKey);
}
