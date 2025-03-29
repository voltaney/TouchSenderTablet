
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TouchSenderTablet.GUI.Helpers;

internal class ElementThemeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ElementTheme theme)
        {
            return theme.ToString();
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string themeString)
        {
            if (Enum.TryParse(typeof(ElementTheme), themeString, out var theme))
            {
                return theme;
            }
        }
        return ElementTheme.Default;
    }
}
