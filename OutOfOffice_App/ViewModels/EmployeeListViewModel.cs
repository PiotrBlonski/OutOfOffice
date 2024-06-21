using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Models;
using OutOfOffice.Pages;
using System.Collections.ObjectModel;

namespace OutOfOffice.ViewModels
{
    [QueryProperty("IsSelecting", "IsSelecting")]
    [QueryProperty("SelectedEmployees", "SelectedEmployees")]
    [QueryProperty("ComfinedPosition", "ComfinedPosition")]
    public partial class EmployeeListViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Employee> employees = [];

        [ObservableProperty]
        ObservableCollection<Employee> sortedEmployees = [];

        [ObservableProperty]
        ObservableCollection<Employee> filteredEmployees = [];

        [ObservableProperty]
        List<Employee> selectedEmployees = [];

        [ObservableProperty]
        ObservableCollection<string> sortList = ["None", "Name", "Position", "Subdivision", "Status"];

        [ObservableProperty]
        string comfinedPosition = "";

        [ObservableProperty]
        string selectedSort = "";

        [ObservableProperty]
        int selectedSortIndex = 4;

        [ObservableProperty]
        bool descending;

        [ObservableProperty]
        bool isSelecting;

        [ObservableProperty]
        bool canEditEmployees;

        [RelayCommand]
        void ChooseUser(Employee Employee)
        {
            if (IsSelecting)
            {
                bool IsSelected = Employee.IsSelected;
                Employee.IsSelected = !IsSelected;
                Employees[Employees.IndexOf(Employee)].IsSelected = !IsSelected;
            }
            else OpenUser(Employee);
        }

        [RelayCommand]
        void CreateUser()
        {
            OpenUser(new() { Status = 1, Balance = 240 });
        }

        async void OpenUser(Employee Employee)
        {
            await Shell.Current.GoToAsync($"{nameof(EmployeePage)}", new Dictionary<string, object> { { "Employee", Employee } });
        }
    }
}
