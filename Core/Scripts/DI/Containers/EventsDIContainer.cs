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

namespace GameEvents
{
    public partial class EventsDIContainer : DIContainer
    {
        [Export] private EventsSpawner eventsSpawner;
        [Export] private MouseManager mouseManager;
        override protected void RegisterObjects()
        {
            builder.Register<IChoiceFabric>(new ChoiceFabric());
            builder.Register<IEventsSpawner>(eventsSpawner);
            builder.Register<IPointerManager>(mouseManager);
        }
    }
}


