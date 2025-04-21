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
        private INextSceneButton view;

        public GoToTownButtonObserver(INextSceneButton view)
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
            view.GetTree().ChangeSceneToFile(view.GetNextScene());
        }
    }
}

