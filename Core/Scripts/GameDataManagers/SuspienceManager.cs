using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards;
using DI;
using Game_events;
using Godot;

namespace ZaSadka
{
    public interface ISuspicionManager
    {
        event Action<DistrictName, int> onSuspicionChange;
        int GetValue(DistrictName district);
    }

    internal class SuspicionManager : ISuspicionManager, IStartable, IDispose
    {
        [Inject] private IEventsManager eventsManager;
        [Inject] private IDistrictsManager districtsManager;
        public event Action<DistrictName, int> onSuspicionChange;

        private const int minSuspicion = 0;
        private const int maxSuspicion = 10;

        private static Dictionary<DistrictName, int> suspicionByDistrict = new Dictionary<DistrictName, int>();

        public void Start()
        {
            for(int i = 0; i < (int)DistrictName.DistrictNameNum; i++)
            {
                DistrictName name = (DistrictName)i;

                if (!suspicionByDistrict.ContainsKey(name))
                    suspicionByDistrict.Add(name, int.MinValue);
            }

            districtsManager.onAddCard += OnAddCard;
            districtsManager.onRemoveCard += OnRemoveCard;

            if (eventsManager != null)
                eventsManager.onChoiceActivate += OnChoice;
        }

        public void Dispose()
        {
            districtsManager.onAddCard -= OnAddCard;
            districtsManager.onRemoveCard -= OnRemoveCard;

            if (eventsManager != null)
                eventsManager.onChoiceActivate -= OnChoice;
        }

        private void OnAddCard(ICardSlot slot, ICardView card)
        {
            if(card == null || slot == null) return;

            if (suspicionByDistrict[slot._DistrictName] == int.MinValue)
            {
                suspicionByDistrict[slot._DistrictName] = slot._DistrictInfo.suspicion;
            }

            int changeValue = card.GetItemInfo().suspicion;

            Change(slot._DistrictName, changeValue);
        }

        private void OnRemoveCard(ICardSlot slot, ICardView card)
        {
            if (card == null || slot == null) return;

            int changeValue = card.GetItemInfo().suspicion;

            Change(slot._DistrictName, -changeValue);
        }

        private void Change(DistrictName name, int value)
        {
            suspicionByDistrict[name] = suspicionByDistrict[name] + value;

            onSuspicionChange?.Invoke(name, suspicionByDistrict[name]);
        }

        public int GetValue(DistrictName district)
        {
            return suspicionByDistrict[district];
        }

        private void OnChoice(ChoiceData data)
        {
            List<ActionData> actions = data.actionsData;

            foreach (var action in actions)
            {
                if (action.type != StatType.suspicion)
                    continue;

                GD.Print("Invoked action: " + data.name + ": " + action.type.ToString() + " " + action.changeStatType.ToString() + " " + action.value.ToString());

                switch (action.changeStatType)
                {
                    case ChangeStatType.subtract:
                        Change(data.districtName, -action.value);
                        break;

                    case ChangeStatType.equal:
                        Change(data.districtName, action.value - suspicionByDistrict[data.districtName]);
                        break;

                    case ChangeStatType.add:
                        Change(data.districtName, action.value);
                        break;
                    default:
                        break;
                }
            }

        }
        

    }
}
