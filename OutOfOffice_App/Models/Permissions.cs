namespace OutOfOffice.Models
{
    public class Permissions
    {
        public bool CanSubmitRequests { get; set;}
        public bool CanViewRequests { get; set; }
        public bool CanEditRequests { get; set; }
        public bool CanViewReviews { get; set; }
        public bool CanRemoveRequests { get; set; }
        public bool CanViewProjects { get; set; }
        public bool CanEditProjects { get; set; }
        public bool CanRemoveProjects { get; set; }
        public bool CanChangeManager { get; set; }
        public bool CanViewEmployees { get; set; }
        public bool CanEditEmployees { get; set; }
        public bool CanEditPartner { get; set; }
        public bool CanChangePosition { get; set; }
    }
}
