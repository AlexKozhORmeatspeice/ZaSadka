using DI;
using GameWin;
using Godot;
using System;

public partial class GameWinDi : DIContainer
{
	[Export] GameWinUi gameWinUI;
	override protected void RegisterObjects()
	{
		builder.Register<IGameWinUI>(gameWinUI);
	}
}
