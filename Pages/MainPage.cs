using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MauiReactor;
using MauiReactor.Compatibility;
using MauiReactorPeopleInSpace.Alerts;
using MauiReactorPeopleInSpace.Extensions;
using MauiReactorPeopleInSpace.Models;
using MauiReactorPeopleInSpace.Navigation;
using MauiReactorPeopleInSpace.Reactive;
using MauiReactorPeopleInSpace.Repositories;

namespace MauiReactorPeopleInSpace.Pages;

public class MainPageState
{
    public string PageTitle { get; set; } = "People In Space MauiReactor MAUI";
    public ObservableCollection<CrewModel> Crew { get; set; } = new();
    public bool IsRefreshing { get; set; }
}

public partial class MainPage : Component<MainPageState>
{
    private IDisposable? _refreshSubscription;

    [Inject] private readonly ISchedulerProvider _schedulerProvider;
    [Inject] private readonly ICrewRepository _crewRepository;
    [Inject] private readonly INavigationService _navigationService;
    [Inject] private readonly IUserAlerts _userAlerts;
    
    [Prop("Shell")] protected MauiControls.Shell? ShellRef;
    
    protected override void OnWillUnmount()
    {
        base.OnWillUnmount();
        
        _refreshSubscription?.Dispose();
    }
    
    private void LoadCrew(bool forceRefresh)
    {
        _refreshSubscription?.Dispose();
        
        SetState(_ => _.IsRefreshing = true);

        _refreshSubscription = _crewRepository.GetCrew(forceRefresh)
            .SubscribeOn(_schedulerProvider.ThreadPool)
            .ObserveOn(_schedulerProvider.MainThread)
            .Subscribe(result =>
            {
                SetState(_ => _.IsRefreshing = false);
                
                result.Match(
                    Right: crew => SetState(s => s.Crew = new ObservableCollection<CrewModel>(crew)),
                    Left: HandleError);
            });
    }
    
    private void HandleError(CrewError error)
    {
        switch (error)
        {
            case ParsingError parsingError:
                ShowSnackbar("Parsing error occurred. Please contact customer support or update the app.");
                break;
            default:
                ShowError(error.Message);
                break;
        }
    }

    private void ShowSnackbar(string message)
    {
        _userAlerts.ShowSnackbar(message, TimeSpan.FromSeconds(20)).FireAndForgetSafeAsync();
    }
    
    private void ShowError(string message)
    {
        _userAlerts.ShowToast(message).FireAndForgetSafeAsync();
    }
    
    public override VisualNode Render()
        => ContentPage(
            RefreshView(
                    ScrollView(
                        CollectionView()
                            .ItemsSource(State.Crew, RenderCrew)
                            .SelectionMode(MauiControls.SelectionMode.None)
                            .EmptyView("Please pull to refresh the view")
                            .ItemsLayout(new VerticalLinearItemsLayout().ItemSpacing(10))
                            .Margin(10)
                    )
                )
                .OnRefreshing(() => LoadCrew(true))
                .IsRefreshing(State.IsRefreshing)
        ).OnAppearing(OnAppearing).Title(State.PageTitle);

    private void OnAppearing(object? obj, EventArgs eventArgs)
    {
        LoadCrew(false);
    }


    private VisualNode RenderCrew(CrewModel crew)
    {
        return new Border
            {
                new Grid("80,50", "160,*")
                    {
                        Image(crew.Image)
                            .GridRowSpan(2)
                            .HeightRequest(150)
                            .WidthRequest(150)
                            .Aspect(Aspect.AspectFill)
                            .Margin(0),
                        Label(crew.Name)
                            .GridRow(0)
                            .GridColumn(1)
                            .FontSize(18)
                            .FontAttributes(MauiControls.FontAttributes.Bold)
                            .VStart()
                            .Margin(10, 10, 10, 0),
                        Label(crew.Agency)
                            .GridRow(1)
                            .GridColumn(1)
                            .FontSize(16)
                            .Margin(10, 0, 10, 10),
                    }
                    .Padding(20)
            }
            .Padding(10)
            .StrokeCornerRadius(5)
            .BackgroundColor(Colors.White)
            .OnTapped(Action);

        async void Action() => await NavigateToDetail(crew);
    }
    
    private async Task NavigateToDetail(CrewModel crewMember)
    {
        await _navigationService.NavigateAsync<DetailPageProps>(
            "DetailPage",
            props =>
            {
                props.Name = crewMember.Name;
                props.Image = crewMember.Image;
                props.Wikipedia = crewMember.Wikipedia;
            });
    }
    
    /*
    private async Task NavigateToDetail(CrewModel crewMember)
    {
        await ShellRef!.GoToAsync<DetailPageProps>(
            "DetailPage",
            props =>
            {
                props.Name = crewMember.Name;
                props.Image = crewMember.Image;
                props.Wikipedia = crewMember.Wikipedia;
            });
    }*/
}