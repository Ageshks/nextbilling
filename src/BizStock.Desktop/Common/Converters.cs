using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BizStock.Desktop.Common;

/// <summary>
/// Shared value converters used by the views. Views never contain business logic;
/// these only translate view-model state into visual state.
/// </summary>
public static class Converters
{
    /// <summary>Shared instance so XAML can reference a single converter resource.</summary>
    public static readonly IValueConverter BoolToVisibility = new BoolToVisibilityConverter();

    /// <summary>Shared instance of the inverse boolean converter.</summary>
    public static readonly IValueConverter InverseBool = new InverseBooleanConverter();

    /// <summary>Shared instance of the null-to-visibility converter.</summary>
    public static readonly IValueConverter NullToVisibility = new NullToVisibilityConverter();

    /// <summary>Shared instance of the empty-string-to-visibility converter.</summary>
    public static readonly IValueConverter EmptyToVisibility = new EmptyStringToVisibilityConverter();

    /// <summary>Shared instance of the zero-to-visibility converter.</summary>
    public static readonly IValueConverter ZeroToVisibility = new ZeroToVisibilityConverter();
}

/// <summary>
/// Converts a boolean into a <see cref="Visibility"/>. Pass "Invert" as the parameter
/// to flip the result.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flag = value is true;
        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Visibility.Visible;
}

/// <summary>Inverts a boolean.</summary>
public sealed class InverseBooleanConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is not true;

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is not true;
}

/// <summary>Shows an element only when the bound value is not null. Pass "Invert" to flip.</summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isNull = value is null;
        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
        {
            isNull = !isNull;
        }

        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

/// <summary>Collapses an element when the bound string is null or whitespace.</summary>
public sealed class EmptyStringToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var text = value as string;
        var empty = string.IsNullOrWhiteSpace(text);
        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
        {
            empty = !empty;
        }

        return empty ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

/// <summary>Shows an element only while the bound count is greater than zero. Pass "Invert" to flip.</summary>
public sealed class ZeroToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = value is int i ? i : 0;
        var isZero = count <= 0;
        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
        {
            isZero = !isZero;
        }

        return isZero ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}