using DI;
using Godot;
using System;
using System.Diagnostics;
using System.Dynamic;

namespace Market
{
    public interface IMarketUI
    {

    }

    public partial class MarketUI : Node2D, IMarketUI, IStartable
    {
        [Inject] private INextSceneButton goToTownButton;
        [Inject] private ITextButton buyItemButton;
        [Inject] private IMoneyText moneyText;

        private IGoToTownButtonObserver goToTownButtonObserver;
        private IBuyItemButtonObserver buyItemButtonObserver;
        private IMoneyTextObserver moneyTextObserver;


        [Inject]
        public void Constuct(IObjectResolver resolver)
        {
            resolver.Inject(goToTownButtonObserver = new GoToTownButtonObserver(goToTownButton));
            resolver.Inject(buyItemButtonObserver = new  BuyItemButtonObserver(buyItemButton));
            resolver.Inject(moneyTextObserver = new MoneyTextObserver(moneyText));
        }

        public void Start()
        {
            goToTownButtonObserver.Enable();
            buyItemButtonObserver.Enable();
            moneyTextObserver.Enable();
        }

        public override void _ExitTree()
        {
            goToTownButtonObserver.Disable();
            buyItemButtonObserver.Disable();
            moneyTextObserver.Disable();
        }
    }
}

