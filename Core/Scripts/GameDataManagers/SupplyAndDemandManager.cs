using Cards;
using DI;
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
        float GetNormDemand();

        int MaxSupply { get;  }
        int MaxDemand { get; }

        int GetProfit();
    }
	
	public partial class SupplyAndDemandManager : ISupplyDemandManager, IStartable, IDisposable
	{
		[Inject] private IDistrictsManager districtsManager;

		private static int supply = 0;
		private static int demand = 0;

		private const int minSupply = 0;
        private const int maxSupply = 30;
        private const int minDemand = 0;
        private const int maxDemand = 30;

        public int MaxSupply => maxSupply;

        public int MaxDemand => maxDemand;

        public event Action<int> OnChangeDemand;
		public event Action<int> OnChangeSupply;

        public void Start()
        {
			districtsManager.onAddCard += OnAddCard;
			districtsManager.onRemoveCard += OnRemoveCard;
        }

        public void Dispose()
        {
            districtsManager.onAddCard -= OnAddCard;
            districtsManager.onRemoveCard -= OnRemoveCard;
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

        public float GetNormDemand()
        {
            throw new NotImplementedException();
        }
    }
}
