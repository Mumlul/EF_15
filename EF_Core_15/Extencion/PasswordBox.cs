using System.Windows;
using System.Windows.Controls;

namespace EF_Core_15.Extencion;

public static class PasswordBoxPasword
{
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxPasword),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordChanged));

    public static string GetPassword(DependencyObject obj)
        => (string)obj.GetValue(PasswordProperty);

    public static void SetPassword(DependencyObject obj, string value)
        => obj.SetValue(PasswordProperty, value);

    private static void OnPasswordChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox passwordBox &&
            passwordBox.Password != (string)e.NewValue)
        {
            passwordBox.Password = (string)e.NewValue;
        }
    }
}