using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_events
{
    public interface INoTextObserver
    {
        void Enable();
        void Disable();
    }


    internal class NoTextObserver : INoTextObserver
    {
        [Inject] private IEventsManager eventsManager;

        private IGameText view;

        public NoTextObserver(IGameText view)
        {
            this.view = view;
        }

        public void Enable()
        {
            eventsManager.onSetNowEvent += SetInfo;
        }

        public void Disable()
        {
            eventsManager.onSetNowEvent -= SetInfo;
        }

        private void SetInfo(EventInfo info)
        {
            view.Description = info.no_choice.name;
        }
    }
}
