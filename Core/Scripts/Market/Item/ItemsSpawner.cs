using Cards;
using DI;
using Godot;
using System;
using System.Collections.Generic;
using static Godot.WebSocketPeer;

namespace Market
{
    public interface IItemsSpawner
    {
        Vector2 GetPositionByID(int id);
    }

    public partial class ItemsSpawner : Node2D, IItemsSpawner
    {
        [Export] private int itemsCount;
        [Export] private PackedScene[] packedScenes;
        
        [Export] private float startY;
        [Export] private float itemDistance;

        private List<IItemObserver> itemObservers;
        private List<Vector2> itemPositions;

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
            itemObservers = new List<IItemObserver>();

            for (int i = 0; i < itemsCount; i++)
            {
                int randomItemInd = GD.RandRange(0, packedScenes.Length - 1);
                
                var newInstance = packedScenes[randomItemInd].Instantiate();
                AddChild(newInstance);
                IItem newCardView = (IItem)newInstance;

                ItemObserver itemObserver = new ItemObserver(newCardView);
                resolver.Inject(itemObserver);
                itemObserver.Enable();

                itemObservers.Add(itemObserver);
            }
        }

        private void CreateStartPositions()
        {
            itemPositions = new List<Vector2>();

            float startX = -itemsCount / 2 * itemDistance;

            for (int i = 0; i < itemsCount; i++)
            {
                itemPositions.Add(new Vector2(startX, startY));

                startX += itemDistance;
            }
        }

        public override void _ExitTree()
        {
            foreach (var itemObserver in itemObservers)
            {
                itemObserver.Disable();
            }
        }


        Vector2 IItemsSpawner.GetPositionByID(int id)
        {
            if (id > itemPositions.Count - 1)
                return Vector2.Inf;

            return itemPositions[id];
        }
    }

}

