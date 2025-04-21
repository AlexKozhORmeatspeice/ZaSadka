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
    }

    public class EndDayButtonObserver : IEndDayButtonObserver
    {
        [Inject] private ISupplyDemandManager supplyDemandManager;
        [Inject] private IMoneyManager moneyManager; //TODO: перенести логику внутрь MoneyManager. (меня бесит, что оно здесь, но уже 7 утра и я бля хочу поспать чутка)

        private INextSceneButton view;

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
            moneyManager.ChangeMoney(supplyDemandManager.GetProfit());

            view.GetTree().ChangeSceneToFile(view.GetNextScene());
        }
    }
}
