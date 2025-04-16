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
    /// </summary>

    public partial class DIContatiner : Node2D, IDIContatiner
    {
        [Export] public CardSpawner cardSpawner;
        [Export] public MouseManager mouseManager;
        [Export] public CardMouseManager cardMouseManager;
        private void RegisterObjects()
        {
            Register<IObjectResolver>(new ObjectResolver(this));

            Register<ICardSpawner>(cardSpawner);
            Register<IPointerManager>(mouseManager);
            Register<ICardView>(new CardView());
            Register<ICardMouseManager>(cardMouseManager);
        }

        /// Не трогать все что после. Лень было пока что выносить логику в отдельный класс
        private List<Type> genericTypes = new List<Type>();
        private List<Object> realizations = new List<Object>();

        public override void _Ready()
        {
            RegisterObjects();
            InjectDependencies();
            EnableStarts();
        }

        private void InjectDependencies()
        {
            foreach (Object obj in realizations)
            {
                if (obj == null)
                    return;

                InjectDependencie(obj);
            }
        }

        public void InjectDependencie(object obj)
        {
            Type type = obj.GetType();

            //Регаем переменные
            var customAttributeFieldList = from t in type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                           where t.GetCustomAttributes(false).Any(a => a is Inject)
                                           select t;

            foreach (var field in customAttributeFieldList)
            {
                Object realiz = GetRealization(field.FieldType);

                field.SetValue(obj, realiz);
            }

            //Регаем методы
            var customAttributeMethodList = from t in type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                            where t.GetCustomAttributes(false).Any(a => a is Inject)
                                            select t;

            foreach (var field in customAttributeMethodList)
            {
                ParameterInfo[] parameters = field.GetParameters();
                object[] objParams = new object[parameters.Length];
                int ind = 0;
                foreach (var parameter in parameters)
                {
                    Type paramType = parameter.ParameterType;
                    objParams[ind] = GetRealization(paramType);
                }

                field.Invoke(obj, objParams);
            }
        }

        private void EnableStarts()
        {
            foreach (Object obj in realizations)
            {
                IStartable startable = obj as IStartable;
                if (startable != null)
                {
                    startable.Start();
                }
            }
        }

        private void Register<T>(T service)
        {
            var type = typeof(T);
            if (genericTypes.Exists(x => x == type))
            {
                GD.PrintErr($"Service of type {type} is already registered.");
                return;
            }

            genericTypes.Add(type);
            realizations.Add(service);

        }
        private Object GetRealization<T>()
        {
            var type = typeof(T);

            int index = genericTypes.FindIndex(x => x == type);
            if (index != -1)
            {
                return realizations[index];
            }

            GD.PrintErr($"Service of type {type} is not registered.");
            return default;
        }
        public Object GetRealization(Type type)
        {
            int index = genericTypes.FindIndex(x => x == type);
            if (index != -1)
            {
                return realizations[index];
            }
            GD.PrintErr($"Service of type {type} is not registered.");
            return default;
        }
    }
    public interface IStartable
    {
        void Start();
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

