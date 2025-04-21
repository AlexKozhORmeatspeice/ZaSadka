using Godot;
using System;

namespace Game_events
{
    

    public interface IGameEvent
    {
        Vector2 WorldPosition { get; }
        float DistToDetectPointer { get; }

        string Name { set; }

        void RotateByNormValue(float t01); //0 - left, 1 - right
        void SetStartPos();
        void SetZOrder(int zOrder);
        void Delete();
    }

    public partial class GameEvent : Node2D, IGameEvent
    {
        [Export] private CanvasItem canvasItem;
        [Export] private Label label;
        [Export] private float parameterThreshold;

        [Export] private float maxAngle;
        [Export] private float distToDetectPointer = 100.0f;

        //пока рандомно генерятся от типа
        private IChoice yesChoice;
        private IChoice noChoice;

        private ChoiceData data;

        public Vector2 WorldPosition => Position;

        public float DistToDetectPointer => distToDetectPointer;

        string IGameEvent.Name { set => label.Text = value; }

        public void SetZOrder(int zOrder)
        {
            canvasItem.ZIndex = zOrder;
        }

        public void RotateByNormValue(float t01)
        {
            t01 = Mathf.Clamp(t01, 0.0f, 1.0f);

            float maxRadiance = Mathf.DegToRad(maxAngle);
            Rotation = Mathf.Lerp(-maxRadiance, maxRadiance, t01);
        }

        public void SetStartPos()
        {
            RotateByNormValue(0.5f);
        }

        public void Delete()
        {
            QueueFree();
        }
    }
}
