using Cards;
using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;

namespace ZaSadka
{
    public interface IInventoryManager
    {
        event Action<List<ItemInfo>> onAddItem;
        event Action<List<ItemInfo>> onRemoveItem;

        void AddItem(ItemInfo item);
        void RemoveItem(ItemInfo item);

        List<ItemInfo> GetItems();
    }

    public partial class InventoryManager : IInventoryManager, IStartable, IDispose
    {
        [Inject] private IDistrictsManager districtsManager;

        static private List<ItemInfo> items = [];

        public event Action<List<ItemInfo>> onAddItem;
        public event Action<List<ItemInfo>> onRemoveItem;

        public void Start()
        {
            if(districtsManager != null)
            {
                districtsManager.onAddCard += RemoveCardWhenInSlot;
                districtsManager.onRemoveCard += AddCardFromSlot;
            }
        }

        public void Dispose()
        {
            if(districtsManager != null)
            {
                districtsManager.onAddCard -= RemoveCardWhenInSlot;
                districtsManager.onRemoveCard -= AddCardFromSlot;
            }
        }

        public void AddItem(ItemInfo item)
        {
            items.Add(item);

            onAddItem?.Invoke(items);
        }

        public void RemoveItem(ItemInfo item)
        {
            items.Remove(item);

            onRemoveItem?.Invoke(items);
        }


        List<ItemInfo> IInventoryManager.GetItems()
        {
            return items;
        }

        private void RemoveCardWhenInSlot(ICardSlot slot, ICardView card)
        {
            if (slot == null)
                return;

            RemoveItem(card.GetItemInfo());
        }

        private void AddCardFromSlot(ICardSlot slot, ICardView card)
        {
            AddItem(card.GetItemInfo());
        }
    }
}

