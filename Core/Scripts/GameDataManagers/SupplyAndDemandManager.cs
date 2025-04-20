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
		int GetCurrentSupply();
		int GetPfofit();
    }
	
	public partial class SupplyAndDemandManager : ISupplyDemandManager, IStartable, IDisposable
	{
		[Inject] private IDistrictsManager districtsManager;

		private static int supply = 0;
		private static int demand = 0;

		private const int minSupply = -5;
        private const int maxSupply = 30;
        private const int minDemand = -5;
        private const int maxDemand = 30;

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

            int demandChange = slot.DistrictInfo.demand + card.GetItemInfo().demand;
            int supplyChange = slot.DistrictInfo.supply + card.GetItemInfo().supply;

            ChangeDemand(demandChange);
            ChangeSupply(supplyChange);
        }

        private void OnRemoveCard(ICardSlot slot, ICardView view)
        {
			if(slot == null || view == null) return;

			int demandChange = slot.DistrictInfo.demand + view.GetItemInfo().demand;
			int supplyChange = slot.DistrictInfo.supply + view.GetItemInfo().supply;

            ChangeDemand(-demandChange);
            ChangeSupply(-supplyChange);
        }

        public void ChangeDemand(int value)
		{
			if (value == 0)
			{
				return;
			}

			demand = Mathf.Clamp(demand + value, minDemand, maxDemand);

			GD.Print("Текущий demand " + demand);

            OnChangeDemand?.Invoke(demand);
		}
		public void ChangeSupply(int value)
		{
			if (value == 0)
			{
				return;
			}

            supply = Mathf.Clamp(supply + value, minSupply, maxSupply);

            GD.Print("Текущий supply " + supply);

            OnChangeSupply?.Invoke(supply);
		}

		public int GetCurrentDemand() => demand;
		public int GetCurrentSupply() => supply;
		public int GetPfofit() => demand > supply ? supply : demand;
    }
}
