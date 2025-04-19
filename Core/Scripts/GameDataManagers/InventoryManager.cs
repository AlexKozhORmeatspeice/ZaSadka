using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;

namespace ZaSadka
{
    public interface IInventoryManager
    {
        event Action<ItemInfo> onAddItem;
        event Action<ItemInfo> onRemoveItem;

        void AddItem(ItemInfo item);
        void RemoveItem(ItemInfo item);

        List<ItemInfo> GetItems();
    }

    public partial class InventoryManager : IInventoryManager
    {
        static private List<ItemInfo> items = [];

        public event Action<ItemInfo> onAddItem;
        public event Action<ItemInfo> onRemoveItem;

        public void AddItem(ItemInfo item)
        {
            items.Add(item);
            GD.Print(items.Count);
            onAddItem?.Invoke(item);
        }

        public void RemoveItem(ItemInfo item)
        {
            items.Remove(item);

            onRemoveItem?.Invoke(item);
        }

        List<ItemInfo> IInventoryManager.GetItems()
        {
            GD.Print(items.Count);
            return items;
        }


    }
}

