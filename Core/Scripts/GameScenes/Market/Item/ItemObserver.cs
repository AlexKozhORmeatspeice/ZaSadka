using Cards;
using Godot;
using System;

namespace Market
{
    public interface IItemObserver
    {
        void Enable();
        void Disable();
    }

    public partial class ItemObserver : Node, IItemObserver
    {
        [Inject] private IItemsSpawner itemsSpawner;
        [Inject] private IPointerManager pointer;
        [Inject] private IMarketManager marketManager;

        private IItem view;

        private int ID;
        private static int g_MaxID = 0;

        private bool isChoosed;
        
        public ItemObserver(IItem view)
        {
            this.view = view;

            ID = g_MaxID;
            g_MaxID++;
        }

        public void Enable()
        {
            isChoosed = false;
            view.SetChoosed(isChoosed);

            view.WorldPosition = itemsSpawner.GetPositionByID(ID);

            marketManager.onChoosedItem += SetChoosedItem;
            marketManager.onBuyItem += OnBuy;

            pointer.onMove += ChangeScale;
        }

        public void Disable()
        {
            marketManager.onChoosedItem -= SetChoosedItem;
            marketManager.onBuyItem += OnBuy;

            pointer.onMove -= ChangeScale;
        }

        private void ChangeScale()
        {
            Vector2 pointerScreenPos = pointer.NowWorldPosition;
            Vector2 cardScreenPos = view.WorldPosition;

            float dist = pointerScreenPos.DistanceTo(cardScreenPos);
            float mouseDist = view.MouseDistToDetect;

            float scaleAlpha = 1.0f - Mathf.Clamp((dist - mouseDist * 0.5f) / (mouseDist), 0.0f, 1.0f);

            view.ChangeScale(scaleAlpha);
        }

        private void SetChoosedItem(IItem item)
        {
            if (item == null)
                return;

            if (item != view)
            {
                view.SetChoosed(false);
                isChoosed = false;
            }
            else
            {
                view.SetChoosed(true);
                isChoosed = true;
            }
        }

        private void OnBuy(IItem item)
        {
            if (item != view)
                return;

            view.SetChoosed(false);
            isChoosed = false;

            view.SetVisibility(false);
        }
    }

}

