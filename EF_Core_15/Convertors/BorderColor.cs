using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EF_Core_15.Convertors;

public class BorderColor: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int stock && stock < 10)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffd129"));
        }
        
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}