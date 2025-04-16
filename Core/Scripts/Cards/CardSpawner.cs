using DI;
using Godot;
using System;
using System.Collections.Generic;


namespace Cards
{
    public interface ICardSpawner
    {

    }

    public partial class CardSpawner : Node2D, ICardSpawner
    {
        [ExportCategory("Settings")]
        [Export] private int numOfCards = 3;
        [Export] private float cardDistance = 100;
        [Export] private Transform2D startSpawnPos;

        [ExportCategory("Scenes")]
        [Export] private PackedScene cardViewScene;
        
        private List<ICardObserver> cardObservers;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            CreateCards(resolver);
        }


        private void CreateCards(IObjectResolver resolver)
        {
            cardObservers = new List<ICardObserver>();

            for(int i = 0; i < numOfCards; i++)
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

        public override void _ExitTree()
        {
            foreach(var cardObserver in cardObservers)
            {
                cardObserver.Disable();
            }
        }
    }
}
