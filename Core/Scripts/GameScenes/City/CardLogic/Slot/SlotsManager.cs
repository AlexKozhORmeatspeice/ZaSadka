using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;

namespace Cards
{
    public interface ISlotsManager
    {
        ICardSlot GetSlotByID(int slotID);
    }

    public partial class SlotsManager : Node2D, ISlotsManager, ILateStartable
    {
        [Inject] private IObjectResolver resolver;
        
        [Export] private CardSlot[] slots;

        private List<ICardSlotObserver> observers;

        public ICardSlot GetSlotByID(int slotID)
        {
            if(slotID > slots.Length - 1)
                return null;

            return slots[slotID];
        }

        public void LateStart()
        {
            observers = new List<ICardSlotObserver>();

            foreach (var slot in slots)
            {
                ICardSlotObserver observer = new CardSlotObserver(slot);

                resolver.Inject(observer);
                observer.Enable();

                observers.Add(observer);
            }
        }

        public override void _ExitTree()
        {
            foreach(var observer in observers)
            {
                observer.Disable();
            }
        }
    }
}

