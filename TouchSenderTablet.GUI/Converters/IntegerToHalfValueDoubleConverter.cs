using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace TouchSenderTablet.GUI.Converters;

public partial class IntegerToHalfValueDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int intValue)
        {
            return (double)intValue / 2;
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
