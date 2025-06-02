using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AudioVisualizer.Converters;

public class DictContainsConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var dict = values[0] as IDictionary;
        var key = values[1] as string;
        if (dict == null || key == null)
        {
            return false;
        }
        return dict.Contains(key);
    }
}
