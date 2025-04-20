using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using District;

//TODO: сделать, чтобы в слотах сохранялись билдинги и их нельзя было оттуда вытащить

namespace Cards
{
    public interface ICardSlotObserver
    {
        void Enable();
        void Disable();
    }

    internal class CardSlotObserver : ICardSlotObserver
    {
        [Inject] private ISlotCardSpawner slotSpawner;
        private ICardSlot view;
        private int ID;
        private static int g_MAXID = 0;
        public CardSlotObserver(ICardSlot cardSlot)
        {
            this.view = cardSlot;
            ID = g_MAXID;
            g_MAXID++;
        }
        public void Enable()
        {
            view.WorldPosition = slotSpawner.GetPositionByID(ID);
        }

        public void Disable()
        {

        }
    }
}
