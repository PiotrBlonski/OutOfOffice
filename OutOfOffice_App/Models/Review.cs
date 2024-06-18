namespace OutOfOffice.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Approver_Id { get; set; }
        public required string Approver { get; set; }
        public int LeaveRequest_Id { get; set; }
        public int Status { get; set; }
        public string? Comment { get; set; }
        public string StatusString => StatusToString(Status);
        public static string StatusToString(int Status)
        {
            return Status switch
            {
                1 => "Approved",
                2 => "Denied",
                _ => "Unknown",
            };
        }
    }
}
