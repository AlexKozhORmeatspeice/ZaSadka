using DI;
using Godot;
using System;

namespace Market
{
    public enum ItemType
    {
        None,
        old_docks,
        historical_center,
        suburbs,
        laboratory,
        point_of_sale,
        puppet_enterprise,
        illegal_warehouse,
        office_facade,
        dealer,
        guard,
        marketer,
        intern,
        agent
    }

    public interface IItem
    {
        void SetChoosed(bool isChoosed);
        void SetVisibility(bool isVisible);

        Vector2 WorldPosition { get;  set; }

        void ChangeScale(float t01);

        float MouseDistToDetect { get; }

        int Price {  get; }
    }

    public partial class Item : Node2D, IItem
    {
        [Export] private CollisionShape2D _collisionShape;

        [Export] private Sprite2D sprite;
        [Export] private Color baseColor;
        [Export] private Color choosedColor;
        
        [Export] private int price;
        [Export] private ItemType type;
        
        [Export] private float mouseDistToDetect = 80.0f;

        [Export] private Vector2 maxScale;
        [Export] private Vector2 minScale;

        public Vector2 SpriteScale
        { 
            get => sprite.GlobalScale;
            set => sprite.Scale = value; 
        }

        public float MouseDistToDetect
        {
            get
            {
                return mouseDistToDetect;
            }
        }

        public int Price => price;

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

        public void SetChoosed(bool isChoosed)
        {
            if(isChoosed)
            {
                sprite.Modulate = choosedColor;
            }
            else
            {
                sprite.Modulate = baseColor;
            }
        }

        public void ChangeScale(float t01)
        {
            t01 = Mathf.Clamp(t01, 0.0f, 1.0f);

            Vector2 newScale = sprite.Scale;
            newScale.X = Mathf.Lerp(minScale.X, maxScale.X, t01);
            newScale.Y = Mathf.Lerp(minScale.Y, maxScale.Y, t01);

            _collisionShape.Scale = newScale;
            sprite.Scale = newScale;
        }

        public void SetVisibility(bool isVisible)
        {
            sprite.Visible = isVisible;
        }
    }

}

