namespace OutOfOffice.Pages;

public partial class ShellPage : ContentPage
{
    public ShellPage()
    {
        InitializeComponent();
    }

    private async void Grid_Loaded(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync(new LoginPage(new()));
        Shell.Current.Navigation.RemovePage(this);
    }
}