using Godot;
using System;

namespace Cards
{
    public interface ICardSlot
    {
        Vector2 WorldPosition { get;}
        Vector2 Size {get;}
        void setPosition(Vector2 pos);
    }

    public partial class CardSlot : Node2D, ICardSlot
    {
        public Vector2 WorldPosition => Position;
        public Vector2 Size => GetNode<Sprite2D>("Sprite2D").GetRect().Size;
        public void setPosition(Vector2 pos)
        {
            Position = pos;
        }
    }
}


