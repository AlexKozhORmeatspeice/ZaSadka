using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI;
using Godot;
using ZaSadka;

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

        [Inject] private IInventoryManager inventoryManager;
        [Inject] private IDistrictsManager districtsManager;

        private Vector2 posBeforeMove;

        private bool isDragging = false;

        private int ID;
        private static int g_MAXID = 0;

        private ICardView cardView;
        private ICardSlot nowSlot;
        private ItemInfo itemInfo;
        private bool isEnabled = true;

        public CardObserver(ICardView cardView, bool enabled = true)
        {
            this.cardView = cardView;

            ID = g_MAXID;
            g_MAXID++;

            isEnabled = enabled;

        }

        public void Enable()
        {
            nowSlot = null;

            isDragging = false;

            cardSpawner.onUpdatePosition += SetPosition;
            cardSpawner.onUpdateInfo += SetInfo;
            
            pointer.onMove += MoveCard;
            pointer.onMove += ChangeScale;
            
            cardMouseManager.onPointerDown += CheckCardOnDown;
            cardMouseManager.onPointerUp += CheckCardOnUp;

            districtsManager.onAddCard += SetInSlot;
            inventoryManager.onAddItem += SetInInventory;

        }

        public void Disable()
        {
            cardSpawner.onUpdatePosition -= SetPosition;
            cardSpawner.onUpdateInfo -= SetInfo;

            pointer.onMove -= MoveCard;
            pointer.onMove -= ChangeScale;
            
            cardMouseManager.onPointerDown -= CheckCardOnDown;
            cardMouseManager.onPointerUp -= CheckCardOnUp;

            districtsManager.onAddCard -= SetInSlot;
            inventoryManager.onAddItem -= SetInInventory;
        }

        private void CheckCardOnDown(ICardView card)
        {
            if (card != cardView)
            {
                isDragging = false;
                return;
            }

            isDragging = true;
        }

        private void CheckCardOnUp(ICardView card)
        {
            isDragging = false;
        }

        private void SetInSlot(ICardSlot slot, ICardView card)
        {
            if (card != cardView)
            {
                if(nowSlot == null)
                {
                    SetStartPos();
                }

                return;
            }

            nowSlot = slot;
            cardView.WorldPosition = slot.WorldPosition;
        }

        private void SetInInventory(List<ItemInfo> items)
        {
            foreach (ItemInfo item in items)
            {
                if(item.uniqueID == cardView.GetItemInfo().uniqueID)
                {
                    SetStartPos();
                    nowSlot = null;
                    return;
                }
            }
        }

        private void MoveCard()
        {
            if (!isDragging)
            {
                if(nowSlot == null)
                {
                    SetStartPos();
                }
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
            if (nowSlot != null)
            {
                cardView.ChangeScale(0.0f);
                return;
            }

            Vector2 pointerScreenPos = pointer.NowWorldPosition;
            Vector2 cardScreenPos = cardView.WorldPosition; 
            
            float dist = pointerScreenPos.DistanceTo(cardScreenPos);
            float mouseDist = cardView.MouseDistToDetect;

            float scaleAlpha = 1.0f - Mathf.Clamp((dist - mouseDist * 0.5f) /(mouseDist), 0.0f, 1.0f);

            cardView.ChangeScale(scaleAlpha);
        }

        private void SetInfo(ICardView card, ItemInfo info)
        {
            if (card != cardView)
                return;

            itemInfo = info;
            cardView.SetInfo(info, isEnabled);
        }

        private void SetPosition(ICardView card,  Vector2 pos)
        {
            if (card != cardView)
                return;

            posBeforeMove = pos;            
            cardView.WorldPosition = pos;
        }

        private void SetStartPos()
        {
            cardView.WorldPosition = posBeforeMove;
        }

    }
}
