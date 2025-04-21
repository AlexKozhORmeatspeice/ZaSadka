using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using ZaSadka;

namespace City_UI
{

    public interface IDemandObserver
    {
        void Enable();
        void Disable();
    }

    internal class DemandObserver : IDemandObserver
    {
        [Inject] private ISupplyDemandManager supplyDemandManager;

        private IBarView view;

        public DemandObserver(IBarView view)
        {
            this.view = view;
        }

        public void Enable()
        {
            ChangeValue(supplyDemandManager.NowDemand);
            supplyDemandManager.OnChangeDemand += ChangeValue;
        }

        public void Disable()
        {
            supplyDemandManager.OnChangeDemand -= ChangeValue;
        }

        private void ChangeValue(int value)
        {
            view.SetValue((float)value / (float)supplyDemandManager.MaxDemand);
        }
    }
}
