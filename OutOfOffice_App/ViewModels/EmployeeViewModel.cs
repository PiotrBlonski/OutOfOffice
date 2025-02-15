﻿using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OutOfOffice.Models;
using System.Collections.ObjectModel;

namespace OutOfOffice.ViewModels
{
    [QueryProperty("Employee", "Employee")]
    public partial class EmployeeViewModel : ObservableObject
    {
        [ObservableProperty]
        Employee employee;

        [ObservableProperty]
        List<string> positionList = Globals.Positions ?? [];

        [ObservableProperty]
        List<string> subdivisionList = Globals.Subdivisions ?? [];

        [ObservableProperty]
        List<string> statusList = ["Active", "Inactive"];

        [ObservableProperty]
        int choosenPartner;

        [ObservableProperty]
        bool editing;

        [ObservableProperty]
        string login;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        ObservableCollection<Employee> partners;

        [ObservableProperty]
        ObservableCollection<string> partnerList;

        [ObservableProperty]
        bool canChangeDetails;

        [ObservableProperty]
        bool canChangePartner;

        [ObservableProperty]
        bool canChangeLoggingData;

        [ObservableProperty]
        bool canChangePosition;

        [ObservableProperty]
        string avatarPath = "";

        [RelayCommand]
        async Task Submit()
        {
            bool DialogAnswer = await Shell.Current.DisplayAlert("Are you sure?", "Submit changes", "Yes", "Cancel");
            if (DialogAnswer)
            {
                Employee.Partner_Id = ChoosenPartner > 0 ? Partners[ChoosenPartner - 1].Id : null;

                HttpResponseMessage Response;
                
                Response = Editing ? Globals.User.EditEmployee(Employee) : Globals.User.CreateEmployee(Employee, Login, Password);

                if (Response.IsSuccessStatusCode)
                {
                    if (AvatarPath != "" && !Editing)
                        Globals.User.UploadAvatar(AvatarPath, int.Parse(Task.Run(Response.Content.ReadAsStringAsync).Result.Split(':')[1]));

                    await Shell.Current.GoToAsync("..");
                }
            }
        }

        [RelayCommand]
        public async Task UploadAvatar()
        {
            FileResult? Result = await FilePicker.Default.PickAsync();

            if (Result != null && (Result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) || Result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase)))
            {
                if (Editing)
                {
                    HttpResponseMessage Response = Globals.User.UploadAvatar(Result.FullPath, Employee.Id);
                    if (Response.IsSuccessStatusCode)
                    {
                        AvatarPath = Employee.Avatar;
                        OnPropertyChanged(nameof(AvatarPath));
                    }
                }
                else AvatarPath = Result.FullPath;
            }
            else if (Result != null) await Shell.Current.DisplayAlert("Error", "Incorrect file format only .jpg or .png files are compatible", "OK");
        }
    }
}
