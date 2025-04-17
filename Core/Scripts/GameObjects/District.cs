using Cards;
using DI;
using Godot;
using System;
using System.Collections.Generic;

namespace District
{
	public partial class District : Node2D
	{
		[ExportCategory("Settings")]
		[Export] private int slotCount = 3;
		[Export] private Vector2 size = new Vector2(100, 100);
		[Export] private Vector2 slotSize = new Vector2(0, 0);

		[ExportCategory("Scenes")]
		[Export] private PackedScene cardSlotViewScene;

		private List<ICardSlotObserver> cardSlotObservers;
		private List<Vector2> cardSlotPositions;
	

		[Inject]
		private void Construct(IObjectResolver resolver)
		{
			CreateStartPositions();
			// CreateSlots(resolver);
		}

		public override void _Ready()
		{
			GetNode<ColorRect>("ColorRect").SetSize(size);
			CreateStartPositions();
			CreateSlots();
		}

		private void CreateSlots()
		{
			for(int i = 0; i < slotCount; i++)
			{
				var newInstance = cardSlotViewScene.Instantiate();
				ICardSlot newSLotView = (ICardSlot)newInstance;
				newSLotView.setPosition(cardSlotPositions[i]);
				AddChild(newInstance);
			}
		}

		private void CreateStartPositions()
		{
			cardSlotPositions = new List<Vector2>();
			GD.Print("hello");

			//случай когда можно уместить их горизонтально
			if (size.X >= slotSize.X * slotCount * 1.5)
			{
				float offsetX = size.X / (slotCount + 1);
				float posY = size.Y * .5f;
				for (int i = 1; i <= slotCount; i++)
				{
					cardSlotPositions.Add(new Vector2(offsetX*i, posY));
					GD.Print(offsetX*i, " ", posY);
				}
				GD.Print("horizontal district");
			}
			//случай когда можно уместить их в два ряда
			else if (size.X >= slotSize.X * slotCount * 1.5 * .5)
			{
				float offsetX = size.X / (slotCount * .5f + 1);
				float offsetY = size.Y / 3;
				for (int i = 0; i < slotCount; i++)
				{
					cardSlotPositions.Add(new Vector2(offsetX * (i % (slotCount / 2) + 1), offsetY * ((i / (slotCount / 2)) + 1)));
					GD.Print((i / slotCount) + 1);
				}
				GD.Print("district with two rows");
			}
			//остальной случай помещаем вертикально потому что мне похуй
			else
			{
				float posX = size.X * .5f;
				float offsetY = size.Y / (slotCount + 1);
				for (int i = 1; i <= slotCount; i++)
				{
					cardSlotPositions.Add(new Vector2(posX, offsetY*i));
				}
				GD.Print("vertical district");
			}
		}

		public override void _ExitTree()
		{
			foreach(var cardObserver in cardSlotObservers)
			{
			}
		}
	}
}
