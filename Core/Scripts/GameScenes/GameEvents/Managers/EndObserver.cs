using DI;
using Godot;
using System;
using System.Threading.Tasks;

namespace Game_events
{
    public interface IEndObserver
    {

    }

    public partial class EndObserver : Node2D, IEndObserver, IStartable
    {
        [Inject] private IEventsManager eventsManager;

        [Export] private PackedScene nextScene;

        public void Start()
        {
            eventsManager.onEndEvents += LoadNextScene;
        }

        public override void _ExitTree()
        {
            eventsManager.onEndEvents -= LoadNextScene;
        }

        private async void LoadNextScene()
        {
            await ToSignal(GetTree().CreateTimer(.001), "timeout");
            GetTree().ChangeSceneToFile("res://Core/Scenes/GameScenes/Market.tscn");
        }
    }

}

