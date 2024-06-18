using CommunityToolkit.Mvvm.ComponentModel;
using OutOfOffice.Models;

namespace OutOfOffice.ViewModels
{
    [QueryProperty("Review", "Review")]
    public partial class ReviewViewModel : ObservableObject
    {
        [ObservableProperty]
        Review? review;
    }
}
