using Godot;
using System;


namespace GameEvents
{
    public interface IChoice
    {
        void DoGoodChoise();
        void DoBadChoice();
    }

    public abstract class Choice : IChoice
    {
        public abstract void DoBadChoice();

        public abstract void DoGoodChoise();
    }
}
