using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI;
using Godot;

namespace Cards
{
    public interface ICardObserver
    {
        void Enable();
        void Disable();
    }
    internal class CardObserver : ICardObserver
    {
        [Inject] private IPointerManager pointer;
        [Inject] private ICardMouseManager cardMouseManager;
        private bool isDragging = false;

        private ICardView cardView;

        public CardObserver(ICardView cardView)
        {
            this.cardView = cardView;
        }

        public void Enable()
        {
            isDragging = false;

            pointer.onMove += MoveCard;
            cardMouseManager.onPointerDown += CheckCard;
        }

        public void Disable()
        {
            
        }

        private void CheckCard(ICardView card)
        {
            GD.Print(card);
            if (card != cardView)
            {
                isDragging = false;
                return;
            }

            isDragging = true;
        }

        private void MoveCard()
        {
            if (isDragging)
            {
                cardView.WorldPosition = pointer.NowWorldPosition;
            }
        }
    }
}
