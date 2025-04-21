using Godot;
using System;


namespace Market
{
    public interface ITextButton
    {
        event Action onClick;
    }

    public partial class TextButton : Node2D, ITextButton
    {
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

        public override void _Ready()
        {
            button.Text = text;
        }
    }
}

