using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DirectoryTreeExplorer.Presentation.Converters
{
    /// <summary>
    /// Represents a converter to convert collection of strings to string.
    /// </summary>
    [ValueConversion(typeof(IEnumerable<string>), typeof(string))]
    public sealed class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string.");

            if (!(value is IEnumerable<string> valueList))
                throw new InvalidOperationException("The value must be a collection of strings.");

            return string.Join("\n", valueList);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
