using CommunityToolkit.Maui.Core.Extensions;
using OutOfOffice.Models;
using OutOfOffice.ViewModels;

namespace OutOfOffice.Pages;

public partial class ProjectPage : ContentPage
{
    private readonly ProjectViewModel viewModel;
    public ProjectPage(ProjectViewModel vm)
    {
        BindingContext = vm;
        viewModel = vm;
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void ManagerPicker_Loaded(object sender, EventArgs e)
    {
        viewModel.CanChangeDetails = Globals.User.Permissions.CanEditProjects;
        viewModel.CanChangeManager = Globals.User.Permissions.CanChangeManager;
        viewModel.Editing = viewModel.Project.Manager_Id != 0;
        viewModel.CanRemoveProject = Globals.User.Permissions.CanRemoveProjects;

        List<Employee> Employees = Globals.User.GetEmployees();
        viewModel.Managers = Employees.Where(e => e.Position == "Project Manager").ToList();
        viewModel.Employees = Employees.Where(e => e.Position == "Employee").ToList();

        if (viewModel.Managers.FirstOrDefault(m => m.Id == viewModel.Project.Manager_Id) is Employee Manager)
            viewModel.SelectedManager = viewModel.Managers.IndexOf(Manager);

        if (viewModel.Editing)
            viewModel.AssignedEmployees = Globals.User.GetAssignedtEmployees(viewModel.Project).ToObservableCollection();

        if (viewModel.DidSelect)
        {
            if (viewModel.Editing)
            {
                viewModel.AssignedEmployees.Where(assigned => !viewModel.SelectedEmployees.Any(selected => assigned.Id == selected.Id)).ToList().ForEach(e => viewModel.UnassignEmployee(e));
                viewModel.SelectedEmployees.Where(selected => !viewModel.AssignedEmployees.Any(assigned => assigned.Id == selected.Id)).ToList().ForEach(e => viewModel.AssignEmployee(e));
            }
            viewModel.AssignedEmployees = viewModel.SelectedEmployees.ToObservableCollection();
        }

        if (sender is Picker ManagerPicker)
        {
            ManagerPicker.ItemsSource = viewModel.Managers.Select(m => m.Name).ToList();
            ManagerPicker.SelectedIndex = viewModel.SelectedManager;
        }
    }
}