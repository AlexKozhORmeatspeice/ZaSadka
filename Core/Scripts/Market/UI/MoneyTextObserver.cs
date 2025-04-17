using Godot;
using System;
using ZaSadka;

namespace Market
{
    public interface IMoneyTextObserver
    {
        void Enable();
        void Disable();
    }

    public partial class MoneyTextObserver : IMoneyTextObserver
    {
        [Inject] private IMoneyManager moneyManager;

        private const string descText = "Деньги: ";

        private IMoneyText view;
        public MoneyTextObserver(IMoneyText view)
        {
            this.view = view;
        }

        public void Enable()
        {
            view.MoneyText = GetLableText(moneyManager.StartMoney);

            moneyManager.onMoneyChange += OnChangeMoney;
        }

        public void Disable()
        {
            moneyManager.onMoneyChange -= OnChangeMoney;
        }

        private void OnChangeMoney(int nowMoney)
        {
            GD.Print(descText + nowMoney.ToString());

            view.MoneyText = GetLableText(nowMoney);
        }

        private string GetLableText(int money)
        {
            return descText + money.ToString();
        }
    }

}
