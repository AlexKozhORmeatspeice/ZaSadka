using Godot;
using Market;
using System;
using System.ComponentModel.Design.Serialization;
using System.IO;
using ZaSadka;

namespace Cards
{

    public enum CardSoundType
    {
        Take,
        Laboratory,
        Storage,
        Bar,
        Unit
    }

    public interface ICardView
    {
        event Action onClick;
        
        Vector2 WorldPosition { get; set; }
        float MouseDistToDetect { get; }
        void SetCardSprite(Texture2D texture);
        void ChangeScale(float t01);
        ItemInfo GetItemInfo();
        void SetInfo(ItemInfo info, bool enabled = true);
        void Delete();
        bool isEnabled {get;}
        void SetButtonVisability(bool isVisible);

        void PlaySound(CardSoundType soundType);
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
        [Export] private Button deleteButton;

        [Export] private AudioStreamPlayer2D audioStreamPlayer;
        [Export] private AudioStream[] sounds;
        
        private ItemInfo info;
        private bool isEnabled = true;

        public event Action onClick
        {
            add => deleteButton.Pressed += value;
            remove => deleteButton.Pressed -= value;
        }

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

        bool ICardView.isEnabled => isEnabled;


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

        public void SetInfo(ItemInfo info, bool enabled = true)
        {
            this.info = info;
            name.Text = info.name;

            if (info.supply != 0)
            {
                supplyText.Text = "Предложение: " + info.supply.ToString();
            }
            else
            {
                supplyText.Text = "";
            }

            if (info.demand != 0)
            {
                demandText.Text = "Спрос: " + info.demand.ToString();
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

            //getting region for card's sprite
            float width = 100.0f;
            float height = 150.0f;

            int rectY = info.type == ItemType.building ? 0 : 1;
            int rectX = info.spriteId;
            sprite.RegionRect = new Rect2(width*rectX, height*rectY, width, height);
            
            //disabling cards visually
            sprite.SetInstanceShaderParameter("enabled", enabled);
            isEnabled = enabled;
        }

        public ItemInfo GetItemInfo()
        {
            return info;
        }

        public void Delete()
        {
            GetTree().QueueDelete(this);
        }

        public void SetButtonVisability(bool isVisible)
        {
            deleteButton.Visible = isVisible;
        }

        public void PlaySound(CardSoundType soundType)
        {
            switch(soundType)
            {
                case CardSoundType.Take:
                    audioStreamPlayer.Stream = sounds[0];
                    break;
                case CardSoundType.Laboratory:
                    audioStreamPlayer.Stream = sounds[1];
                    break;
                case CardSoundType.Storage:
                    audioStreamPlayer.Stream = sounds[2];
                    break;
                case CardSoundType.Bar:
                    audioStreamPlayer.Stream = sounds[3];
                    break;
                case CardSoundType.Unit:
                    audioStreamPlayer.Stream = sounds[4];
                    break;
                default:
                    audioStreamPlayer.Stream = sounds[2];
                    break;
            }

            if(audioStreamPlayer.Playing)
            {
                audioStreamPlayer.Stop();
            }

            audioStreamPlayer.Play();
        }
    }

}
