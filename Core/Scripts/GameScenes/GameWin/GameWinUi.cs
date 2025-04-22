using Godot;
using System;
using DI;
using Market;


namespace GameWin
{
	public interface IGameWinUI
	{

	}
	//то как сильно мне хотелось просто сделать кнопку с сигналом pressed...
	public partial class GameWinUi : Node2D, IStartable, IDispose, IGameWinUI
	{
		[Export] private NextSceneButton nextSceneButton;
		private IMainMenuBtnObserver mainMenuBtnObserver;
		
		[Inject]
		public void Construct(IObjectResolver resolver)
		{
			resolver.Inject(mainMenuBtnObserver = new MainMenuBtnObserver(nextSceneButton));
		}

		public void Start()
		{
			mainMenuBtnObserver.Enable();
		}

		public void Dispose()
		{
			mainMenuBtnObserver.Disable();
		}
}
}
