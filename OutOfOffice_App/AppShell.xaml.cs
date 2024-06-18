using OutOfOffice.Pages;

namespace OutOfOffice
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ShellPage), typeof(ShellPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AbsencesListPage), typeof(AbsencesListPage));
            Routing.RegisterRoute(nameof(AbsencePage), typeof(AbsencePage));
            Routing.RegisterRoute(nameof(ReviewPage), typeof(ReviewPage));
            Routing.RegisterRoute(nameof(ProjectListPage), typeof(ProjectListPage));
            Routing.RegisterRoute(nameof(ProjectPage), typeof(ProjectPage));
            Routing.RegisterRoute(nameof(EmployeeListPage), typeof(EmployeeListPage));
            Routing.RegisterRoute(nameof(EmployeePage), typeof(EmployeePage));
        }

        protected override bool OnBackButtonPressed() => base.OnBackButtonPressed();
    }
}
