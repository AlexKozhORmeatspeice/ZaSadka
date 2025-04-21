using Cards;
using DI;
using Godot;
using System;
using System.Collections.Generic;
using ZaSadka;
using Game_events;


namespace Game_events
{
    public interface IEventsSpawner
    {
        event Action<IGameEvent, EventInfo> onUpdateInfo;
        event Action noEvents;
    }

    public partial class EventsSpawner : Node2D, ILateStartable, IEventsSpawner
    {
        [Inject] private IObjectResolver resolver;
        [Inject] private IEventsFabric jsonEventsFabric;

        [Export] private PackedScene gameEventScene;

        private List<IGameEventObserver> gameEventObservers;

        public event Action<IGameEvent, EventInfo> onUpdateInfo;
        public event Action noEvents;

        public void LateStart()
        {
            CreateEvents();
        }

        private void CreateEvents()
        {
            //get slots for cards
            gameEventObservers = new List<IGameEventObserver>();
            List<EventInfo> eventsInfo = new List<EventInfo>();

            for(int i = 0; i < (int)DistrictName.DistrictNameNum; i++)
            {
                DistrictName districtName = (DistrictName)i;

                EventInfo info = jsonEventsFabric.GetActiveEvent(districtName);

                if (info.canActivate)
                {
                    eventsInfo.Add(info);
                }
            }

            if (eventsInfo.Count == 0)
            {
                noEvents?.Invoke();
                return;
            }

            foreach (EventInfo eventInfo in eventsInfo)
            {
                var newInstance = gameEventScene.Instantiate();
                AddChild(newInstance);
                IGameEvent newCardView = (IGameEvent)newInstance;

                IGameEventObserver cardObserver = new GameEventObserver(newCardView);
                resolver.Inject(cardObserver);
                cardObserver.Enable();

                onUpdateInfo?.Invoke(newCardView, eventInfo);

                gameEventObservers.Add(cardObserver);
            }
        }

        public override void _ExitTree()
        {
            foreach (var gameEventObserver in gameEventObservers)
            {
                gameEventObserver.Disable();
            }
        }
    }

}