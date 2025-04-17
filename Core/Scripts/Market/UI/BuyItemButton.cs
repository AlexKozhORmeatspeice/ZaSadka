using Godot;
using System;


namespace Market
{
    public interface IBuyItemButton
    {
        event Action onClick;
    }

    public partial class BuyItemButton : Button, IBuyItemButton
    {
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
    }
}

