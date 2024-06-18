using OutOfOffice.ViewModels;

namespace OutOfOffice.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}