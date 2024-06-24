
using MauiReactor;
using MauiReactorPeopleInSpace.Pages;

namespace MauiReactorPeopleInSpace;

class AppShell : Component
{  
    private const string MainPageName = "MainPage";
    
    private MauiControls.Shell? _shell;
    
    protected override void OnMounted()
    {
        Routing.RegisterRoute<MainPage>(nameof(MainPage));
        Routing.RegisterRoute<DetailPage>(nameof(DetailPage));

        base.OnMounted();
    }
    
    public override VisualNode Render() =>
            new Shell(x => _shell = x!)
                {
                    new ShellContent(MainPageName)
                        .AutomationId(MainPageName)
                        .Title(MainPageName)
                        .Route(MainPageName)
                        .RenderContent(() => new MainPage().Shell(_shell))
                }
                .AutomationId("MainShell");
}