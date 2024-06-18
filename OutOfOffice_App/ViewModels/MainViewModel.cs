using CommunityToolkit.Mvvm.ComponentModel;
using OutOfOffice.Models;

namespace OutOfOffice.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        Employee employee = new();

        [ObservableProperty]
        bool canViewProjects;

        [ObservableProperty]
        bool canViewEmployees;

        [ObservableProperty]
        bool canViewAbsences;

        [ObservableProperty]
        string avatar;
    }
}
