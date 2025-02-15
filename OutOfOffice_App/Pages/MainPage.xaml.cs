﻿using OutOfOffice.ViewModels;

namespace OutOfOffice.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel viewModel;
        public MainPage(MainViewModel vm)
        {
            BindingContext = vm;
            viewModel = vm;
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, EventArgs e)
        {
            viewModel.Employee = Globals.User.Employee;
            viewModel.CanViewEmployees = Globals.User.Permissions.CanViewEmployees;
            viewModel.CanViewProjects = Globals.User.Permissions.CanViewProjects;
            viewModel.CanViewAbsences = true;
            viewModel.UpdateProfile();

            if (FindByName("LoadingCover") is Grid Cover)
                Cover.IsVisible = false;
        }

        private async void OpenAbsences(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AbsencesListPage));
        }

        protected override bool OnBackButtonPressed() => true;

        private async void OpenProjects(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ProjectListPage));
        }

        private async void OpenEmployees(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EmployeeListPage));
        }
    }
}
