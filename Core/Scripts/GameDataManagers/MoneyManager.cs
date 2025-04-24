using City_UI;
using DI;
using Game_events;
using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace ZaSadka
{
    public interface IMoneyManager
    {
        event Action<int> onMoneyChange;
        bool ChangeMoney(int value);
        int NowMoney { get; }
       
    }

    public partial class MoneyManager : IMoneyManager, IStartable, IDispose
    {
        [Inject] private IEventsManager eventsManager;
        [Inject] private ICityUI cityUI;
        [Inject] private ISupplyDemandManager supplyDemandManager;

        private const int startMoney = 30;
        private static int nowMoney = startMoney;
        
        public int NowMoney => nowMoney;

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
            if (eventsManager != null)
                eventsManager.onChoiceActivate += OnChoice;

            if (MenuDI.instance != null)
            {
                ClearData();
            }

            if (cityUI != null)
            {
                cityUI.OnDayEnded += GetDayPaycheck;
            }
        }

        public void Dispose()
        {
            if (eventsManager != null)
                eventsManager.onChoiceActivate -= OnChoice;

            if (cityUI != null)
            {
                cityUI.OnDayEnded -= GetDayPaycheck;
            }
        }

        private void ClearData()
        {
            nowMoney = startMoney;
        }

        private void GetDayPaycheck()
        {
            ChangeMoney(supplyDemandManager.GetProfit());
        }

        private void OnChoice(ChoiceData data)
        {
            List<ActionData> actions = data.actionsData;

            foreach (var action in actions)
            {
                if (action.type != StatType.money)
                    continue;

                GD.Print("Invoked action: " + data.name + ": " + action.type.ToString() + " " + action.changeStatType.ToString() + " " + action.value.ToString());

                switch (action.changeStatType)
                {
                    case ChangeStatType.subtract:
                        ChangeMoney(-action.value);
                        break;

                    case ChangeStatType.equal:
                        ChangeMoney(action.value - nowMoney);
                        break;

                    case ChangeStatType.add:
                        ChangeMoney(action.value);
                        break;
                    default:
                        break;
                }
            }

        }
    }
}

