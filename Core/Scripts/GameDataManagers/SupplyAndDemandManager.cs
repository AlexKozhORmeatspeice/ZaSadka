using Cards;
using DI;
using Game_events;
using Godot;
using Market;
using System;
using System.Collections.Generic;

namespace ZaSadka
{
	public interface ISupplyDemandManager
    {
        void ChangeDemand(int value);
		event Action<int> OnChangeDemand;
		void ChangeSupply(int value);
		event Action<int> OnChangeSupply;
		int GetCurrentDemand();

        int MaxSupply { get;  }
        int NowSupply { get; }
        int MaxDemand { get; }
        int NowDemand { get; }

        int GetProfit();
    }
	
	public partial class SupplyAndDemandManager : ISupplyDemandManager, IStartable, IDisposable
	{
        [Inject] private IEventsManager eventsManager;
		[Inject] private IDistrictsManager districtsManager;

		private static int supply = 0;
		private static int demand = 0;

		private const int minSupply = 0;
        private const int maxSupply = 30;
        private const int minDemand = 0;
        private const int maxDemand = 30;

        public int MaxSupply => maxSupply;

        public int MaxDemand => maxDemand;

        public int NowSupply => supply;

        public int NowDemand => demand;

        public event Action<int> OnChangeDemand;
		public event Action<int> OnChangeSupply;

        public void Start()
        {
			districtsManager.onAddCard += OnAddCard;
			districtsManager.onRemoveCard += OnRemoveCard;

            if (eventsManager != null)
                eventsManager.onChoiceActivate += OnChoice;

            if (MenuDI.instance != null)
            {
                ClearData();
            }
        }

        public void Dispose()
        {
            districtsManager.onAddCard -= OnAddCard;
            districtsManager.onRemoveCard -= OnRemoveCard;

            if (eventsManager != null)
                eventsManager.onChoiceActivate -= OnChoice;
        }

        private void ClearData()
        {
            supply = 0;
            demand = 0;
        }

		private void OnAddCard(ICardSlot slot, ICardView card)
		{
            if (slot == null || card == null) return;

            int demandChange = slot._DistrictInfo.demand + card.GetItemInfo().demand;
            int supplyChange = slot._DistrictInfo.supply + card.GetItemInfo().supply;

            ChangeDemand(demandChange);
            ChangeSupply(supplyChange);
        }

        private void OnRemoveCard(ICardSlot slot, ICardView view)
        {
			if(slot == null || view == null) return;

			int demandChange = slot._DistrictInfo.demand + view.GetItemInfo().demand;
			int supplyChange = slot._DistrictInfo.supply + view.GetItemInfo().supply;

            ChangeDemand(-demandChange);
            ChangeSupply(-supplyChange);
        }

        public void ChangeDemand(int value)
		{
			demand = Mathf.Clamp(demand + value, minDemand, maxDemand);

            OnChangeDemand?.Invoke(demand);
		}
		public void ChangeSupply(int value)
		{
            supply = Mathf.Clamp(supply + value, minSupply, maxSupply);

            OnChangeSupply?.Invoke(supply);
		}

		public int GetCurrentDemand() => demand;

        public int GetCurrentSupply() => supply;

        public int GetProfit() => demand > supply ? supply : demand;

        private void OnChoice(ChoiceData data)
        {
            List<ActionData> actions = data.actionsData;

            //change supply
            foreach (var action in actions)
            {
                if (action.type != StatType.supply)
                    continue;

                GD.Print("Invoked action: " + data.name + ": " + action.type.ToString() + " " + action.changeStatType.ToString() + " " + action.value.ToString());

                switch (action.changeStatType)
                {
                    case ChangeStatType.subtract:
                        ChangeSupply(-action.value);
                        break;

                    case ChangeStatType.equal:
                        ChangeSupply(action.value - supply);
                        break;

                    case ChangeStatType.add:
                        ChangeSupply(action.value);
                        break;
                    default:
                        break;
                }
            }

            //change demand
            foreach (var action in actions)
            {
                if (action.type != StatType.demand)
                    continue;

                GD.Print("Invoked action: " + data.name + ": " + action.type.ToString() + " " + action.changeStatType.ToString() + " " + action.value.ToString());

                switch (action.changeStatType)
                {
                    case ChangeStatType.subtract:
                        ChangeDemand(-action.value);
                        break;

                    case ChangeStatType.equal:
                        ChangeDemand(action.value - demand);
                        break;

                    case ChangeStatType.add:
                        ChangeDemand(action.value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
