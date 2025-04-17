using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace GameEvents
{
    public interface IGameEventObserver
    {
        void Enable();
        void Disable();
    }

    internal class GameEventObserver : IGameEventObserver
    {
        [Inject] private IPointerManager pointer;
        
        private bool isDragging;
        private bool canDrag;

        private int ID;
        private static int g_MaxID = 0;
        private static int g_NowChoosedID = 0;

        private IGameEvent view;
        public GameEventObserver(IGameEvent view)
        {
            this.view = view;

            ID = g_MaxID;
            g_NowChoosedID = ID;
            g_MaxID++;
        }

        public void Enable()
        {
            canDrag = false;
            isDragging = false;

            pointer.onMove += RotateObj;
            pointer.onPointerDown += EnableDragging;
            pointer.onPointerUp += DisableDragging;
        }

        public void Disable()
        {
            pointer.onMove -= RotateObj;
            pointer.onPointerDown -= EnableDragging;
            pointer.onPointerUp -= DisableDragging;
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

            GD.Print(canDrag);     
            
            return canDrag;
        }

        private void ChangeChoosedID()
        {
            g_NowChoosedID--;
        }
    }
}
