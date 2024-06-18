using OutOfOffice.ViewModels;

namespace OutOfOffice.Pages;

public partial class ReviewPage : ContentPage
{
    public ReviewPage(ReviewViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}