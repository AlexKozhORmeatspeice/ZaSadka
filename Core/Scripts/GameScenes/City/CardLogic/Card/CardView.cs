using Godot;
using System;
using ZaSadka;

namespace Cards
{
    public interface ICardView
    {
        Vector2 WorldPosition { get; set; }
        float MouseDistToDetect { get; }
        void SetCardSprite(Texture2D texture);
        void ChangeScale(float t01);
        void SetInfo(ItemInfo info);
    }

    public partial class CardView : Node2D, ICardView
    {
        [Export] private float mouseDistToDetect = 80.0f;
        [Export] public Sprite2D sprite { get; set; }
        [Export] private Vector2 minScale;
        [Export] private Label name;
        [Export] private Vector2 maxScale;
        
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
            newScale.X = Mathf.Lerp(minScale.X, maxScale.X, t01);
            newScale.Y = Mathf.Lerp(minScale.Y, maxScale.Y, t01);

            sprite.Scale = newScale;
        }

        public void SetInfo(ItemInfo info)
        {
            name.Text = info.name;

            // VBoxContainer dataShower = GetNode<VBoxContainer>("DataShower");
            // if (info.supply != 0)
            // {
            //     Label supplyInfo = new Label();
            //     supplyInfo.Text = info.supply.ToString();
            //     dataShower.AddChild(supplyInfo);
            // }
            // if (info.demand != 0)
            // {
            //     Label demandInfo = new Label();
            //     demandInfo.Text = info.demand.ToString();
            //     dataShower.AddChild(demandInfo);
            // }
            // if (info.influence != 0)
            // {
            //     Label influenceInfo = new Label();
            //     influenceInfo.Text = info.influence.ToString();
            //     dataShower.AddChild(influenceInfo);
            // }
            // if (info.suspicion != 0)
            // {
            //     Label suspicionInfo = new Label();
            //     suspicionInfo.Text = info.suspicion.ToString();
            //     dataShower.AddChild(suspicionInfo);
            // }
        }
    }
}
