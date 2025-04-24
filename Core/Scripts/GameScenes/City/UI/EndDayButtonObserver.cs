using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market;
using ZaSadka;

namespace City_UI
{
    public interface IEndDayButtonObserver
    {
        void Enable();
        void Disable();
        event Action OnDayEnded;
    }

    public class EndDayButtonObserver : IEndDayButtonObserver
    {
        private INextSceneButton view;
        public event Action OnDayEnded;
        [Inject] private IInfluenceManager influenceManager;
        [Inject] private ISuspicionManager suspicionManager;

        public EndDayButtonObserver(INextSceneButton view)
        {
            this.view = view;
        }

        public void Enable()
        {
            view.onClick += LoadScene;
        }

        public void Disable()
        {
            view.onClick -= LoadScene;
        }

        private void LoadScene()
        {
            OnDayEnded?.Invoke();

            string nextScene = "";
            if (suspicionManager.CheckLoseCondition())
            {
                nextScene = view.GetLoseScene();
            }
            else
            {
                nextScene = influenceManager.CheckWinningCondition() ? view.GetFinalScene() : view.GetNextScene();
            }

            view.GetTree().ChangeSceneToFile(nextScene);
        }
    }
}
