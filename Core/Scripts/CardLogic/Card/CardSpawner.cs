using DI;
using Godot;
using System;
using System.Collections.Generic;


namespace Cards
{
    public interface ICardSpawner
    {
        Vector2 GetPositionByID(int id);
        Vector2 CardsScreenZone { get; }
    }

    public partial class CardSpawner : Node2D, ICardSpawner
    {
        [ExportCategory("Settings")]
        [Export] private int cardCount = 3;
        [Export] private float cardDistance = 100;
        [Export] private float startY;

        [ExportCategory("Scenes")]
        [Export] private PackedScene cardViewScene;
        
        
        private List<ICardObserver> cardObservers;
        private List<Vector2> cardPositions;

        public Vector2 CardsScreenZone => GetViewportRect().Size;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            CreateCards(resolver);
            
        }

        public override void _Ready()
        {
            CreateStartPositions();
        }

        private void CreateCards(IObjectResolver resolver)
        {
            //get slots for cards
            cardObservers = new List<ICardObserver>();

            for(int i = 0; i < cardCount; i++)
            {
                var newInstance = cardViewScene.Instantiate();
                AddChild(newInstance);
                ICardView newCardView = (ICardView)newInstance;
                
                CardObserver cardObserver = new CardObserver(newCardView);
                resolver.Inject(cardObserver);
                cardObserver.Enable();
                
                cardObservers.Add(cardObserver);
            }
        }

        private void CreateStartPositions()
        {
            cardPositions = new List<Vector2>();

            float startX = -cardCount / 2 * cardDistance;

            for (int i = 0; i < cardCount;i++)
            {
                cardPositions.Add(new Vector2(startX, startY));

                startX += cardDistance;
            }
            GD.Print(startY);
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
    }
}
