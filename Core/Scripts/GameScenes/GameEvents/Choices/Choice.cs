using Godot;
using System;


namespace Game_events
{
    public interface IChoice
    {
        void Activate();
        void SetData(ChoiceData data);
    }

    public class Choice : IChoice
    {
        private ChoiceData data;

        public void Activate()
        {

        }

        public void SetData(ChoiceData data)
        {
            this.data = data;
        }
    }
}
