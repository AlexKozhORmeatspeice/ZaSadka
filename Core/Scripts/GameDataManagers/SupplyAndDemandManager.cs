using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;

namespace ZaSadka
{
	public interface ISupplyDemandManager
    {
        void OnDemandChange(int bonus, ItemType reason);
		event Action<int> OnChangeDemand;
		void OnSupplyChange(int bonus, ItemType reason);
		event Action<int> OnChangeSupply;
		int GetCurrentDemand();
		int GetCurrentSupply();
		int GetPfofit();
		void FinalizeStats();
		void ClearLog();
		List<LogInfo> GetDailyLog();
    }
	
	public struct LogInfo
	{
		public LogInfo(StatsType stat, ItemType reason, int bonus) {
			Stat = stat;
			Reason = reason;
			Bonus = bonus;
		}

		public StatsType Stat = StatsType.Demand;
		public ItemType Reason = ItemType.Building;
		public int Bonus = 0;
	}
	public partial class SupplyAndDemandManager : ISupplyDemandManager
	{
			private static int supply = 0;
			private static int demand = 0;

			public event Action<int> OnChangeDemand;
			public event Action<int> OnChangeSupply;
			private List<LogInfo> log = [];

			public void OnDemandChange(int bonus, ItemType reason)
			{
				if (bonus == 0)
				{
					return;
				}
				log.Add(new(StatsType.Demand, reason, bonus));
				OnChangeDemand?.Invoke(demand);
			}
			public void OnSupplyChange(int bonus, ItemType reason)
			{
				if (bonus == 0)
				{
					return;
				}
				log.Add(new(StatsType.Supply, reason, bonus));
				OnChangeSupply?.Invoke(supply);
			}
			//мне кажется лучше это всё в конце дня суммировать и лог вывести
			public void FinalizeStats()
			{
				foreach (var info in log)
				{
					if (info.Stat == StatsType.Demand)
					{
						demand += info.Bonus;
					}
					else if (info.Stat == StatsType.Supply)
					{
						supply += info.Bonus;
					}
				}
			}

			public int GetCurrentDemand() => demand;
			public int GetCurrentSupply() => supply;
			public int GetPfofit() => demand > supply ? supply : demand;
			public void ClearLog()
			{
				log.Clear();
			}

			public List<LogInfo> GetDailyLog() => log;
	}

}
