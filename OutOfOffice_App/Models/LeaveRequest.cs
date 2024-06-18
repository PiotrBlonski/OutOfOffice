namespace OutOfOffice.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int Employee_Id { get; set; }
        public int Reason_Id { get; set; }
        public int ReasonId { get => Reason_Id - 1; set => Reason_Id = value + 1; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Comment { get; set; }
        public int Status { get; set; }
        public Review? Review { get; set; }
        public required string Owner { get; set; }
        public string Name { get => Owner; }
        public string Reason => Globals.Reason(ReasonId);
        public string StartDateOnly => "From: " + StartDate.ToShortDateString();
        public string EndDateOnly => "To: " + EndDate.ToShortDateString();
        public string StatusString => StatusToString(Status);
        public static string StatusToString(int Status)
        {
            return Status switch
            {
                1 => "Pending",
                2 => "Reviewed",
                3 => "Canceled",
                _ => "Unknown",
            };
        }
    }
}
