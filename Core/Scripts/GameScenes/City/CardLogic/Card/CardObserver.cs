using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI;
using Godot;
using Market;
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

            cardView.SetButtonVisability(false);

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

            cardMouseManager.onPointerEnter -= CheckButtonOnEnter;
            cardMouseManager.onPointerLeave -= CheckButtonOnExit;
            cardView.onClick -= DeleteCard;
        }

        public void SetInSlotBuildingCard()
        {
            Disable();
            GD.Print("Got card in building slot");

            cardView.onClick += DeleteCard;
            cardMouseManager.onPointerEnter += CheckButtonOnEnter;
            cardMouseManager.onPointerLeave += CheckButtonOnExit;
        }

        private void CheckButtonOnEnter(ICardView view)
        {
            if (cardView != view)
            {
                cardView.SetButtonVisability(false);
                return;
            }

            cardView.SetButtonVisability(true);
        }

        private void CheckButtonOnExit(ICardView view)
        {
            cardView.SetButtonVisability(false);
        }

        private void DeleteCard()
        {
            districtsManager.PermanentDelete(cardView);
            inventoryManager.RemoveItem(itemInfo);

            cardView.SetInfo(new ItemInfo(), false);

            Disable();
        }

        private void CheckCardOnDown(ICardView card)
        {
            if (card != cardView)
            {
                isDragging = false;
                return;
            }

            cardView.PlaySound(CardSoundType.Take);
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
                if (nowSlot == null)
                {
                    SetStartPos();
                }

                return;
            }


            ChooseInSlotSound();
            nowSlot = slot;
            cardView.WorldPosition = slot.WorldPosition;
        }

        private void SetInInventory(List<ItemInfo> items)
        {
            foreach (ItemInfo item in items)
            {
                if (item.uniqueID == cardView.GetItemInfo().uniqueID)
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
                if (nowSlot == null)
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

            float scaleAlpha = 1.0f - Mathf.Clamp((dist - mouseDist * 0.5f) / (mouseDist), 0.0f, 1.0f);

            cardView.ChangeScale(scaleAlpha);
        }

        private void SetInfo(ICardView card, ItemInfo info)
        {
            if (card != cardView)
                return;

            itemInfo = info;
            cardView.SetInfo(info, isEnabled);
        }

        private void SetPosition(ICardView card, Vector2 pos)
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

        private void ChooseInSlotSound()
        {
            switch (itemInfo.type)
            {
                case ItemType.building:
                    switch(itemInfo.spriteId)
                    {
                        case 0:
                            cardView.PlaySound(CardSoundType.Bar);
                            break;
                        case 1:
                            cardView.PlaySound(CardSoundType.Laboratory);
                            break;
                        default:
                        case 2:
                            cardView.PlaySound(CardSoundType.Storage);
                            break;

                    }
                    break;
                case ItemType.unit:
                    cardView.PlaySound(CardSoundType.Unit);
                    break;
                default:
                    break;
            }
        }
    }
}
