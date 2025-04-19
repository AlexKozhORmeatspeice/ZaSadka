using DI;
using Godot;
using System;
using System.Collections.Generic;
using ZaSadka;

//TODO: либо впихнуть интерфейс дополнительный юнита сюда же и просто им пользоваться, если тип карты совпал, либо сделать отдельную карту?? надо подумать

namespace Cards
{
    public interface ICardSpawner
    {
        Vector2 GetPositionByID(int id);
        Vector2 CardsScreenZone { get; }
        ItemInfo GetItemInfoByID(int id);
    }

    public partial class CardSpawner : Node2D, ILateStartable, ICardSpawner
    {
        [ExportCategory("Settings")]
        [Export] private float cardDistance = 100;
        [Export] private float startY;

        [ExportCategory("Scenes")]
        [Export] private PackedScene cardViewScene;
        [Inject] private IInventoryManager inventoryManager;
        [Inject] private IObjectResolver resolver;
        
        
        private List<ICardObserver> cardObservers;
        private List<Vector2> cardPositions;
        private List<ItemInfo> cards;

        public Vector2 CardsScreenZone => GetViewportRect().Size;

        public override void _Ready()
        {
        }

        private void CreateCards(IObjectResolver resolver)
        {
            //get slots for cards
            cardObservers = [];

            for(int i = 0; i < cards.Count; i++)
            {
                var newInstance = cardViewScene.Instantiate();
                AddChild(newInstance);
                ICardView newCardView = (ICardView)newInstance;
                
                CardObserver cardObserver = new(newCardView);
                resolver.Inject(cardObserver);
                cardObserver.Enable();
                
                cardObservers.Add(cardObserver);
            }
        }

        private void CreateStartPositions()
        {
            GD.Print("1");
            cardPositions = new List<Vector2>();
            GD.Print("2");

            float startX = -cards.Count / 2 * cardDistance;
            GD.Print(cards.Count);

            for (int i = 0; i < cards.Count; i++)
            {
                GD.Print("4");
                cardPositions.Add(new Vector2(startX, startY));
                GD.Print("5");

                startX += cardDistance;
                GD.Print("6");
            }
        }

        public override void _ExitTree()
        {
            foreach(var cardObserver in cardObservers)
            {
                cardObserver.Disable();
            }
        }


        Vector2 ICardSpawner.GetPositionByID(int id)
        {
            if (id > cardPositions.Count - 1)
                return Vector2.Inf;

            return cardPositions[id];
        }

        void ILateStartable.LateStart()
        {
            cards = inventoryManager.GetItems();
            CreateStartPositions();
            CreateCards(resolver);
        }

        ItemInfo ICardSpawner.GetItemInfoByID(int id)
        {
            if (id >= cards.Count)
            {
                return new ItemInfo();
            }
            return cards[id];
        }
    }
}
