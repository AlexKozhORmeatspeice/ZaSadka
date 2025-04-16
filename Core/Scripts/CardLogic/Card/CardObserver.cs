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
        [Inject] private ISlotMouseManager slotMouseManager;
        [Inject] private IPointerManager pointer;
        [Inject] private ICardSpawner cardSpawner;
        [Inject] private ICardMouseManager cardMouseManager;

        private Vector2 posBeforeMove;

        private bool isDragging = false;
        private bool isInSlot = false;

        private int ID;
        private static int g_MAXID = 0;

        private ICardView cardView;

        public CardObserver(ICardView cardView)
        {
            this.cardView = cardView;

            ID = g_MAXID;
            g_MAXID++;
        }

        public void Enable()
        {
            isInSlot = false;

            isDragging = false;

            posBeforeMove = cardView.WorldPosition;

            cardView.WorldPosition = cardSpawner.GetPositionByID(ID);

            pointer.onMove += MoveCard;
            pointer.onMove += ChangeScale;
            
            cardMouseManager.onPointerDown += CheckCardOnDown;
        }

        public void Disable()
        {
            pointer.onMove -= MoveCard;
            pointer.onMove -= ChangeScale;
            
            cardMouseManager.onPointerDown -= CheckCardOnDown;

            slotMouseManager.onPointerUp -= SetInSlot;
        }

        private void CheckCardOnDown(ICardView card)
        {
            if (card != cardView)
            {
                isDragging = false;
                return;
            }

            isDragging = true;
            slotMouseManager.onPointerUp += SetInSlot;
        }

        private void SetInSlot(ICardSlot slot)
        {
            isDragging = false;

            if (slot == null)
            {
                isInSlot = false;
                SetPosBeforeMove();
                return;
            }

            isInSlot = true;
            cardView.WorldPosition = slot.WorldPosition;

            slotMouseManager.onPointerUp -= SetInSlot;
        }

        private void MoveCard()
        {
            if (!isDragging)
            {
                posBeforeMove = cardView.WorldPosition;
                return;
            }

            //TODO: поправить чтобы ограничения не зависили от положения камеры.
            //Сейчас идет привязка к 0 координате

            Vector2 screenRect = cardSpawner.CardsScreenZone;

            Vector2 pointerPos = pointer.NowWorldPosition;
            pointerPos.X = Mathf.Clamp(pointerPos.X, -screenRect.X / 2.0f, screenRect.X / 2.0f);
            pointerPos.Y = Mathf.Clamp(pointerPos.Y, -screenRect.Y / 2.0f, screenRect.Y / 2.0f);

            cardView.WorldPosition = pointerPos;
        }

        private void ChangeScale()
        {
            Vector2 pointerScreenPos = pointer.NowWorldPosition;
            Vector2 cardScreenPos = cardView.WorldPosition; 
            
            float dist = pointerScreenPos.DistanceTo(cardScreenPos);
            float mouseDist = cardView.MouseDistToDetect;

            float scaleAlpha = 1.0f - Mathf.Clamp((dist - mouseDist * 0.5f) /(mouseDist), 0.0f, 1.0f);

            cardView.ChangeScale(scaleAlpha);
        }

        private void SetPosBeforeMove()
        {
            cardView.WorldPosition = posBeforeMove;
        }
    }
}
