using Godot;
using System;

namespace GameEvents
{
    public enum ParamType
    {
        None,
        Influence,
        Suspicion
    }

    enum ChoiceType
    {

    }

    public interface IGameEvent
    {
        Vector2 WorldPosition { get; }
        float DistToDetectPointer { get; }

        bool IsCanInvoke();
        void RotateByNormValue(float t01); //0 - left, 1 - right
        void SetStartPos();
    }

    public partial class GameEvent : Node2D, IGameEvent
    {
        [Export] private string name;
        [Export] private ParamType paramType;
        [Export] private float parameterThreshold;

        [Export] private float maxAngle;
        [Export] private float distToDetectPointer = 100.0f;

        //пока рандомно генерятся от типа
        private IChoice yesChoice;
        private IChoice noChoice;

        public Vector2 WorldPosition => Position;

        public float DistToDetectPointer => distToDetectPointer;

        public bool IsCanInvoke() //TODO: данные должны подгружаться из json файлов
        {
            return true;
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
    }
}
