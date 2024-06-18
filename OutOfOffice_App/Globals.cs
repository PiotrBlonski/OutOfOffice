using OutOfOffice.Models;
using OutOfOffice.Web;

namespace OutOfOffice
{
    public static class Globals
    {
        public static string Address = "http://localhost:3000";
        public static User User = new();
        public static List<string>? Reasons { get => User.GetReasons(); }
        public static List<string>? ProjectTypes { get => User.GetProjectTypes(); }
        public static List<string>? Positions { get => User.GetPositions(); }
        public static List<string>? Subdivisions { get => User.GetSubdivisions(); }
        public static string Reason(int Reason) => (Reasons != null && Reason > -1 && Reason < Reasons.Count) ? Reasons[Reason] : "";
        public static string ProjectType(int Type) => (ProjectTypes != null && Type > -1 && Type < ProjectTypes.Count) ? ProjectTypes[Type] : "";
        public static string Position(int Position) => (Positions != null && Position > -1 && Position < Positions.Count) ? Positions[Position] : "";
        public static string Subdivision(int Subdivision) => (Subdivisions != null && Subdivision > -1 && Subdivision < Subdivisions.Count) ? Subdivisions[Subdivision] : "";
        public static Employee Employee(int Id) => Id == User.Employee.Id ? User.Employee : User.GetEmployee(Id);
    }
}
