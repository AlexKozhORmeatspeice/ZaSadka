using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace DI
{
    public interface IDIBuilder
    {
        void Register<T>(T service);
        void InjectDependencies();
        void InjectDependencie(object obj);
        void EnableStarts();
        void EnableLateStarts();
        Object GetRealization(Type type);
    }

    public class DIBuilder : IDIBuilder
    {
        private List<Type> genericTypes;
        private List<Object> realizations;

        public DIBuilder()
        {
            genericTypes = new List<Type>();
            realizations = new List<Object>();
        }


        public void InjectDependencies()
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

        public void EnableStarts()
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

        public void Register<T>(T service)
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
        public Object GetRealization<T>()
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

        public void EnableLateStarts()
        {
            foreach (Object obj in realizations)
            {
                ILateStartable startable = obj as ILateStartable;
                if (startable != null)
                {
                    startable.LateStart();
                }
            }
        }
    }
}
