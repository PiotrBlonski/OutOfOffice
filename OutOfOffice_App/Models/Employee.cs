using CommunityToolkit.Mvvm.ComponentModel;

namespace OutOfOffice.Models
{
    public class Employee : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position_Id { get; set; }
        public int Subdivision_Id { get; set; }
        public int? Partner_Id { get; set; }
        public int PositionId { get => Position_Id - 1; set => Position_Id = value + 1; }
        public int SubdivisionId { get => Subdivision_Id - 1; set => Subdivision_Id = value + 1; }
        public int? PartnerId { get => Partner_Id - 1; set => Partner_Id = value + 1; }
        public int Status { get; set; }
        public int Balance { get; set; } 
        public string StatusString => StatusToString(Status);
        public string Position => Globals.Position(PositionId);
        public string Subdivision => Globals.Subdivision(SubdivisionId);
        public string Avatar => Globals.Address + "/employees/avatar/" + Id;
        public static string StatusToString(int Status)
        {
            return Status switch
            {
                1 => "Active",
                2 => "Inactive",
                _ => "Unknown",
            };
        }
        public bool CanSubmitRequests => Position == "Employee";
        public bool CanViewRequests => Position == "HR Manager" || Position == "Project Manager" || Position == "Administrator" || Position == "Employee";
        public bool CanEditRequests => Position == "HR Manager" || Position == "Project Manager";
        public bool CanViewReviews => Position == "HR Manager" || Position == "Project Manager" || Position == "Administrator" || Position == "Employee";
        public bool CanRemoveRequests => Position == "Administrator";
        public bool CanViewProjects => Position == "HR Manager" || Position == "Project Manager" || Position == "Administrator" || Position == "Employee";
        public bool CanEditProjects => Position == "Project Manager" || Position == "Administrator";
        public bool CanRemoveProjects => Position == "Administrator";
        public bool CanChangeManager => Position == "Administrator";
        public bool CanViewEmployees => Position == "HR Manager" || Position == "Project Manager" || Position == "Administrator";
        public bool CanEditEmployees => Position == "HR Manager" || Position == "Administrator";
        public bool CanEditPartner => Position == "Administrator";
        public bool CanChangePosition => Position == "Administrator";

        private bool isSelected;
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }
    }
}
