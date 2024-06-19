using CommunityToolkit.Maui.Core.Extensions;
using OutOfOffice.ViewModels;

namespace OutOfOffice.Pages;

public partial class EmployeePage : ContentPage
{
    private readonly EmployeeViewModel viewmodel;
    public EmployeePage(EmployeeViewModel vm)
    {
        BindingContext = vm;
        viewmodel = vm;
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void VerticalStackLayout_Loaded(object sender, EventArgs e)
    {
        viewmodel.CanChangeDetails = Globals.User.Employee.CanEditEmployees;
        viewmodel.Editing = viewmodel.Employee.Id > 0;
        viewmodel.CanChangePartner = Globals.User.Employee.CanEditPartner;
        viewmodel.CanChangePosition = Globals.User.Employee.CanChangePosition;
        viewmodel.AvatarPath = viewmodel.Employee.Avatar ?? "";

        if (Globals.User.Employee.CanEditPartner)
        {
            viewmodel.Partners = Globals.User.GetEmployees().Where(e => e.Position == "HR Manager").ToObservableCollection();
            viewmodel.PartnerList = viewmodel.Partners.Select(e => e.Name).ToObservableCollection();
            viewmodel.PartnerList.Insert(0, "None");
            viewmodel.ChoosenPartner = viewmodel.Employee.Partner_Id > 0 ? viewmodel.Partners.IndexOf(viewmodel.Partners.First(e => e.Id == viewmodel.Employee.Partner_Id)) + 1 : 0;
        }

        (FindByName("StatusPicker") as Picker).SelectedIndex -= 1;

    }
}