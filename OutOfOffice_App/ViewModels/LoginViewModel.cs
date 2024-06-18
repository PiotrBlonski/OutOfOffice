using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Pages;
using OutOfOffice.Web;

namespace OutOfOffice.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        string login = "";

        [ObservableProperty]
        string password = "";

        [ObservableProperty]
        string message = "";

        [RelayCommand]
        async Task LogIn()
        {
            User User = new();
            HttpResponseMessage Response = User.LogIn(Login, Password);
            if (Response.IsSuccessStatusCode)
            {
                Globals.User = User;
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
            else Message = await Response.Content.ReadAsStringAsync();
        }
    }
}
