using Godot;
using Market;
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

        ItemInfo GetItemInfo();

        void SetInfo(ItemInfo info);
    }

    public partial class CardView : Node2D, ICardView
    {
        [Export] private float mouseDistToDetect = 80.0f;
        [Export] public Sprite2D sprite { get; set; }
        [Export] private Vector2 minScale;
        [Export] private Vector2 maxScale;
        
        [Export] private Label name;
        [Export] private Label supplyText;
        [Export] private Label demandText;
        [Export] private Label influenceText;
        [Export] private Label susText;

        private ItemInfo info;

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
            this.info = info;
            name.Text = info.name;

            if (info.supply != 0)
            {
                supplyText.Text = "Спрос: " + info.supply.ToString();
            }
            else
            {
                supplyText.Text = "";
            }

            if (info.demand != 0)
            {
                demandText.Text = "Предложение: " + info.demand.ToString();
            }
            else
            {
                demandText.Text = "";
            }

            if (info.influence != 0)
            {
                influenceText.Text = "Влияние: " + info.influence.ToString();
            }
            else
            {
                influenceText.Text = "";
            }

            if (info.suspicion != 0)
            {
                susText.Text = "Подозрение: " + info.suspicion.ToString();
            }
            else
            {
                susText.Text = "";
            }

            float width = 100.0f;
            float height = 150.0f;

            int rectY = info.type == ItemType.Building ? 0 : 1;
            int rectX = info.spriteId;
            sprite.RegionRect = new Rect2(width*rectX, height*rectY, width, height);
        }

        public ItemInfo GetItemInfo()
        {
            return info;
        }
    }

}
