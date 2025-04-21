using Godot;
using System;

namespace Game_events
{
    public interface IGameText
    {
        string Description { set; }
    }

    public partial class GameText : Label, IGameText
    {
        public string Description { set => this.Text = value; }
    }
}


