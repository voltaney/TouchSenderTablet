using Microsoft.UI.Xaml.Data;

namespace TouchSenderTablet.GUI.Converters;

public partial class InvertedBooleanConverter : IValueConverter
{
    // ref: https://stackoverflow.com/questions/1039636/how-to-bind-inverse-boolean-properties-in-wpf
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return !(bool)value;
    }
}
