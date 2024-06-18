using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Models;
using OutOfOffice.Pages;
using System.Collections.ObjectModel;

namespace OutOfOffice.ViewModels
{
    public partial class AbsencesListViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<LeaveRequest> leaveRequests = [];

        [ObservableProperty]
        ObservableCollection<Review> reviews = [];

        [ObservableProperty]
        ObservableCollection<LeaveRequest> sortedLeaveRequests = [];

        [ObservableProperty]
        ObservableCollection<LeaveRequest> filteredLeaveRequests = [];

        [ObservableProperty]
        ObservableCollection<string> sortList = ["None", "Name", "Start Date", "End Date", "Status", "Reason"];

        [ObservableProperty]
        string selectedSort = "";

        [ObservableProperty]
        int selectedSortIndex = 4;

        [ObservableProperty]
        bool descending;

        [ObservableProperty]
        bool canViewApproval;

        [ObservableProperty]
        bool canSubmitLeaveRequest;

        [RelayCommand]
        async Task RequestLeave()
        {
            await OpenLeave(new() { Owner = Globals.User.Employee.Name, Employee_Id = Globals.User.Employee.Id, StartDate = DateTime.Today, EndDate = DateTime.Today });
        }

        [RelayCommand]
        async Task OpenLeave(LeaveRequest LeaveRequest)
        {
            await Shell.Current.GoToAsync($"{nameof(AbsencePage)}", new Dictionary<string, object> { { "LeaveRequest", LeaveRequest } });
        }

        [RelayCommand]
        async Task OpenApproval(Review Review)
        {
            await Shell.Current.GoToAsync($"{nameof(ReviewPage)}", new Dictionary<string, object> { { "Review", Review } });
        }
    }
}
