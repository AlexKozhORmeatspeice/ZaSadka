using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Market
{
	public interface INextSceneButton
	{
		event Action onClick;

		string GetNextScene();
		string GetFinalScene();
		string GetLoseScene();

		SceneTree GetTree();
	}

	public partial class NextSceneButton : Node2D, INextSceneButton
	{
		[Export] private string nextScene = "res://Core/Scenes/GameScenes/EventsWindow.tscn"; 
		[Export] private string finalScene = "res://Core/Scenes/GameScenes//game_win.tscn";
		[Export] private string loseScene = "res://Core/Scenes/GameScenes/game_lose.tscn";
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
		public string GetFinalScene()
		{
			return finalScene;
		}

		public override void _Ready()
		{
			button.Text = text;
		}

        public string GetLoseScene()
        {
			return loseScene;
        }
    }
}
