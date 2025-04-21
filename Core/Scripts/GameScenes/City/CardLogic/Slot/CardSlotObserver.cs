using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Godot;
using ZaSadka;

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
        [Inject] private IJsonDistrictManager districtJsonManager;
        [Inject] private IDistrictsManager districtsManager;

        private DistrictInfo info;

        private ICardSlot view;
        private ICardView cardOnSlot;

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
            info = LoadInfo();

            districtsManager.onAddCard += OnAddCard;
            districtsManager.onRemoveCard += OnRemoveCard;

            view._DistrictInfo = info;
        }

        public void Disable()
        {

        }

        private DistrictInfo LoadInfo()
        {
            return districtJsonManager.GetDistrictInfo((int)view._DistrictName);
        }

        private void OnAddCard(ICardSlot slot, ICardView card)
        {
            if(slot != view)
            {
                return;
            }

            cardOnSlot = card;

            view.SupplyText = (info.supply + card.GetItemInfo().supply).ToString();
            view.DemandText = (info.demand + card.GetItemInfo().demand).ToString();
            view.InfluenceText = (info.influence + card.GetItemInfo().influence).ToString();
            view.SusText = (info.suspicion + card.GetItemInfo().suspicion).ToString();
        }

        private void OnRemoveCard(ICardSlot slot, ICardView card)
        {
            if (slot != view)
                return;

            SetBaseInfo();
        }

        private void SetBaseInfo()
        {
            view._DistrictInfo = info;
        }
    }
}
