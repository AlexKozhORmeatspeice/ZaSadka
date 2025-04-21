using Godot;
using System;


namespace City_UI
{
    public interface IBarView
    {
        void SetValue(float value);
    }

    public partial class BarView : ProgressBar, IBarView
    {
        public void SetValue(float value)
        {
            Mathf.Clamp(value, 0.0f, 1.0f);

            this.Value = value;
        }
    }
}
