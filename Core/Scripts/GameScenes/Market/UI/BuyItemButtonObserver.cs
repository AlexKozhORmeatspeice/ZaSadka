using Godot;
using System;

namespace Market
{

    public interface IBuyItemButtonObserver
    {
        void Enable();
        void Disable();
    }

    public partial class BuyItemButtonObserver : Node, IBuyItemButtonObserver
    {
        [Inject] private IMarketManager marketManager;
        private ITextButton view;
        public BuyItemButtonObserver(ITextButton view)
        {
            this.view = view;
        }

        public void Enable()
        {
            view.onClick += marketManager.BuyChoosedItem;
        }

        public void Disable()
        {
            view.onClick += marketManager.BuyChoosedItem;
        }
    }

}

