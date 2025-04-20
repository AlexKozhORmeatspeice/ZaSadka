using DI;
using Godot;
using System;
using ZaSadka;

namespace Market
{
    public enum ItemType
    {
        Building = 0,
        Unit,
        District
    }

    public interface IItem
    {
        void SetChoosed(bool isChoosed);
        void SetVisibility(bool isVisible);

        Vector2 WorldPosition { get;  set; }

        void ChangeScale(float t01);

        float MouseDistToDetect { get; }

        int Price {  get; }
        void SetInfo(ItemInfo info);
        ItemInfo GetInfo();
    }

    public partial class Item : Node2D, IItem
    {
        [Export] private Label label;
        [Export] private Label priceLabel;
        [Export] private VBoxContainer dataShower;
            
        [Export] private CollisionShape2D _collisionShape;

        [Export] private Sprite2D sprite;
        [Export] private Color baseColor;
        [Export] private Color choosedColor;
        
        [Export] private float mouseDistToDetect = 80.0f;

        [Export] private Vector2 maxScale;
        [Export] private Vector2 minScale;
        private ItemInfo itemInfo;

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

        public int Price => itemInfo.price;

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

        private void AddBonus(int bonus, string bonusName, bool inverse = false)
        {
            Color green = new(0, 1, 0);
            Color red = new(1, 0, 0);

            if (bonus != 0)
            {
                bool isPositive = bonus > 0;
                Label info = new()
                {
                    Text = bonusName + ": " + (isPositive ? "+" : "") + bonus.ToString()
                };
                if (inverse)
                {
                    isPositive = !isPositive;
                }
                info.AddThemeColorOverride("font_color", isPositive ? green : red);
                info.AddThemeFontSizeOverride("font_size", 9);
                dataShower.AddChild(info);
            }
        }

        public void SetInfo(ItemInfo info)
        {
            label.Text = info.name;
            priceLabel.Text = info.price.ToString();

            AddBonus(info.demand, "Спрос");
            AddBonus(info.supply, "Предложение");
            AddBonus(info.influence, "Влияние");
            AddBonus(info.suspicion, "Подозрение", true);

            itemInfo = info;

            float width = 100.0f;
            float height = 150.0f;

            int rectY = info.type == ItemType.Building ? 0 : 1;
            int rectX = info.spriteId;
            sprite.RegionRect = new Rect2(width*rectX, height*rectY, width, height);
        }

        public ItemInfo GetInfo() => itemInfo;
    }

}

