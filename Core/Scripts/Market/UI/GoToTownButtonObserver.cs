using Godot;
using System;

namespace Market
{
    public interface IGoToTownButtonObserver
    {
        void Enable();
        void Disable();
    }

    public partial class GoToTownButtonObserver : Node, IGoToTownButtonObserver
    {
        private IGoToTownButton view;

        public GoToTownButtonObserver(IGoToTownButton view)
        {
            this.view = view;
        }

        public void Enable()
        {
            view.onClick += LoadScene; 
        }

        public void Disable()
        {
            view.onClick -= LoadScene;
        }

        private void LoadScene()
        {
            view.GetTree().ChangeSceneToPacked(view.GetNextScene());
        }
    }
}

