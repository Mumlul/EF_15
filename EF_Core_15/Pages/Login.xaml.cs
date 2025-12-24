using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using EF_Core_15.Extencion;

namespace EF_Core_15.Pages;

public partial class Login : Page,INotifyPropertyChanged
{
    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            if (SetField(ref _password, value))
            {
                IsPasswordCorrect = _password == "1234";
            }
        }
    }
    private bool _isAdmin = false;
    public bool IsAdmin { get => _isAdmin; set => SetField(ref _isAdmin, value); }
    private bool _isPasswordCorrect;
    public bool IsPasswordCorrect
    {
        get => _isPasswordCorrect;
        set => SetField(ref _isPasswordCorrect, value);
    }
    
    public Login()
    {
        InitializeComponent();
        DataContext = this;
        Application.Current.MainWindow.Title = "Вход";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void GoToMain(object obj, RoutedEventArgs e)
    {
        if(obj is Button btn)
            if (btn.Name != "ManagerButton")
                NavigationService.Navigate(new Main(false));
            else
                NavigationService.Navigate(new Main(true));
    }
    
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var pb = (PasswordBox)sender;
        PasswordBoxPasword.SetPassword(pb, pb.Password);
        /*Password = pb.Password;
        IsPasswordCorrect = Password == "1234";*/
        
    }
}