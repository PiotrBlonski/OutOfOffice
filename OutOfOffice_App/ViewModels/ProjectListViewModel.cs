using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Models;
using OutOfOffice.Pages;
using System.Collections.ObjectModel;

namespace OutOfOffice.ViewModels
{
    public partial class ProjectListViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Project> projects = [];

        [ObservableProperty]
        ObservableCollection<Project> sortedProjects = [];

        [ObservableProperty]
        ObservableCollection<Project> filteredProjects = [];

        [ObservableProperty]
        ObservableCollection<string> sortList = ["None", "Name", "Manager", "Project Type", "Start Date", "End Date", "Status"];

        [ObservableProperty]
        string selectedSort = "";

        [ObservableProperty]
        int selectedSortIndex = 3;

        [ObservableProperty]
        bool descending;

        [ObservableProperty]
        bool canEditProjects;

        [RelayCommand]
        async Task CreateProject()
        {
            await OpenProject(new() { StartDate = DateTime.Today.ToUniversalTime(), EndDate = DateTime.Today.ToUniversalTime() + new TimeSpan(24, 0, 0) });
        }

        [RelayCommand]
        async Task OpenProject(Project Project)
        {
            await Shell.Current.GoToAsync($"{nameof(ProjectPage)}", new Dictionary<string, object> { { "Project", Project } });
        }
    }
}
