using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModel;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Foxminded.Curriculum.App.ServiceProviderInitialization;

public static class ServiceCollectionInit
{
    public static IServiceCollection GetServicesTransient(this IServiceCollection service)
    {
        return service
            .AddLogging()

            .AddTransient<CourseService>()
            .AddTransient<GroupService>()
            .AddTransient<StudentService>()
            .AddTransient<TeacherService>()
            .AddSingleton<ObservableCollection<Courses>>()
            .AddSingleton<ObservableCollection<Teachers>>()
            .AddSingleton<INavigation, Navigation>()
            .AddSingleton<IDialogService, DialogService>()

            .AddSingleton<MainViewModel>()
            .AddTransient<HomeViewModel>()
            .AddTransient<GroupSettingsViewModel>()

            .AddSingleton<CoursesViewModel>()
            .AddTransient<StudentsViewModel>()
            .AddTransient<TeachersViewModel>()
            .AddTransient<GroupsViewModel>();
    }
}
