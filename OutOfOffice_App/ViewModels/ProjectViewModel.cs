using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Models;
using OutOfOffice.Pages;
using System.Collections.ObjectModel;

namespace OutOfOffice.ViewModels
{
    [QueryProperty("Project", "Project")]
    [QueryProperty("SelectedEmployees", "SelectedEmployees")]
    [QueryProperty("DidSelect", "DidSelect")]
    public partial class ProjectViewModel : ObservableObject
    {
        [ObservableProperty]
        Project project = new();

        [ObservableProperty]
        bool canChangeDetails;

        [ObservableProperty]
        bool canChangeManager;

        [ObservableProperty]
        bool canRemoveProject;

        [ObservableProperty]
        bool editing;

        [ObservableProperty]
        bool didSelect;

        [ObservableProperty]
        string[] statusList = ["Ongoing", "Complete", "Canceled"];

        [ObservableProperty]
        List<Employee> managers = [];

        [ObservableProperty]
        List<Employee> employees = [];

        [ObservableProperty]
        ObservableCollection<Employee> assignedEmployees = [];

        [ObservableProperty]
        List<Employee> selectedEmployees = [];

        [ObservableProperty]
        int selectedManager;

        [ObservableProperty]
        int selectedEmployee;

        [ObservableProperty]
        List<string> projectTypes = Globals.ProjectTypes ?? [];

        [RelayCommand]
        public async Task Submit()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", (!Editing ? "Submit new" : "Edit") + " project", "Yes", "Cancel");
            if (DialogAnswer)
            {
                if (CanChangeManager)
                    Project.Manager_Id = Managers[SelectedManager].Id;

                var Response = Globals.User.SubmitProject(Project, Editing, CanChangeManager);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");

                if (!Editing)
                    foreach (var Employee in AssignedEmployees)
                        Globals.User.AssignEmployee(int.Parse(Task.Run(Response.Content.ReadAsStringAsync).Result.Split(':')[1]), Employee.Id);
            }
        }

        [RelayCommand]
        public async Task RemoveProject()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Remove project", "Yes", "Cancel");
            if (DialogAnswer)
            {
                var Response = Globals.User.RemoveProject(Project.Id);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task RemoveEmployee(Employee Employee)
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Unassign employee from this project", "Yes", "Cancel");

            if (DialogAnswer)
            {
                UnassignEmployee(Employee);
                AssignedEmployees.Remove(Employee);
            }
        }

        [RelayCommand]
        async Task ChangeAssignedEmployees(Employee Employee)
        {
            await Shell.Current.GoToAsync($"{nameof(EmployeeListPage)}", new Dictionary<string, object> { { "SelectedEmployees", AssignedEmployees.ToList() }, { "IsSelecting", true }, { "ComfinedPosition", "Employee" } });
        }

        public void UnassignEmployee(Employee Employee)
        {
            var Response = Globals.User.UnassignEmployee(Project.Id, Employee.Id);

            if (Response.IsSuccessStatusCode)
                AssignedEmployees.Remove(Employee);
        }

        public void AssignEmployee(Employee Employee)
        {
            var Response = Globals.User.AssignEmployee(Project.Id, Employee.Id);

            if (Response.IsSuccessStatusCode)
                AssignedEmployees.Add(Employee);
        }
    }
}
