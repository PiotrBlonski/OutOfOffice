namespace OutOfOffice.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectType_Id { get; set; }
        public int ProjectTypeId { get => ProjectType_Id - 1; set => ProjectType_Id = value + 1; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Manager_Id { get; set; }
        public string? Comment { get; set; }
        public int ClientStatus { get; set; }
        public int Status { get => ClientStatus; set => ClientStatus = value - 1; }
        public string Manager => Globals.Employee(Manager_Id).Name;
        public string ProjectType => Globals.ProjectType(ProjectTypeId);
        public string StatusString => StatusToString(ClientStatus);
        public string StartDateOnly => "From: " + StartDate.ToShortDateString();
        public string EndDateOnly => "To: " + EndDate.ToShortDateString();
        public static string StatusToString(int Status)
        {
            return Status switch
            {
                0 => "Ongoing",
                1 => "Complete",
                2 => "Canceled",
                _ => "Unknown",
            };
        }
    }
}
