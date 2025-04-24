using Godot;
using ZaSadka;
using DI;

namespace Game_events
{
    public partial class EventsDIContainer : DIContainer
    {
        [Export] private EventsSpawner eventsSpawner;
        [Export] private MouseManager mouseManager;
        [Export] private EventsUI eventsUI;
        [Export] private EndObserver endObserver;
        [Export] private ChooseSoundManager chooseSoundManager;
        override protected string name {get => "Events DI";}
        override protected void RegisterObjects()
        {
            builder.Register<IDistrictsManager>(new DistrictsManager());
            builder.Register<ISuspicionManager>(new SuspicionManager());
            builder.Register<IInfluenceManager>(new InfluenceManager());
            builder.Register<ISupplyDemandManager>(new SupplyAndDemandManager());
            builder.Register<IMoneyManager>(new MoneyManager());
            builder.Register<IEventsManager>(new EventsManager());

            builder.Register<IChoiceFabric>(new JsonChoiceFabric());
            builder.Register<IEventsFabric>(new JsonEventsFabric());

            builder.Register<IEventsSpawner>(eventsSpawner);
            builder.Register<IPointerManager>(mouseManager);
            builder.Register<IEventsUI>(eventsUI);
            builder.Register<IEndObserver>(endObserver);
            builder.Register<IChooseSoundManager>(chooseSoundManager);
        }
    }
}


