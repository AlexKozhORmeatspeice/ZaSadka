using DI;
using Godot;
using System;

namespace ZaSadka
{
	public interface ISupplyDemandManager
    {
        event Action<int> onSupplyChange;
        bool ChangeSupply(int value);
        int StartSupply { get; }
		event Action<int> onDemandChange;
		bool ChangeDemand(int value);
		int StartDemand {get;}
    }
	public partial class SupplyAndDemandManager : ISupplyDemandManager, IStartable
	{
		private const int StartSupply = 0;
		private const int StartDemand = 0;
		private int nowSupply;
		private int nowDemand;
		bool ChangeSupply(int value)
		{
			nowSupply += value;
			onSupplyChange?.Invoke(nowSupply);
			return true;
		}

		
	}

}
