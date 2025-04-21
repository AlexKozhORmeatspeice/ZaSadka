using DI;
using Godot;
using System;

namespace Game_events
{
    public interface IEventsUI
    {

    }

    public partial class EventsUI : Node2D, IEventsUI, IStartable
    {
        [Export] private GameText yesChoice;
        [Export] private GameText noChoice;

        private IYesTextObserver yesObserver;
        private INoTextObserver noObserver;

        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            resolver.Inject(yesObserver = new YesTextObserver(yesChoice));
            resolver.Inject(noObserver = new NoTextObserver(noChoice));
        }

        public void Start()
        {
            yesObserver.Enable();
            noObserver.Enable();
        }

        public override void _ExitTree()
        {
            yesObserver.Disable();
            noObserver.Disable();
        }
    }

}

