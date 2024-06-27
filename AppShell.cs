
using System;
using MauiReactor;
using MauiReactorPeopleInSpace.Alerts;
using MauiReactorPeopleInSpace.Extensions;
using MauiReactorPeopleInSpace.Network;
using MauiReactorPeopleInSpace.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Networking;

namespace MauiReactorPeopleInSpace;

public class AppShellState
{
    public NetworkAccess? CurrentNetworkAccess;
}

partial class  AppShell : Component<AppShellState>
{  
    private const string MainPageName = "MainPage";
    private readonly TimeSpan _snackbarDuration = TimeSpan.FromSeconds(3);
    
    private MauiControls.Shell? _shell;
    
    [Inject] private readonly INetworkStatusObserver _networkStatusObserver;
    [Inject] private readonly IUserAlerts _userAlerts;
    [Inject] private readonly IConnectivity _connectivity;
    
    protected override void OnMounted()
    {
        Routing.RegisterRoute<MainPage>(nameof(MainPage));
        Routing.RegisterRoute<DetailPage>(nameof(DetailPage));

        base.OnMounted();
        
        _networkStatusObserver.Start();
        
        _networkStatusObserver.ConnectivityNotifications.Subscribe(networkAccess =>
        {
            if (State.CurrentNetworkAccess == networkAccess) return;

            SetState(s => s.CurrentNetworkAccess = networkAccess);
           
            _userAlerts.ShowSnackbar(networkAccess != NetworkAccess.Internet
                    ? "Internet access has been lost."
                    : "Internet access has been restored.", 
                _snackbarDuration).FireAndForgetSafeAsync();
        });
        
        CheckInitialNetworkStatus();
    }

    protected override void OnWillUnmount()
    {
        _networkStatusObserver.Dispose();
        
        base.OnWillUnmount();
    }

    private void CheckInitialNetworkStatus()
    {
        SetState(s => s.CurrentNetworkAccess = _connectivity.NetworkAccess);
        if (State.CurrentNetworkAccess != NetworkAccess.Internet)
        {
            _userAlerts.ShowSnackbar("No internet access.", _snackbarDuration)
                .FireAndForgetSafeAsync();
        }
    }
    
    public override VisualNode Render() =>
            new Shell(x => _shell = x!)
                {
                    new ShellContent(MainPageName)
                        .AutomationId(MainPageName)
                        .Title(MainPageName)
                        .Route(MainPageName)
                        .RenderContent(() => Services.GetRequiredService<MainPage>().Shell(_shell))
                }
                .AutomationId("MainShell");
}