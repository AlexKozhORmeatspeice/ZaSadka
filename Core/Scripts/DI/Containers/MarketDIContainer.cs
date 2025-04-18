using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cards;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using Market;
using ZaSadka;

namespace DI
{
    public partial class MarketDIContainer : DIContainer
    {
        [Export] private ItemsSpawner itemsSpawner;
        [Export] private MouseManager pointer;
        [Export] private ItemMouseManager itemMouseManager;
        [Export] private MarketManager marketManager;
        [Export] private MarketUI marketUI;
        [Export] private BuyItemButton buyItemButton;
        [Export] private GoToTownButton goToTownButton;
        [Export] private MoneyText moneyText;
        override protected void RegisterObjects()
        {
            builder.Register<IItemMouseManager>(itemMouseManager);
            builder.Register<IItemsSpawner>(itemsSpawner);
            builder.Register<IPointerManager>(pointer);
            builder.Register<IMarketManager>(marketManager);
            builder.Register<IInventoryManager>(new InventoryManager());

            builder.Register<IJsonCardManager>(new JsonCardManager());

            
            builder.Register<IMarketUI>(marketUI);
            
            builder.Register<IMoneyManager>(new MoneyManager());
            builder.Register<IMoneyText>(moneyText);

            builder.Register<IBuyItemButton>(buyItemButton);
            builder.Register<IGoToTownButton>(goToTownButton);

            builder.Register<IItem>(new Item());
        }
    }
}


