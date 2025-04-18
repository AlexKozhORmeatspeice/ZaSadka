using DI;
using Godot;
using System;
using ZaSadka;

namespace Market
{
    public enum ItemType
    {
        Building = 0,
        Unit
    }

    public interface IItem
    {
        void SetChoosed(bool isChoosed);
        void SetVisibility(bool isVisible);

        Vector2 WorldPosition { get;  set; }

        void ChangeScale(float t01);

        float MouseDistToDetect { get; }

        int Price {  get; }
        void setInfo(ItemInfo info);
    }

    public partial class Item : Node2D, IItem
    {
        [Export] private CollisionShape2D _collisionShape;

        [Export] private Sprite2D sprite;
        [Export] private Color baseColor;
        [Export] private Color choosedColor;
        
        [Export] private int price;
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

        public void setInfo(ItemInfo info)
        {
            GD.Print("hi!");
            GetNode<Label>("Label").Text = info.name;

            Color green = new Color(0, 1, 0);
            Color red = new Color(1, 0, 0);

            VBoxContainer dataShower = GetNode<VBoxContainer>("DataShower");
            if (info.supply != 0)
            {
                Label supplyInfo = new()
                {
                    Text = info.supply.ToString()
                };
                supplyInfo.AddThemeColorOverride("font_color", info.supply > 0 ? green : red);
                supplyInfo.AddThemeFontSizeOverride("font_color", 8);
                dataShower.AddChild(supplyInfo);
            }
            if (info.demand != 0)
            {
                Label demandInfo = new()
                {
                    Text = info.demand.ToString()
                };
                demandInfo.AddThemeColorOverride("font_color", info.supply > 0 ? green : red);
                demandInfo.AddThemeFontSizeOverride("font_color", 8);
                dataShower.AddChild(demandInfo);
            }
            if (info.influence != 0)
            {
                Label influenceInfo = new()
                {
                    Text = info.influence.ToString()
                };
                influenceInfo.AddThemeColorOverride("font_color", info.supply > 0 ? green : red);
                influenceInfo.AddThemeFontSizeOverride("font_color", 8);
                dataShower.AddChild(influenceInfo);
            }
            if (info.suspicion != 0)
            {
                Label suspicionInfo = new()
                {
                    Text = info.suspicion.ToString()
                };
                suspicionInfo.AddThemeColorOverride("font_color", info.supply > 0 ? green : red);
                suspicionInfo.AddThemeFontSizeOverride("font_color", 8);
                dataShower.AddChild(suspicionInfo);
            }
        }
    }

}

