using Cards;
using DI;
using Godot;
using System;
using System.Collections.Generic;
using static Godot.WebSocketPeer;
using ZaSadka;

namespace Market
{
    public interface IItemsSpawner
    {
        event Action<ItemInfo, Vector2> onUpdatePos; 
        event Action<IItemObserver, ItemInfo> onUpdateInfo;
    }

    public partial class ItemsSpawner : Node2D, IItemsSpawner, ILateStartable
    {
        [Export] private int itemsCount;
        [Export] private string cardViewPath;
        
        [Export] private float startY;
        [Export] private float itemDistance;

        private List<IItemObserver> itemObservers;
        private List<Vector2> itemPositions;
        private List<ItemInfo> itemData;

        public Vector2 CardsScreenZone => GetViewportRect().Size;
        [Inject] private IJsonCardManager jsonCardManager;
        [Inject] private IObjectResolver resolver;

        public event Action<ItemInfo, Vector2> onUpdatePos;
        public event Action<IItemObserver, ItemInfo> onUpdateInfo;

        public void LateStart()
        {
            CreateCards(resolver);

            CreateItemData();
            CreateStartPositions();
        }

        private void CreateCards(IObjectResolver resolver)
        {
            itemObservers = [];

            for (int i = 0; i < itemsCount; i++)
            {
                var newInstance = GD.Load<PackedScene>(cardViewPath).Instantiate();
                AddChild(newInstance);
                IItem newCardView = (IItem)newInstance;

                newCardView.SetVisibility(true);

                ItemObserver itemObserver = new(newCardView);
                
                resolver.Inject(itemObserver);
                itemObserver.Enable();
                itemObservers.Add(itemObserver);
            }
        }

        private void CreateStartPositions()
        {
            itemPositions = [];

            float startX = -itemsCount / 2 * itemDistance;

            for (int i = 0; i < itemData.Count; i++)
            {
                onUpdatePos?.Invoke(itemData[i], new Vector2(startX, startY));
                
                startX += itemDistance;
            }
        }
        void CreateItemData()
        {
            itemData = new List<ItemInfo>();
            
            for (int i = 0; i < itemsCount; i++)
            {
                ItemType itemType = (ItemType)GD.RandRange(0, 1);
                int itemID = GD.RandRange(0, jsonCardManager.GetCardsAmount(itemType) - 1);

                ItemInfo itemInfo = jsonCardManager.GetItemInfo(itemType, itemID);
                itemData.Add(itemInfo);

                onUpdateInfo?.Invoke(itemObservers[i], itemInfo);
            }
        }

        public override void _ExitTree()
        {
            foreach (var itemObserver in itemObservers)
            {
                itemObserver.Disable();
            }
        }
    }
}

