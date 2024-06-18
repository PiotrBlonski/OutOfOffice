using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using OutOfOffice.Pages;
using OutOfOffice.ViewModels;

namespace OutOfOffice
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<ShellPage>()
                            .AddSingleton<MainPage>()
                            .AddSingleton<MainViewModel>()
                            .AddSingleton<LoginPage>()
                            .AddSingleton<LoginViewModel>()
                            .AddSingleton<AbsencesListPage>()
                            .AddSingleton<AbsencesListViewModel>()
                            .AddSingleton<ProjectListPage>()
                            .AddSingleton<ProjectListViewModel>()
                            .AddTransient<ProjectPage>()
                            .AddTransient<ProjectViewModel>()
                            .AddTransient<AbsencePage>()
                            .AddTransient<AbsenceViewModel>()
                            .AddTransient<ReviewPage>()
                            .AddTransient<ReviewViewModel>()
                            .AddTransient<EmployeeListPage>()
                            .AddTransient<EmployeeListViewModel>()
                            .AddTransient<EmployeePage>()
                            .AddTransient<EmployeeViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
