using System;
using System.Reactive;
using Akavache;
using CommunityToolkit.Maui;
using MauiReactor;
using MauiReactorPeopleInSpace.Alerts;
using MauiReactorPeopleInSpace.Apis;
using MauiReactorPeopleInSpace.Navigation;
using MauiReactorPeopleInSpace.Network;
using MauiReactorPeopleInSpace.Pages;
using MauiReactorPeopleInSpace.Reactive;
using MauiReactorPeopleInSpace.Repositories;
using Microsoft.Extensions.Logging;

using ReactiveUI;
using Refit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Networking;

namespace MauiReactorPeopleInSpace;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Akavache.Registrations.Start("PeopleInSpace");

        RxApp.DefaultExceptionHandler = new AnonymousObserver<Exception>(ex =>
        {
            //App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        });
        
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiReactorApp<AppShell>(app =>
            {
                app.AddResource("Resources/Styles/Colors.xaml");
                app.AddResource("Resources/Styles/Styles.xaml");
            })
            .UseMauiCommunityToolkit()
#if DEBUG
            .EnableMauiReactorHotReload()
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IBlobCache>(BlobCache.LocalMachine);
        
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DetailPage>();
        builder.Services.AddSingleton<ICrewRepository, CrewRepository>();
        builder.Services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IUserAlerts, UserAlerts>();
        builder.Services.AddRefitClient<ISpaceXApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.spacexdata.com/v4"));
        builder.Services.AddSingleton<IConnectivity>(provider => Connectivity.Current);
        builder.Services.AddSingleton<INetworkStatusObserver, NetworkStatusObserver>();
        
        return builder.Build();
    }
}