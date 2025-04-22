using Godot;
using Market;
using System;

namespace GameWin
{
	public interface IMainMenuBtnObserver
	{
		void Enable();
		void Disable();
	}
	public partial class MainMenuBtnObserver : Node2D, IMainMenuBtnObserver
	{
		INextSceneButton view;
		public MainMenuBtnObserver(INextSceneButton view)
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
