using CommunityToolkit.Maui.Core.Extensions;
using OutOfOffice.Models;
using OutOfOffice.ViewModels;
using System.Text.RegularExpressions;

namespace OutOfOffice.Pages;

public partial class EmployeeListPage : ContentPage
{
    private readonly EmployeeListViewModel viewmodel;
    public EmployeeListPage(EmployeeListViewModel vm)
    {
        BindingContext = vm;
        viewmodel = vm;
        InitializeComponent();
    }
    private void CollectionView_Loaded(object sender, EventArgs e)
    {
        viewmodel.Employees = Globals.User.GetEmployees().ToObservableCollection();
        viewmodel.CanEditEmployees = Globals.User.Permissions.CanEditEmployees;

        if (viewmodel.ComfinedPosition != "")
            viewmodel.Employees = viewmodel.Employees.Where(e => e.Position == viewmodel.ComfinedPosition).ToObservableCollection();

        if (viewmodel.IsSelecting)
        {
            if (!viewmodel.SortList.Contains("Selected"))
                viewmodel.SortList.Add("Selected");

            foreach (Employee ChoosenEmployee in viewmodel.ChoosenEmployees)
            {
                Employee? Employee = viewmodel.Employees.FirstOrDefault(e => e.Id == ChoosenEmployee.Id);

                if (Employee != null)
                    viewmodel.Employees[viewmodel.Employees.IndexOf(Employee)].IsSelected = true;
            }
        }

        viewmodel.FilteredEmployees = viewmodel.Employees;

        ApplyFilters(FindByName("FilterEntry"), null);
        UpdateSort();
    }
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void SortPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateSort();
    }

    private void UpdateSort()
    {
        string SelectedSort = Regex.Replace(viewmodel.SelectedSort, @"\s+", "");

        if (SelectedSort == "")
            return;

        viewmodel.SortedEmployees = viewmodel.FilteredEmployees
            .OrderBy(l => l.GetType().GetProperty(SelectedSort)?.GetValue(l))
            .ToObservableCollection();

        if (viewmodel.Descending)
            viewmodel.SortedEmployees = viewmodel.SortedEmployees.Reverse().ToObservableCollection();
    }

    private void OrderButtonClicked(object sender, EventArgs e)
    {
        viewmodel.Descending = !viewmodel.Descending;
        UpdateSort();
    }

    private void ApplyFilters(object sender, TextChangedEventArgs e)
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

        viewmodel.FilteredEmployees = viewmodel.Employees.Where(e =>
            (!FilterDictionary.ContainsKey("status") || e.StatusString.Contains(FilterDictionary["status"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("position") || e.Position.Contains(FilterDictionary["position"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("subdivision") || e.Subdivision.Contains(FilterDictionary["subdivision"], StringComparison.OrdinalIgnoreCase)) &&
            (!FilterDictionary.ContainsKey("name") || e.Name.Contains(FilterDictionary["name"], StringComparison.OrdinalIgnoreCase)) &&
            (!viewmodel.IsSelecting || (!FilterDictionary.TryGetValue("selected", out string? value) || (!bool.TryParse(value, out bool IsSelected) || IsSelected == e.IsSelected))))
            .ToObservableCollection();

        UpdateSort();
    }

    private async void ApplyButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", new Dictionary<string, object> { { "SelectedEmployees", viewmodel.Employees.Where(e => e.IsSelected).ToList() }, { "DidSelect", true } });
    }
}