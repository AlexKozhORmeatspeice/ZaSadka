using Godot;
using System;

namespace DI
{
    public interface IObjectResolver
    {
        void Inject(object obj);
        Type GetRealizationType(object obj);
    }

    public partial class ObjectResolver : IObjectResolver
    {
        private IDIContatiner DIContatiner;

        public ObjectResolver(IDIContatiner _diContatiner)
        {
            DIContatiner = _diContatiner;
        }

        public void Inject(object obj) 
        {
            DIContatiner.InjectDependencie(obj);
        }

        public Type GetRealizationType(object obj)
        {
            return DIContatiner.GetRealization(obj.GetType()).GetType();
        }
    }

}
