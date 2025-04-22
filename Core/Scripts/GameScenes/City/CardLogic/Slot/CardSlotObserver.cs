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
        [Inject] private ISuspicionManager suspicionManager;
        [Inject] private IInfluenceManager influenceManager;

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

            view._DistrictInfo = info;

            int currentSuspicion = suspicionManager.GetValue(view._DistrictName);
            if (currentSuspicion == int.MinValue)
            {
                currentSuspicion = 0;
            }
            view.SusText = currentSuspicion.ToString();

            int currentInfluence = influenceManager.GetValue(view._DistrictName);
            if (currentInfluence == int.MinValue)
            {
                currentInfluence = 0;
            }
            view.InfluenceText = currentInfluence.ToString();

            districtsManager.onAddCard += OnAddCard;

            suspicionManager.onSuspicionChange += onSuspicionChange;
            influenceManager.onInfluenceChange += onInfluenceChange;
        }

        public void Disable()
        {
            districtsManager.onAddCard -= OnAddCard;
            suspicionManager.onSuspicionChange -= onSuspicionChange;
            influenceManager.onInfluenceChange -= onInfluenceChange;
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
        }

        private void onSuspicionChange(DistrictName districtName, int newSuspicion)
        {
            if (view._DistrictName != districtName)
            {
                return;
            }
            newSuspicion = Math.Clamp(newSuspicion, 0, 10);
            view.SusText = newSuspicion.ToString();
        }
        private void onInfluenceChange(DistrictName districtName, int newInfluence)
        {
            if (view._DistrictName != districtName)
            {
                return;
            }
            newInfluence = Math.Clamp(newInfluence, 0, 10);
            view.InfluenceText = newInfluence.ToString();
        }

    }
}
