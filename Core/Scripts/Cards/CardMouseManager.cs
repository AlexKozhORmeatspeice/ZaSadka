using DI;
using Godot;
using System;

namespace Cards
{
    public interface ICardMouseManager
    {
        event Action<ICardView> onPointerEnter;
        event Action<ICardView> onPointerHover;
        event Action<ICardView> onPointerLeave;
        event Action<ICardView> onPointerDown;
        event Action<ICardView> onPointerUp;
    }

    public partial class CardMouseManager : Node2D, ICardMouseManager, IStartable
    {
        [Inject] private IPointerManager pointer;

        private ICardView hoveredCard;

        public event Action<ICardView> onPointerEnter;
        public event Action<ICardView> onPointerHover;
        public event Action<ICardView> onPointerLeave;
        public event Action<ICardView> onPointerDown;
        public event Action<ICardView> onPointerUp;

        public void Start()
        {
            pointer.onMove += CheckCards;
            pointer.onPointerDown += OnClickCard;
            pointer.onPointerUp += OnReleaseCard;
        }

        public override void _ExitTree()
        {
            pointer.onMove -= CheckCards;
            pointer.onPointerDown -= OnClickCard;
            pointer.onPointerUp -= OnReleaseCard;
        }

        private void CheckCards()
        {
            var newCard = pointer.RaycastByType(typeof(ICardView)) as ICardView;

            if (newCard == null)
            {
                if (hoveredCard != null)
                {
                    onPointerLeave?.Invoke(hoveredCard);
                    hoveredCard = null;
                }
                return;
            }

            if (newCard != hoveredCard)
            {
                if (hoveredCard != null)
                {
                    onPointerLeave?.Invoke(hoveredCard);
                }

                hoveredCard = newCard;
                onPointerEnter?.Invoke(hoveredCard);
            }

            onPointerHover?.Invoke(hoveredCard);
        }

        private void OnClickCard()
        {
            if (hoveredCard == null)
            {
                onPointerDown?.Invoke(hoveredCard);
            }
        }

        private void OnReleaseCard()
        {
            if (hoveredCard == null)
            {
                onPointerUp?.Invoke(hoveredCard);
            }
        }
    }
}
