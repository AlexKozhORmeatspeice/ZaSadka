using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cards;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using Market;
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
        }
    }
}


