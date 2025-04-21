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
        [Export] private TextButton buyItemButton;
        [Export] private NextSceneButton goToTownButton;
        [Export] private MoneyText moneyText;
        override protected string name {get => "Market DI";}
        override protected void RegisterObjects()
        {
            builder.Register<IJsonCardManager>(new JsonCardManager());
            builder.Register<IInventoryManager>(new InventoryManager());
            builder.Register<IMoneyManager>(new MoneyManager());

            builder.Register<IItemMouseManager>(itemMouseManager);
            builder.Register<IItemsSpawner>(itemsSpawner);
            builder.Register<IPointerManager>(pointer);
            builder.Register<IMarketManager>(marketManager);
            
            builder.Register<IMarketUI>(marketUI);
            
            builder.Register<IMoneyText>(moneyText);

            builder.Register<ITextButton>(buyItemButton);
            builder.Register<INextSceneButton>(goToTownButton);

            builder.Register<IItem>(new Item());
        }
    }
}


