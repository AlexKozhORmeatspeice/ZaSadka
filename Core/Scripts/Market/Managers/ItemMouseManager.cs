using Cards;
using DI;
using Godot;
using System;


namespace Market
{
    public interface IItemMouseManager
    {
        event Action<IItem> onPointerEnter;
        event Action<IItem> onPointerHover;
        event Action<IItem> onPointerLeave;
        event Action<IItem> onPointerDown;
        event Action<IItem> onPointerUp;
    }

    public partial class ItemMouseManager : Node2D, IItemMouseManager, IStartable
    {
        [Inject] private IPointerManager pointer;

        private IItem hoveredItem;

        public event Action<IItem> onPointerEnter;
        public event Action<IItem> onPointerHover;
        public event Action<IItem> onPointerLeave;
        public event Action<IItem> onPointerDown;
        public event Action<IItem> onPointerUp;

        public void Start()
        {
            pointer.onMove += CheckItems;
            pointer.onPointerDown += OnClickItem;
            pointer.onPointerUp += OnReleaseItem;
        }

        public override void _ExitTree()
        {
            pointer.onMove -= CheckItems;
            pointer.onPointerDown -= OnClickItem;
            pointer.onPointerUp -= OnReleaseItem;
        }

        private void CheckItems()
        {
            var newItem = pointer.RaycastByType(typeof(IItem)) as IItem;

            if (newItem == null)
            {
                if (hoveredItem != null)
                {
                    onPointerLeave?.Invoke(hoveredItem);
                    hoveredItem = null;
                }
                return;
            }

            if (newItem != hoveredItem)
            {
                if (hoveredItem != null)
                {
                    onPointerLeave?.Invoke(hoveredItem);
                }

                hoveredItem = newItem;
                onPointerEnter?.Invoke(hoveredItem);
            }

            onPointerHover?.Invoke(hoveredItem);
        }

        private void OnClickItem()
        {
            onPointerDown?.Invoke(hoveredItem);
        }

        private void OnReleaseItem()
        {
            onPointerUp?.Invoke(hoveredItem);
        }
    }
}
