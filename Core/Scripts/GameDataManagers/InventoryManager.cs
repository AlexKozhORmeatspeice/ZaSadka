using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;

namespace ZaSadka
{
    public interface IInventoryManager
    {
        event Action<IItem> onAddItem;
        event Action<IItem> onRemoveItem;

        void AddItem(IItem item);
        void RemoveItem(IItem item);
    }

    public partial class InventoryManager : Node2D, IInventoryManager, IStartable
    {
        private List<IItem> items;

        public event Action<IItem> onAddItem;
        public event Action<IItem> onRemoveItem;

        public void AddItem(IItem item)
        {
            items.Add(item);
            onAddItem?.Invoke(item);
        }

        public void RemoveItem(IItem item)
        {
            items.Remove(item);

            onRemoveItem?.Invoke(item);
        }

        public void Start()
        {
            items = new List<IItem>();
        }

    }
}

