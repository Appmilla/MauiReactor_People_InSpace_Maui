using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MauiReactor;
using MauiReactor.Compatibility;
using MauiReactorPeopleInSpace.Models;
using MauiReactorPeopleInSpace.Navigation;
using MauiReactorPeopleInSpace.Reactive;
using MauiReactorPeopleInSpace.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MauiReactorPeopleInSpace.Pages;

public class MainPageState
{
    public string PageTitle { get; set; } = "People In Space MauiReactor MAUI";
    public ObservableCollection<CrewModel> Crew { get; set; } = new();
    public bool IsRefreshing { get; set; }
}

public partial class MainPage : Component<MainPageState>
{
    private readonly ISchedulerProvider _schedulerProvider;
    private readonly ICrewRepository _crewRepository;
    private readonly INavigationService _navigationService;

    [Prop("Shell")] protected MauiControls.Shell? shellRef;
    
    public MainPage()
    {
        _schedulerProvider = Services.GetRequiredService<ISchedulerProvider>();
        _crewRepository = Services.GetRequiredService<ICrewRepository>();
        _navigationService = Services.GetRequiredService<INavigationService>();
    }

    protected override void OnMounted()
    {
        LoadCrew(false);
        base.OnMounted();
    }

    private void LoadCrew(bool forceRefresh)
    {
        State.IsRefreshing = true;

        _crewRepository.GetCrew(forceRefresh)
            .SubscribeOn(_schedulerProvider.ThreadPool)
            .ObserveOn(_schedulerProvider.MainThread)
            .Subscribe(result =>
            {
                SetState(_ => _.IsRefreshing = false);
                
                result.Match(
                    Right: crew => SetState(s => s.Crew = new ObservableCollection<CrewModel>(crew)),
                    Left: error => ShowError(error.Message));
            });
    }

    private void ShowError(string message)
    {
        // Implement error handling
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
        ).Title(State.PageTitle);

    private VisualNode RenderCrew(CrewModel crew)
    {
        return new Frame
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
            .CornerRadius(5)
            .HasShadow(false)
            .BackgroundColor(Colors.White)
        .OnTapped(async () => await NavigateToDetail(crew));
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
        await shellRef!.GoToAsync<DetailPageProps>(
            "DetailPage",
            props =>
            {
                props.Name = crewMember.Name;
                props.Image = crewMember.Image;
                props.Wikipedia = crewMember.Wikipedia;
            });
    }*/
}