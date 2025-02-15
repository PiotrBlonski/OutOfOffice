using CommunityToolkit.Maui.Core.Extensions;
using OutOfOffice.ViewModels;
using System.Text.RegularExpressions;

namespace OutOfOffice.Pages;

public partial class ProjectListPage : ContentPage
{
    private readonly ProjectListViewModel viewmodel;
    public ProjectListPage(ProjectListViewModel vm)
    {
        BindingContext = vm;
        viewmodel = vm;
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        viewmodel.Projects = Globals.User.GetProjects().ToObservableCollection();
        viewmodel.FilteredProjects = viewmodel.Projects;
        viewmodel.CanEditProjects = Globals.User.Permissions.CanEditProjects;

        ApplyFilters(FindByName("ProjectFilter"), true);
        Task.Run(UpdateSort);
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void SortPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Task.Run(UpdateSort);
    }

    private void UpdateSort()
    {
        string SelectedSort = Regex.Replace(viewmodel.SelectedSort, @"\s+", "");

        if (SelectedSort == "")
            return;

        viewmodel.SortedProjects = viewmodel.FilteredProjects
            .OrderBy(l => l.GetType().GetProperty(SelectedSort)?.GetValue(l))
            .ToObservableCollection();

        if (viewmodel.Descending)
            viewmodel.SortedProjects = viewmodel.SortedProjects.Reverse().ToObservableCollection();
    }

    private void OrderButtonClicked(object sender, EventArgs e)
    {
        viewmodel.Descending = !viewmodel.Descending;
        Task.Run(UpdateSort);
    }

    Dictionary<string, string> PreviousFilters = [];
    private void ApplyFilters(object sender, bool Reloaded = false)
    {
        string[] Filters = [];

        if (sender is Entry FilterEntry && FilterEntry.Text is string)
            Filters = FilterEntry.Text.Split(",");

        Dictionary<string, string> FilterDictionary = [];

        foreach (string Filter in Filters)
        {
            string[] SplitFilter = Filter.Split('=');

            if (SplitFilter.Length > 1 && SplitFilter[1].Trim() != "")
                FilterDictionary.Add(Regex.Replace(SplitFilter[0], @"\s+", ""), SplitFilter[1].Trim().ToLower());
        }

        if (Reloaded || FilterDictionary.Count != PreviousFilters.Count || !FilterDictionary.All(PreviousFilters.Contains))
        {
            viewmodel.FilteredProjects = viewmodel.Projects.Where(p =>
            (!FilterDictionary.ContainsKey("name") || p.Name.Contains(FilterDictionary["name"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("status") || p.StatusString.Contains(FilterDictionary["status"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("startdate") || p.StartDateOnly.Split(' ')[1].Contains(FilterDictionary["startdate"])) &&
            (!FilterDictionary.ContainsKey("enddate") || p.EndDateOnly.Split(' ')[1].Contains(FilterDictionary["enddate"])) &&
            (!FilterDictionary.ContainsKey("type") || p.ProjectType.Contains(FilterDictionary["type"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("manager") || p.Manager.Contains(FilterDictionary["manager"], StringComparison.OrdinalIgnoreCase)))
            .ToObservableCollection();

            UpdateSort();
        }

        PreviousFilters = FilterDictionary;
    }

    private void ProjectFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        Task.Run(() => ApplyFilters(sender));
    }
}