using DI;
using Godot;
using System;
using System.Runtime.InteropServices;


namespace ZaSadka
{
    public interface IMoneyManager
    {
        event Action<int> onMoneyChange;
        bool ChangeMoney(int value);
        int StartMoney { get; }
    }

    public partial class MoneyManager : IMoneyManager, IStartable
    {
        private const int startMoney = 100;
        private int nowMoney;

        public int StartMoney => startMoney;

        public event Action<int> onMoneyChange;

        public bool ChangeMoney(int value)
        {
            if (nowMoney + value < 0)
                return false;

            nowMoney += value;

            onMoneyChange?.Invoke(nowMoney);
            
            return true;
        }

        public void Start()
        {
            nowMoney = startMoney;
        }
    }
}

