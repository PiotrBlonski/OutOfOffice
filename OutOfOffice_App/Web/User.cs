using OutOfOffice.Models;
using System.Text.Json;

namespace OutOfOffice.Web
{
    public class User
    {
        private readonly Client UserClient;
        public Employee Employee { get; set; }
        public User()
        {
            UserClient = new(Globals.Address);
            Employee = new();
        }

        public HttpResponseMessage LogIn(string Login, string Password)
        {
            HttpResponseMessage Response = UserClient.SetToken(Login, Password);

            if (GetUserData() is Employee Employee && Response.IsSuccessStatusCode)
                this.Employee = Employee;

            return Response;
        }

        public Employee GetUserData()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/employees/me", true);
            return Response.IsSuccessStatusCode && Client.GetData<Employee>(Response) is Employee Employee ? Employee : new();
        }

        public List<LeaveRequest> GetLeaveRequests()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/requests/leave", true);
            return Response.IsSuccessStatusCode && Client.GetData<List<LeaveRequest>>(Response) is List<LeaveRequest> Requests ? Requests : [];
        }

        public List<Review> GetReviews()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/requests/approval", true);
            return Response.IsSuccessStatusCode && Client.GetData<List<Review>>(Response) is List<Review> Reviews ? Reviews : [];
        }

        public HttpResponseMessage SubmitRequest(LeaveRequest LeaveRequest, bool Editing)
        {
            var Data = new Dictionary<string, string>
            {
                { "reason", (LeaveRequest.Reason_Id).ToString() },
                { "startdate", LeaveRequest.StartDate.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")},
                { "enddate", LeaveRequest.EndDate.Date.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                { "comment", LeaveRequest.Comment ?? "" },
            };

            HttpResponseMessage Response;

            if (Editing)
            {
                Data.Add("Id", LeaveRequest.Id.ToString());
                Response = UserClient.SendRequest(HttpMethod.Post, "/requests/leave/edit", true, Data);
            }
            else Response = UserClient.SendRequest(HttpMethod.Post, "/requests/leave/submit", true, Data);

            return Response;
        }

        public HttpResponseMessage CancelRequest(int Id)
        {
            var Data = new Dictionary<string, string>
            {
                { "Id", Id.ToString() }
            };
            return UserClient.SendRequest(HttpMethod.Post, "/requests/leave/cancel", true, Data);
        }

        public HttpResponseMessage RemoveRequest(int Id)
        {
            var Data = new Dictionary<string, string>
            {
                { "Id", Id.ToString() }
            };
            return UserClient.SendRequest(HttpMethod.Post, "/requests/leave/remove", true, Data);
        }

        public HttpResponseMessage ChangeStatusOfRequest(LeaveRequest LeaveRequest, bool Approved, string Comment)
        {
            var Data = new Dictionary<string, string>
            {
                { "leaveid", LeaveRequest.Id.ToString() },
                { "status", Approved ? "1" : "2" },
                { "comment", Comment },
            };
            return UserClient.SendRequest(HttpMethod.Post, "/requests/leave/changestatus", true, Data);
        }

        public List<string> GetReasons()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/requests/reasons", false);

            List<string> Data = [];

            if (Response.IsSuccessStatusCode)
                Data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Response.Content.ReadAsStream())?.SelectMany(d => d.Values).ToList() ?? [];

            return Data;

        }

        public List<string> GetProjectTypes()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/projects/types", true);

            List<string> Data = [];

            if (Response.IsSuccessStatusCode)
                Data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Response.Content.ReadAsStream())?.SelectMany(d => d.Values).ToList() ?? [];

            return Data;
        }

        public List<string> GetPositions()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/employees/positions", true);

            List<string> Data = [];

            if (Response.IsSuccessStatusCode)
                Data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Response.Content.ReadAsStream())?.SelectMany(d => d.Values).ToList() ?? [];

            return Data;
        }

        public List<string> GetSubdivisions()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/employees/subdivisions", true);

            List<string> Data = [];

            if (Response.IsSuccessStatusCode)
                Data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(Response.Content.ReadAsStream())?.SelectMany(d => d.Values).ToList() ?? [];

            return Data;
        }

        public List<Project> GetProjects()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/projects", true);
            return Response.IsSuccessStatusCode && Client.GetData<List<Project>>(Response) is List<Project> Projects ? Projects : [];
        }

        public HttpResponseMessage SubmitProject(Project Project, bool Editing, bool CanChangeManager)
        {
            var Data = new Dictionary<string, string>
            {
                { "name", Project.Name },
                { "projecttype", (Project.ProjectType_Id).ToString() },
                { "startdate", Project.StartDate.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")},
                { "enddate", Project.EndDate.Date.ToString("yyyy-MM-dd HH:mm:ss.fff")},
                { "comment", Project.Comment ?? "" },
                { "status", (Project.ClientStatus + 1).ToString() }
            };

            HttpResponseMessage Response;

            if (CanChangeManager)
                Data.Add("manager", Project.Manager_Id.ToString());

            if (Editing)
            {
                Data.Add("projectid", Project.Id.ToString());
                Response = UserClient.SendRequest(HttpMethod.Post, "/projects/edit", true, Data);
            }
            else Response = UserClient.SendRequest(HttpMethod.Post, "/projects/submit", true, Data);

            return Response;
        }

        public HttpResponseMessage RemoveProject(int Id)
        {
            var Data = new Dictionary<string, string>
            {
                { "id", Id.ToString() },
            };
            return UserClient.SendRequest(HttpMethod.Post, "/projects/remove", true, Data);
        }

        public HttpResponseMessage EditEmployee(Employee Employee)
        {
            var Data = new Dictionary<string, string>
            {
                { "id", Employee.Id.ToString() },
                { "name", Employee.Name },
                { "subdivision", Employee.Subdivision_Id.ToString()},
                { "status", (Employee.Status + 1).ToString() },
                { "balance", Employee.Balance.ToString() },
                { "partner", this.Employee.CanEditPartner ? Employee.Partner_Id.ToString() : this.Employee.Id.ToString() }
            };

            if (this.Employee.CanChangePosition)
                Data.Add("position", Employee.Position_Id.ToString());

            return UserClient.SendRequest(HttpMethod.Post, "/employees/edit", true, Data);
        }

        public HttpResponseMessage CreateEmployee(Employee Employee, string Login, string Password)
        {
            var Data = new Dictionary<string, string>
            {
                { "name", Employee.Name },
                { "subdivision", Employee.Subdivision_Id.ToString()},
                { "status", (Employee.Status + 1).ToString() },
                { "balance", Employee.Balance.ToString() },
                { "login", Login },
                { "password", Password },
            };

            if (this.Employee.CanChangePosition)
                Data.Add("position", Employee.Position_Id.ToString());

            if (Employee.Partner_Id > 0)
                Data.Add("partner", Employee.Partner_Id.ToString());

            return UserClient.SendRequest(HttpMethod.Post, "/user/create", true, Data);
        }

        public Employee GetEmployee(int Id)
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/employees/" + Id, true);
            return Response.IsSuccessStatusCode && Client.GetData<Employee>(Response) is Employee Employee ? Employee : new();
        }

        public List<Employee> GetEmployees()
        {
            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/employees", true);
            return Response.IsSuccessStatusCode && Client.GetData<List<Employee>>(Response) is List<Employee> Employees ? Employees : [];
        }

        public List<Employee> GetAssignedtEmployees(Project Project)
        {
            var Data = new Dictionary<string, string>
            {
                { "projectid", Project.Id.ToString() },
            };

            HttpResponseMessage Response = UserClient.SendRequest(HttpMethod.Get, "/projects/assigned", true, Data);

            return Response.IsSuccessStatusCode && Client.GetData<List<Employee>>(Response) is List<Employee> Employees ? Employees : [];
        }

        public HttpResponseMessage UnassignEmployee(int ProjectId, int EmployeeId)
        {
            var Data = new Dictionary<string, string>
            {
                { "employeeid", EmployeeId.ToString() },
                { "projectid", ProjectId.ToString() }
            };

            return UserClient.SendRequest(HttpMethod.Post, "/projects/unassign", true, Data);
        }

        public HttpResponseMessage AssignEmployee(int ProjectId, int EmployeeId)
        {
            var Data = new Dictionary<string, string>
            {
                { "employeeid", EmployeeId.ToString() },
                { "projectid", ProjectId.ToString() }
            };

            return UserClient.SendRequest(HttpMethod.Post, "/projects/assign", true, Data);
        }

        public HttpResponseMessage UploadAvatar(string FilePath, int Id)
        {
            return UserClient.SendRequest(HttpMethod.Post, "/employees/avatar/" + Id.ToString(), true, null, FilePath);
        }
    }
}
