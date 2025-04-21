using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cards;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using ZaSadka;
using City_UI;

namespace DI
{

    public partial class MainDIContatiner : DIContainer
    {
        [Export] public SlotMouseManager slotMouseManager;
        [Export] public CardSpawner cardSpawner;
        [Export] public MouseManager mouseManager;
        [Export] public CardMouseManager cardMouseManager;
        [Export] public SlotsManager slotsManager;
        [Export] public CityUI cityUI;

        override protected void RegisterObjects()
        {
            builder.Register<IMoneyManager>(new MoneyManager());
            builder.Register<IInventoryManager>(new InventoryManager());
            builder.Register<ISupplyDemandManager>(new SupplyAndDemandManager());
            builder.Register<IJsonDistrictManager>(new JsonDistrictManager());
            builder.Register<IDistrictsManager>(new DistrictsManager());
            builder.Register<ISuspicionManager>(new SuspicionManager());
            builder.Register<IInfluenceManager>(new InfluenceManager());

            builder.Register<ICardSpawner>(cardSpawner);
            builder.Register<IPointerManager>(mouseManager);
            builder.Register<ICardMouseManager>(cardMouseManager);
            builder.Register<ISlotMouseManager>(slotMouseManager);
            builder.Register<ISlotsManager>(slotsManager);
            builder.Register<ICityUI>(cityUI);

            //не уверен как править эту хуйню, но регаю, чтобы потом можно было обратиться к реализации этой хуйни при raycast.
            //В ГОДОТЕ НЕТ ПОИСКА ПО ИНТЕРФЕЙСАМ ПРИКРЕПЛЕННЫМ К ОБЪЕКТАМ (хотя логично с учетом того, что их GDScript даже не реализует)
            builder.Register<ICardSlot>(new CardSlot());
            builder.Register<ICardView>(new CardView());
        }
    }
}
