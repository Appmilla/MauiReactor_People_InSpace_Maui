using System;
using MauiReactor;
using MauiReactorPeopleInSpace.Models;

namespace MauiReactorPeopleInSpace.Pages;

public class DetailPageState
{
    public CrewDetailModel CrewMember { get; set; }
}

public class DetailPageProps
{
    public string Name { get; set; } = null!;
    public Uri Image { get; set; } = null!;
    public Uri Wikipedia { get; set; } = null!;
}

class DetailPage : Component<DetailPageState, DetailPageProps>
{
    protected override void OnMounted()
    {
        State.CrewMember = new CrewDetailModel(Props.Name, Props.Image, Props.Wikipedia);
        base.OnMounted();
    }

    public override VisualNode Render()
    {
        return new ContentPage("Biography")
            {
                new ScrollView()
                {
                    new Grid("Auto 70 3000", "*")
                    {
                        new Image()
                            .GridRow(0)
                            .GridColumn(0)
                            .Source(State.CrewMember.Image)
                            .VCenter()
                            .HCenter(),
                   
                        new Label(State.CrewMember.Name)
                            .GridRow(1)
                            .GridColumn(0)
                            .Padding(10)
                            .HCenter()
                            .FontSize(24),
                    
                        new WebView()
                            .GridRow(2)
                            .GridColumn(0)
                            .Source(State.CrewMember.Wikipedia)
                            .VFill()
                    }
                }
            }
            .BackgroundColor(Colors.White);
    }
}