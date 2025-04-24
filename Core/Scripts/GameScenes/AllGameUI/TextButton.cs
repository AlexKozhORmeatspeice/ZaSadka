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
        [Export] private AudioStreamPlayer2D audioPlayer;
         
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

            onClick += PlaySound;
        }

        public override void _ExitTree()
        {
            onClick -= PlaySound;
        }

        private void PlaySound()
        {
            if (audioPlayer != null)
            {
                audioPlayer.Play();
            }
        }
    }
}

