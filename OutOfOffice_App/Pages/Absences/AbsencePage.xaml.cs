using OutOfOffice.ViewModels;
namespace OutOfOffice.Pages;

public partial class AbsencePage : ContentPage
{
    private readonly AbsenceViewModel viewModel;
    public AbsencePage(AbsenceViewModel vm)
    {
        BindingContext = vm;
        viewModel = vm;
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void Picker_Loaded(object sender, EventArgs e)
    {
        viewModel.CanChangeStatus = Globals.User.Permissions.CanEditRequests && viewModel.LeaveRequest.Status < 2;
        viewModel.CanChangeDetails = Globals.User.Employee.Id == viewModel.LeaveRequest.Employee_Id && viewModel.LeaveRequest.Status < 2;
        viewModel.Editing = viewModel.LeaveRequest.Status != 0;
        viewModel.CanRemoveRequest = Globals.User.Permissions.CanRemoveRequests;
    }
}