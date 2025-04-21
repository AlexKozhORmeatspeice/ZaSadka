using DI;
using Godot;
using System;
using System.Reflection;
using ZaSadka;

namespace Market
{
    public interface IMarketManager
    {
        event Action<IItem> onChoosedItem;
        event Action<IItem> onBuyItem;

        void BuyChoosedItem();
    }

    public partial class MarketManager : Node2D, IMarketManager, IStartable
    {
        [Inject] private IItemMouseManager itemMouseManager;
        [Inject] private IMoneyManager moneyManager; //TODO: перенести логику внутрь MoneyManager. (меня бесит, что оно здесь, но уже 7 утра и я бля хочу поспать чутка)
        [Inject] private IInventoryManager inventoryManager;

        private IItem choosedItem;

        public event Action<IItem> onChoosedItem;
        public event Action<IItem> onBuyItem;

        public void Start()
        {
            itemMouseManager.onPointerDown += SetChoosedItem;
        }

        public override void _ExitTree()
        {
            itemMouseManager.onPointerDown -= SetChoosedItem;
        }

        private void SetChoosedItem(IItem _choosedItem)
        {
            if (_choosedItem == null)
                return;

            choosedItem = _choosedItem;

            onChoosedItem?.Invoke(choosedItem);
        }

        public void BuyChoosedItem()
        {
            if (choosedItem == null)
                return;

            if(moneyManager.ChangeMoney(-choosedItem.Price))
            {
                onBuyItem?.Invoke(choosedItem);
                ItemInfo itemInfo = choosedItem.GetInfo();
                inventoryManager.AddItem(itemInfo);
            }

            choosedItem = null;
        }
    }

}

