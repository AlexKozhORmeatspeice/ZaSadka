using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using ZaSadka;

namespace City_UI
{

    public interface ISupplyObserver
    {
        void Enable();
        void Disable();
    }

    internal class SupplyObserver : ISupplyObserver
    {
        [Inject] private ISupplyDemandManager supplyDemandManager;

        private IBarView view;

        public SupplyObserver(IBarView view)
        {
            this.view = view;
        }

        public void Enable()
        {
            supplyDemandManager.OnChangeSupply += ChangeValue;
        }

        public void Disable()
        {
            supplyDemandManager.OnChangeSupply -= ChangeValue;
        }

        private void ChangeValue(int value)
        {
            GD.Print("Supply :" + value);

            view.SetValue((float)value / (float)supplyDemandManager.MaxSupply);
        }
    }
}
