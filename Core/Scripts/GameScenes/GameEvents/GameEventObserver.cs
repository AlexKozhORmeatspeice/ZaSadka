using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game_events
{
    public interface IGameEventObserver
    {
        void Enable();
        void Disable();
    }

    internal class GameEventObserver : IGameEventObserver
    {
        [Inject] private IEventsSpawner eventsSpawner;
        [Inject] private IEventsManager eventsManager;
        [Inject] private IPointerManager pointer;
        
        private bool isDragging;
        private bool canDrag;

        private int ID;
        private static int g_MaxID = 0;
        private static int g_NowChoosedID = 0;

        private EventInfo eventInfo;

        private IGameEvent view;
        public GameEventObserver(IGameEvent view)
        {
            this.view = view;

            ID = g_MaxID;
            g_NowChoosedID = ID;
            g_MaxID++;

            view.SetZOrder(ID);
        }

        public void Enable()
        {
            canDrag = false;
            isDragging = false;

            pointer.onMove += RotateObj;
            pointer.onPointerDown += EnableDragging;
            pointer.onPointerUp += DisableDragging;

            eventsSpawner.onUpdateInfo += UpdateInfo;

            eventsManager.onDeleteInfo += Delete;
        }

        public void Disable()
        {
            pointer.onMove -= RotateObj;
            pointer.onPointerDown -= EnableDragging;
            pointer.onPointerUp -= DisableDragging;

            eventsSpawner.onUpdateInfo -= UpdateInfo;

            eventsManager.onDeleteInfo -= Delete;
        }

        private void EnableDragging()
        {
            if (!CheckCanDrag())
                return;

            isDragging = true;
        }

        private void DisableDragging()
        {
            if (!CheckCanDrag())
                return;

            isDragging = false;
            view.SetStartPos();
        }

        private void RotateObj()
        {
            if (!isDragging)
                return;

            float pointerPosX = pointer.NowWorldPosition.X;
            float viewPosX = view.WorldPosition.X;
            
            float dist = pointerPosX - viewPosX;

            //линейная нормализация
            float alpha = (dist + view.DistToDetectPointer) / (2.0f * view.DistToDetectPointer);

            view.RotateByNormValue(alpha);
        }

        private bool CheckCanDrag()
        {
            canDrag = ID == g_NowChoosedID;
            
            return canDrag;
        }

        private void ChangeChoosedID()
        {
            g_NowChoosedID--;
        }

        private void UpdateInfo(IGameEvent gameEvent, EventInfo info)
        {
            if (gameEvent != view)
                return;

            eventInfo = info;
            view.Name = info.name;
        }

        private void Delete(EventInfo info)
        {
            if (eventInfo.name != info.name)
                return;

            ChangeChoosedID();
            Disable();
            view.Delete();
        }
    }
}
