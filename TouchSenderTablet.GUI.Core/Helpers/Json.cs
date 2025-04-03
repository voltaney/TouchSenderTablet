using Newtonsoft.Json;

namespace TouchSenderTablet.GUI.Core.Helpers;

public static class Json
{
    public static async Task<T> ToObjectAsync<T>(string value)
    {
        return await Task.Run<T>(() =>
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return default;
            }
        });
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run<string>(() =>
        {
            return JsonConvert.SerializeObject(value);
        });
    }
}
