using Godot;
using System;

namespace Market
{
    public interface IGoToTownButton
    {
        event Action onClick;

        PackedScene GetNextScene();

        SceneTree GetTree();
    }

    public partial class GoToTownButton : Button, IGoToTownButton
    {
        [Export] private PackedScene nextScene; 
        [Export] private Button button;

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

        public PackedScene GetNextScene()
        {
            return nextScene;
        }
    }
}
