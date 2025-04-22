using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DI;
using Godot;

namespace Game_events
{
    public interface IEventsManager
    {
        event Action<ChoiceData> onChoiceActivate;
        event Action<EventInfo> onSetNowEvent;
        event Action<EventInfo> onDeleteInfo;

        event Action onEndEvents;
    }

    internal class EventsManager : IEventsManager, IStartable
    {
        [Inject] private IEventsSpawner eventsSpawner;
        [Inject] private IPointerManager pointer;

        public event Action<ChoiceData> onChoiceActivate;
        public event Action<EventInfo> onSetNowEvent;
        public event Action<EventInfo> onDeleteInfo;
        public event Action onEndEvents;

        private List<EventInfo> events;
        private EventInfo nowEvent;

        private bool isChoosing;

        private const float mouseDeltaToDetect = 600.0f;

        public void Start()
        {
            events = new List<EventInfo>();
            
            isChoosing = false;

            eventsSpawner.onUpdateInfo += AddInfo;
            eventsSpawner.noEvents += EndEvents;

            pointer.onPointerDown += SetIsChoosing;
            pointer.onPointerUp += SetNotChoosing;
            pointer.onMove += CheckChoose;
        }

        private void EndEvents()
        {
            onEndEvents?.Invoke();
        }

        private void ActivateNoChoice()
        {
            CallChoice(nowEvent.no_choice);
        }

        private void ActivateYesChoice()
        {
            CallChoice(nowEvent.yes_choice);
        }

        private void CallChoice(ChoiceData data)
        {
            events.Remove(nowEvent);
            onDeleteInfo?.Invoke(nowEvent);
           
            //do choice by data
            float chance = data.chance;
            float randVal01 = GD.Randf();

            if (randVal01 <= chance) //шанс сработал
            {
                onChoiceActivate?.Invoke(data);
            }
            else
            {
                data.actionsData.ForEach(actionData => actionData.type = StatType.none);

                onChoiceActivate?.Invoke(data);
            }

            //change now event
            if (events.Count > 0)
            {
                nowEvent = events[events.Count - 1];
                onSetNowEvent?.Invoke(nowEvent);
            }
            else
            {
                onEndEvents?.Invoke();
            }
        }

        private void AddInfo(IGameEvent gameEvent, EventInfo info)
        {
            events.Add(info);

            nowEvent = info;
            onSetNowEvent?.Invoke(nowEvent);
        }

        private void CheckChoose()
        {
            if (!isChoosing)
                return;

            float pointerPosX = pointer.NowWorldPosition.X;

            if(pointerPosX < -mouseDeltaToDetect)
            {
                ActivateYesChoice();
                isChoosing = false;
            }
            else if(pointerPosX > mouseDeltaToDetect)
            {
                ActivateNoChoice();
                isChoosing = false;
            }
        }

        private void SetIsChoosing()
        {
            isChoosing = true;
        }

        private void SetNotChoosing()
        {
            isChoosing = false;
        }
    }
}
