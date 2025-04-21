using DI;
using Godot;
using Market;
using System;

namespace City_UI
{

    public interface ICityUI
    {

    }
    public partial class CityUI : Node2D, ICityUI, IStartable, IDispose
    {
        [Export] public BarView supplyBar;
        [Export] public BarView demandBar;
        [Export] public NextSceneButton nextSceneButton;

        private ISupplyObserver supplyObserver;
        private IDemandObserver demandObserver;
        private IEndDayButtonObserver endDayButtonObserver;

        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            resolver.Inject(supplyObserver = new SupplyObserver(supplyBar));
            resolver.Inject(demandObserver = new DemandObserver(demandBar));
            resolver.Inject(endDayButtonObserver = new EndDayButtonObserver(nextSceneButton));
        }

        public void Start()
        {
            supplyObserver.Enable();
            demandObserver.Enable();
            endDayButtonObserver.Enable();
        }

        public void Dispose()
        {
            supplyObserver.Disable();
            demandObserver.Disable();
            endDayButtonObserver.Disable();
        }
    }

}

