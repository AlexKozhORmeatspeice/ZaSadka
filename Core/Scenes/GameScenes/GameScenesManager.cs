using Godot;
using System;

public interface IGameScenesManager
{
    void LoadMarket();
    void LoadMain();
    void LoadEvents();
}

public partial class GameScenesManager : Node2D, IGameScenesManager
{
    [Export] private Node2D market;
    [Export] private Node2D main;
    [Export] private Node2D events;

    public event Action onLoadMarket;
    public event Action onLoadMain;
    public event Action onLoadEvents;

    public override void _Ready()
    {
        LoadMarket();
    }

    public void LoadMarket()
    {
        onLoadMarket?.Invoke();

        main.Visible = false;
        events.Visible = false;
        market.Visible = true;
    }

    public void LoadMain()
    {
        onLoadMain?.Invoke();

        main.Visible = true;
        events.Visible = false;
        market.Visible = false;
    }

    public void LoadEvents()
    {
        onLoadEvents?.Invoke();

        main.Visible = false;
        events.Visible = true;
        market.Visible = false;
    }
}
