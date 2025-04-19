using Godot;
using System;
using System.Collections.Generic;
using Cards;
using DI;

namespace District
{
	public interface ISlotCardSpawner
	{
		Vector2 GetPositionByID(int id);
	}
	public partial class SlotCardSpawner : Node2D, ISlotCardSpawner
	{
		[Export] private PackedScene cardSlotViewScene;
		private List<ICardSlotObserver> cardSlotObservers;
		private List<Vector2> cardSlotPositions;
		[Export] private Vector2 slotSize = new(100, 130);
		[Inject] IObjectResolver resolver;
		private int slots;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

        public void CreateCards(Vector2 size, int slotCount)
        {
			slots = slotCount;
			CreateStartPositions(size);
			CreateSlots(resolver);
        }

		private void CreateSlots(IObjectResolver resolver)
		{
            for(int i = 0; i < slots; i++)
            {
                var newInstance = cardSlotViewScene.Instantiate();
                AddChild(newInstance);
                ICardSlot newCardView = (ICardSlot)newInstance;
                
                CardSlotObserver observer = new(newCardView);
                resolver.Inject(observer);
                observer.Enable();
                
                cardSlotObservers.Add(observer);
            }
		}

		private void CreateStartPositions(Vector2 size)
		{
			cardSlotPositions = [];

			//случай когда можно уместить их горизонтально
			if (size.X >= slotSize.X * slots * 1.5)
			{
				float offsetX = size.X / (slots + 1);
				float posY = size.Y * .5f;
				for (int i = 1; i <= slots; i++)
				{
					cardSlotPositions.Add(new Vector2(offsetX * i, posY));
					GD.Print(offsetX * i, " ", posY);
				}
				GD.Print("horizontal district");
			}
			//случай когда можно уместить их в два ряда
			else if (size.X >= slotSize.X * slots * 1.5 * .5)
			{
				float offsetX = size.X / (slots * .5f + 1);
				float offsetY = size.Y / 3;
				for (int i = 0; i < slots; i++)
				{
					cardSlotPositions.Add(new Vector2(offsetX * (i % (slots / 2) + 1), offsetY * ((i / (slots / 2)) + 1)));
					GD.Print((i / slots) + 1);
				}
				GD.Print("district with two rows");
			}
			//остальной случай помещаем вертикально потому что мне похуй
			else
			{
				float posX = size.X * .5f;
				float offsetY = size.Y / (slots + 1);
				for (int i = 1; i <= slots; i++)
				{
					cardSlotPositions.Add(new Vector2(posX, offsetY * i));
				}
				GD.Print("vertical district");
			}
		}
		public override void _ExitTree()
		{
			foreach (var cardObserver in cardSlotObservers)
			{
				cardObserver.Disable();
			}
		}

        Vector2 ISlotCardSpawner.GetPositionByID(int id)
        {
            if (id >= cardSlotPositions.Count)
			{
				return Vector2.Inf;
			}
			return cardSlotPositions[id];
        }
    }

}
