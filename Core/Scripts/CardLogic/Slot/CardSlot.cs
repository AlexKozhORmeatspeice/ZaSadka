using Godot;
using System;

namespace Cards
{
    public interface ICardSlot
    {
        Vector2 WorldPosition { get; }
    }

    public partial class CardSlot : Node2D, ICardSlot
    {
        public Vector2 WorldPosition => Position;
    }
}


