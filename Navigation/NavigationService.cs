using System.Threading.Tasks;

namespace MauiReactorPeopleInSpace.Navigation;

public interface INavigationService
{
    Task NavigateAsync(string route);
}

public class NavigationService : INavigationService
{
    public async Task NavigateAsync(string route)
    {
        await MauiControls.Shell.Current.GoToAsync(route);
    }
}