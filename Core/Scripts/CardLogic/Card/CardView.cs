using Godot;
using System;

namespace Cards
{
    public interface ICardView
    {
        Vector2 WorldPosition { get; set; }
        float MouseDistToDetect { get; }
        void SetCardSprite(Texture2D texture);
        void ChangeScale(float t01);
    }

    public partial class CardView : Node2D, ICardView
    {
        [Export] private float mouseDistToDetect = 80.0f;
        [Export] public Sprite2D sprite { get; set; }
        [Export] private Vector2 maxSize;
        [Export] private Vector2 minSize;
        
        public Vector2 WorldPosition 
        {
            get
            {
                return Position;
            }
            set
            {
                Position = value;
            }
        }

        public float MouseDistToDetect
        {
            get
            {
                return mouseDistToDetect;
            }
        }

        public void SetCardSprite(Texture2D texture)
        {
            sprite.Texture = texture;
        }

        public void ChangeScale(float t01)
        {
            t01 = Mathf.Clamp(t01, 0.0f, 1.0f);

            Vector2 newScale = sprite.GlobalScale;
            newScale.X = Mathf.Lerp(minSize.X, maxSize.X, t01);
            newScale.Y = Mathf.Lerp(minSize.Y, maxSize.Y, t01);

            sprite.Scale = newScale;
        }
    }
}
