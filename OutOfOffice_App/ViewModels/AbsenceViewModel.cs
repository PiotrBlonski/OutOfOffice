using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Models;

namespace OutOfOffice.ViewModels
{
    [QueryProperty("LeaveRequest", "LeaveRequest")]
    public partial class AbsenceViewModel : ObservableObject
    {
        [ObservableProperty]
        LeaveRequest? leaveRequest;

        [ObservableProperty]
        bool canChangeStatus;

        [ObservableProperty]
        bool canChangeDetails;

        [ObservableProperty]
        List<string> reasons = Globals.Reasons ?? [];

        [ObservableProperty]
        bool editing;

        [ObservableProperty]
        bool canRemoveRequest;

        [ObservableProperty]
        string statusComment = "";

        [RelayCommand]
        async Task Submit()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Submit changes", "Yes", "Cancel");
            if (DialogAnswer)
            {
                if (LeaveRequest.ReasonId == -1)
                {
                    await Shell.Current.DisplayAlert("Error", "Please provide reason!", "OK");
                    return;
                }

                if (LeaveRequest.StartDate.Date < DateTime.Now.Date)
                {
                    await Shell.Current.DisplayAlert("Error", "Starting date cannot be set in past!", "OK");
                    return;
                }

                if (LeaveRequest.EndDate.Date < LeaveRequest.StartDate.Date)
                {
                    await Shell.Current.DisplayAlert("Error", "Ending date cannot be set before starting date!", "OK");
                    return;
                }

                var Response = Globals.User.SubmitRequest(LeaveRequest, Editing);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");
            }
        }
        [RelayCommand]
        async Task CancelRequest()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Cancel request", "Yes", "Cancel");
            if (DialogAnswer && LeaveRequest != null)
            {
                var Response = Globals.User.CancelRequest(LeaveRequest.Id);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task DenyRequest()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Deny request", "Yes", "Cancel");
            if (DialogAnswer && LeaveRequest != null)
            {
                var Response = Globals.User.ChangeStatusOfRequest(LeaveRequest, false, StatusComment);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task ApproveRequest()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Approve request", "Yes", "Cancel");
            if (DialogAnswer && LeaveRequest != null)
            {
                var Response = Globals.User.ChangeStatusOfRequest(LeaveRequest, true, StatusComment);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task RemoveRequest()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Remove request", "Yes", "Cancel");
            if (DialogAnswer && LeaveRequest != null)
            {
                var Response = Globals.User.RemoveRequest(LeaveRequest.Id);

                if (Response.IsSuccessStatusCode)
                    await Shell.Current.GoToAsync("..");
            }
        }
    }
}
