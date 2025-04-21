using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Market
{
    public interface INextSceneButton
    {
        event Action onClick;

        string GetNextScene();

        SceneTree GetTree();
    }

    public partial class NextSceneButton : Node2D, INextSceneButton
    {
        [Export] private string nextScene = "res://Core/Scenes/GameScenes/EventsWindow.tscn"; 
        [Export] private Button button;
        [Export] private string text;

        public event Action onClick
        {
            add
            {
                button.Pressed += value;
            }
            remove
            {
                button.Pressed -= value;
            }
        }

        public string GetNextScene()
        {
            return nextScene;
        }

        public override void _Ready()
        {
            button.Text = text;
        }
    }
}
