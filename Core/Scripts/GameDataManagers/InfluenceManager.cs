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
    public interface IInfluenceManager
    {
        event Action<DistrictName, int> onInfluenceChange;

        int GetValue(DistrictName districtName);
        bool CheckWinningCondition();
    }

    internal class InfluenceManager : IInfluenceManager, IStartable, IDispose
    {
        [Inject] private IEventsManager eventsManager;
        [Inject] private IDistrictsManager districtsManager;
        
        public event Action<DistrictName, int> onInfluenceChange;

        private const int minInfluence = 0;
        private const int maxInfluence = 10;

        private static Dictionary<DistrictName, int> influenceByDistrict = new Dictionary<DistrictName, int>();

        public void Start()
        {
            for (int i = 0; i < (int)DistrictName.DistrictNameNum; i++)
            {
                DistrictName name = (DistrictName)i;

                if (!influenceByDistrict.ContainsKey(name))
                    influenceByDistrict.Add(name, int.MinValue);
            }

            districtsManager.onAddCard += OnAddCard;
            districtsManager.onRemoveCard += OnRemoveCard;

            if (eventsManager != null)
                eventsManager.onChoiceActivate += OnChoice;

            if (MenuDI.instance != null)
            {
                ClearData();
            }
        }

        public void Dispose()
        {
            districtsManager.onAddCard -= OnAddCard;
            districtsManager.onRemoveCard -= OnRemoveCard;

            if (eventsManager != null)
                eventsManager.onChoiceActivate -= OnChoice;
        }

        private void ClearData()
        {
            for (int i = 0; i < (int)DistrictName.DistrictNameNum; i++)
            {
                DistrictName name = (DistrictName)i;

                if (!influenceByDistrict.ContainsKey(name))
                {
                    influenceByDistrict.Add(name, int.MinValue);
                }
                else
                {
                    influenceByDistrict[name] = int.MinValue;
                }
            }
        }

        private void OnAddCard(ICardSlot slot, ICardView card)
        {
            if (card == null || slot == null) return;

            if (influenceByDistrict[slot._DistrictName] == int.MinValue)
            {
                influenceByDistrict[slot._DistrictName] = slot._DistrictInfo.influence;
            }

            int changeValue = card.GetItemInfo().influence;

            Change(slot._DistrictName, changeValue);
        }

        private void OnRemoveCard(ICardSlot slot, ICardView card)
        {
            if (card == null || slot == null) return;

            int changeValue = card.GetItemInfo().influence;

            Change(slot._DistrictName, -changeValue);
        }

        private void Change(DistrictName name, int value)
        {
            influenceByDistrict[name] = Math.Clamp(influenceByDistrict[name] + value, minInfluence, maxInfluence);

            onInfluenceChange?.Invoke(name, influenceByDistrict[name]);
        }

        public int GetValue(DistrictName districtName)
        {
            return influenceByDistrict[districtName];
        }

        private void OnChoice(ChoiceData data)
        {
            List<ActionData> actions = data.actionsData;

            foreach (var action in actions)
            {
                if (action.type != StatType.influence)
                    continue;
                GD.Print("Invoked action: " + data.name + ": " + action.type.ToString() + " " + action.changeStatType.ToString() + " " + action.value.ToString());
                switch (action.changeStatType)
                {
                    case ChangeStatType.subtract:
                        Change(data.districtName, -action.value);
                        break;

                    case ChangeStatType.equal:
                        Change(data.districtName, action.value - influenceByDistrict[data.districtName]);
                        break;

                    case ChangeStatType.add:
                        Change(data.districtName, action.value);
                        break;
                    default:
                        break;
                }
            }

        }
        public bool CheckWinningCondition()
        {
            foreach (var districtInfl in influenceByDistrict.Values)
            {
                if (districtInfl < maxInfluence)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
