using Godot;
using System;

namespace Cards
{
    public interface ICardView
    {
        Vector2 WorldPosition { set; }

        void SetCardSprite(Texture2D texture);
    }

    public partial class CardView : Node2D, ICardView
    {
        [Export] 
        public Sprite2D sprite { get; set; }
        public Vector2 WorldPosition 
        {
            set
            {
                Position = value;
            }
        }

        public void SetCardSprite(Texture2D texture)
        {
            sprite.Texture = texture;
        }
    }
}
