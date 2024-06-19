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
        private bool isSelected;
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }
        public bool Selected => !IsSelected;
    }
}
