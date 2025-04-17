using DI;
using Godot;
using System;

namespace Cards
{
    public interface ISlotMouseManager
    {
        event Action<ICardSlot> onPointerEnter;
        event Action<ICardSlot> onPointerHover;
        event Action<ICardSlot> onPointerLeave;
        event Action<ICardSlot> onPointerDown;
        event Action<ICardSlot> onPointerUp;
    }

    public partial class SlotMouseManager : Node2D, ISlotMouseManager, IStartable
    {
        [Inject] private IPointerManager pointer;

        private ICardSlot hoveredSlot;

        public event Action<ICardSlot> onPointerEnter;
        public event Action<ICardSlot> onPointerHover;
        public event Action<ICardSlot> onPointerLeave;
        public event Action<ICardSlot> onPointerDown;
        public event Action<ICardSlot> onPointerUp;

        public void Start()
        {
            pointer.onMove += CheckSlots;
            pointer.onPointerDown += OnClickSlot;
            pointer.onPointerUp += OnReleaseSlot;
        }

        public override void _ExitTree()
        {
            pointer.onMove -= CheckSlots;
            pointer.onPointerDown -= OnClickSlot;
            pointer.onPointerUp -= OnReleaseSlot;
        }

        private void CheckSlots()
        {
            var newSlot = pointer.RaycastByType(typeof(ICardSlot)) as ICardSlot;

            if (newSlot == null)
            {
                if (hoveredSlot != null)
                {
                    onPointerLeave?.Invoke(hoveredSlot);
                    hoveredSlot = null;
                }
                return;
            }

            if (newSlot != hoveredSlot)
            {
                if (hoveredSlot != null)
                {
                    onPointerLeave?.Invoke(hoveredSlot);
                }

                hoveredSlot = newSlot;
                onPointerEnter?.Invoke(hoveredSlot);
            }

            onPointerHover?.Invoke(hoveredSlot);

            
        }

        private void OnClickSlot()
        {
            onPointerDown?.Invoke(hoveredSlot);
        }

        private void OnReleaseSlot()
        {
            onPointerUp?.Invoke(hoveredSlot);
        }
    }
}
