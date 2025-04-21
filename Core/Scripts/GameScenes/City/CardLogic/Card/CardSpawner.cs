using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;
using ZaSadka;

//TODO: либо впихнуть интерфейс дополнительный юнита сюда же и просто им пользоваться, если тип карты совпал, либо сделать отдельную карту?? надо подумать

namespace Cards
{
    public interface ICardSpawner
    {
        event Action<ICardView, Vector2> onUpdatePosition;
        event Action<ICardView, ItemInfo> onUpdateInfo;
        event Action<ICardView, int> onUpdateCardInSlotID;
        Vector2 CardsScreenZone { get; }
    }

    public partial class CardSpawner : Node2D, ILateStartable, ICardSpawner
    {
        [Inject] private IInventoryManager inventoryManager;
        [Inject] private IDistrictsManager districtsManager;

        [Inject] private IObjectResolver resolver;

        [ExportCategory("Settings")]
        [Export] private float cardDistance = 100;
        [Export] private float startY;

        [ExportCategory("Scenes")]
        [Export] private PackedScene cardViewScene;

        private List<ICardObserver> cardObservers;
        private Dictionary<int, ICardView> cardByID;
        
        public event Action<ICardView, Vector2> onUpdatePosition;
        public event Action<ICardView, ItemInfo> onUpdateInfo;
        public event Action<ICardView, int> onUpdateCardInSlotID;

        public Vector2 CardsScreenZone => GetViewportRect().Size;

        void ILateStartable.LateStart()
        {
            CreateCards(inventoryManager.GetItems());
            CreatePositions(inventoryManager.GetItems());
            CreateInSlotCard();

            inventoryManager.onAddItem += CreatePositions;
            inventoryManager.onRemoveItem += CreatePositions;
        }

        private void CreateCards(List<ItemInfo> items)
        {
            //get slots for cards
            cardObservers = [];
            cardByID = new Dictionary<int, ICardView>();

            for (int i = 0; i < items.Count; i++)
            {
                ItemInfo nowItem = items[i];

                var newInstance = cardViewScene.Instantiate();
                AddChild(newInstance);
                ICardView newCardView = (ICardView)newInstance;

                CardObserver cardObserver = new(newCardView);
                resolver.Inject(cardObserver);
                cardObserver.Enable();

                onUpdateInfo?.Invoke(newCardView, nowItem);

                cardByID[nowItem.uniqueID] = newCardView;
                cardObservers.Add(cardObserver);
            }
        }

        private void CreatePositions(List<ItemInfo> items)
        {
            float startX = -inventoryManager.GetItems().Count / 2 * cardDistance;

            for (int i = 0; i < items.Count; i++)
            {
                ItemInfo item = items[i];

                if(cardByID.ContainsKey(item.uniqueID))
                {
                    onUpdatePosition?.Invoke(cardByID[item.uniqueID], new Vector2(startX, startY));
                }

                startX += cardDistance;
            }
        }

        public void CreateInSlotCard()
        {
            for(int i = 0; i < (int)DistrictName.DistrictNameNum; i++)
            {
                DistrictName name = (DistrictName)i;

                Dictionary<int, ItemInfo> dict = districtsManager.GetData(name).itemInfoByID;
                foreach(int id in dict.Keys)
                {
                    ItemInfo nowItem = dict[id];
                    
                    if (nowItem.uniqueID < 0)
                        continue;

                    var newInstance = cardViewScene.Instantiate();
                    AddChild(newInstance);
                    ICardView newCardView = (ICardView)newInstance;

                    CardObserver cardObserver = new(newCardView);
                    resolver.Inject(cardObserver);
                    cardObserver.Enable();
                    
                    onUpdateInfo?.Invoke(newCardView, nowItem);
                    onUpdateCardInSlotID?.Invoke(newCardView, id);

                    cardByID[nowItem.uniqueID] = newCardView;
                    cardObservers.Add(cardObserver);
                }
            }
        }

        public override void _ExitTree()
        {
            foreach(var cardObserver in cardObservers)
            {
                cardObserver.Disable();
            }

            inventoryManager.onAddItem -= CreatePositions;
            inventoryManager.onRemoveItem -= CreatePositions;
        }
    }
}
