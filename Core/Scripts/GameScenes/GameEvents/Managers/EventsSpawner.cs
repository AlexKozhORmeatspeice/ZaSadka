using Cards;
using DI;
using Godot;
using System;
using System.Collections.Generic;

namespace GameEvents
{
    public interface IEventsSpawner
    {

    }

    public partial class EventsSpawner : Node2D, IStartable, IEventsSpawner
    {
        [Inject] private IObjectResolver resolver;

        [ExportCategory("Settings")]
        [Export] private int eventCount = 3;

        [ExportCategory("Scenes")]
        [Export] private PackedScene[] eventsPackedSecnes;

        private List<IGameEventObserver> gameEventObservers;

        public void Start()
        {
            CreateEvents();
        }

        private void CreateEvents()
        {
            //get slots for cards
            gameEventObservers = new List<IGameEventObserver>();

            //check available events
            List<PackedScene> availablePackedScemes = new List<PackedScene>();
            foreach(PackedScene gameEventPackedScene in eventsPackedSecnes)
            {
                //полная хуйня, но как еще это сделать не ебу
                Node newGameEventNode = gameEventPackedScene.Instantiate();
                IGameEvent newGameEvent = newGameEventNode as GameEvent;

                if (newGameEvent != null && newGameEvent.IsCanInvoke())
                {
                    availablePackedScemes.Add(gameEventPackedScene);
                }

                (newGameEvent as Node).QueueFree();
            }

            if (availablePackedScemes.Count == 0)
                return;

            //get random events
            for (int i = 0; i < eventCount; i++)
            {
                int randomItemInd = GD.RandRange(0, availablePackedScemes.Count - 1);

                var newInstance = availablePackedScemes[randomItemInd].Instantiate();
                AddChild(newInstance);
                IGameEvent newCardView = (IGameEvent)newInstance;

                IGameEventObserver cardObserver = new GameEventObserver(newCardView);
                resolver.Inject(cardObserver);
                cardObserver.Enable();

                gameEventObservers.Add(cardObserver);
            }
        }

        public override void _ExitTree()
        {
            foreach (var cardObserver in gameEventObservers)
            {
                cardObserver.Disable();
            }
        }
    }

}