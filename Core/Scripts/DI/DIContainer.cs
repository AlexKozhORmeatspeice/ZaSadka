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
    /// <summary>
    /// Класс используется для создания DI 
    /// 
    /// Указания к использованию:
    /// Нужно зарегать все объекты которые будут использоваться
    /// Если это класс из сцены - добавляем атрибут [Export] и регаем его
    /// 
    /// Внутри классов реализации можно (и нужно) использовать атрибут [Inject] для указания зависимостей.
    ///
    /// 1. В случае переменной просто загрузится зависимость в класс
    /// 
    /// 2. В случае функции подугрузятся зависимости в параметры функции и вызовится функция. Можно использовать 
    /// если есть какой-то спаунер например, который вызывает объекты с доп. зависимостями.
    /// 
    /// 3. Также реализовал IObjectResolver, который используется чтобы опракидывать зависимости в новые сущности например
    /// 
    /// PS:
    /// Аккуратнее, потому что могут образоваться циклы Inject-ов и у меня для такого случая пока нет проверки
    /// 
    /// TODO: сделать проверку на циклы
    /// TODO: сделать опцию автоматического добавления объекта из иерархии сцены
    /// TODO: удаление зависимостей после выхода со сцены
    /// TODO: попробовать все же подключить мап-ы для хранения, чтобы log(n) сделать и меньше памяти тратить
    /// TODO: сделать подключение классов
    /// </summary>

    public partial class DIContainer : Node, IDIContatiner
    {
        protected IDIBuilder builder;
        //надо для дебага, чёт поломалось
        protected virtual string name {get;}

        protected virtual void RegisterObjects() {}
        public override void _Ready()
        {
            builder = new DIBuilder() as IDIBuilder;
            builder.Register<IObjectResolver>(new ObjectResolver(this));

            RegisterObjects();
            builder.InjectDependencies();
            builder.EnableStarts();
            builder.EnableLateStarts();
        }

        public override void _ExitTree()
        {
            builder.EnableDisposables();
        }

        void IDIContatiner.InjectDependencie(object obj)
        {
            builder.InjectDependencie(obj);
        }

        object IDIContatiner.GetRealization(Type type)
        {
            return builder.GetRealization(type);
        }
    }
    public interface IStartable
    {
        void Start();
    }

    public interface ILateStartable
    {
        void LateStart();
    }

    public interface IDispose
    {
        void Dispose();
    }

    public interface IDIContatiner
    {
        void InjectDependencie(object obj);
        Object GetRealization(Type type);
    }

}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
internal class Inject : Attribute //использовать, чтобы добавлять зависимости в классы
{ }


