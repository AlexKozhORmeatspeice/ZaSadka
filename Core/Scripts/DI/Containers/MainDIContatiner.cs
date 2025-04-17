using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cards;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace DI
{

    public partial class MainDIContatiner : DIContainer
    {
        [Export] public SlotMouseManager slotMouseManager;
        [Export] public CardSpawner cardSpawner;
        [Export] public MouseManager mouseManager;
        [Export] public CardMouseManager cardMouseManager;
        
        override protected void RegisterObjects()
        {
            builder.Register<ICardSpawner>(cardSpawner);
            builder.Register<IPointerManager>(mouseManager);
            builder.Register<ICardMouseManager>(cardMouseManager);
            builder.Register<ISlotMouseManager>(slotMouseManager);
            
            //не уверен как править эту хуйню, но регаю, чтобы потом можно было обратиться к реализации этой хуйни при raycast.
            //В ГОДОТЕ НЕТ ПОИСКА ПО ИНТЕРФЕЙСАМ ПРИКРЕПЛЕННЫМ К ОБЪЕКТАМ (хотя логично с учетом того, что они их GDScript даже не реализуют)
            builder.Register<ICardSlot>(new CardSlot());
            builder.Register<ICardView>(new CardView());
        }
    }
}
