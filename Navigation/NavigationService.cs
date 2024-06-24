using System;
using System.Threading.Tasks;
using MauiReactor;
namespace MauiReactorPeopleInSpace.Navigation;

public interface INavigationService
{
    Task NavigateAsync<P>(string route, Action<P> propsInitializer) where P : new();
}

public class NavigationService : INavigationService
{
    public async Task NavigateAsync<P>(string route, Action<P> propsInitializer) where P : new()
    {
        await MauiControls.Shell.Current.GoToAsync<P>(route, propsInitializer);
    }
}