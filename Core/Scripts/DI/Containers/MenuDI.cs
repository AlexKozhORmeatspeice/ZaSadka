using Godot;
using ZaSadka;
using DI;
using System;

namespace DI
{
    public partial class MenuDI : DIContainer
    {
        static public MenuDI instance;
        override protected void RegisterObjects()
        {
            instance = this;

            builder.Register<IDistrictsManager>(new DistrictsManager());
            builder.Register<ISuspicionManager>(new SuspicionManager());
            builder.Register<IInfluenceManager>(new InfluenceManager());
            builder.Register<ISupplyDemandManager>(new SupplyAndDemandManager());
            builder.Register<IMoneyManager>(new MoneyManager());
            builder.Register<IInventoryManager>(new InventoryManager());
        }

        public override void _ExitTree()
        {
            instance = null;
        }
    }
}


