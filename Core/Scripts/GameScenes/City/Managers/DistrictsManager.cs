using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards;
using DI;
using Game_events;
using Godot;

namespace ZaSadka
{
    public enum DistrictName
    {
        OldDocks,
        HistoricalCenter,
        Outskirts,
        DistrictNameNum
    }

    public struct DistrictData
    {
        public DistrictData()
        {
            cardBySlot = new Dictionary<ICardSlot, ICardView>();
            itemInfoByID = new Dictionary<int, ItemInfo>();
        }

        public Dictionary<ICardSlot, ICardView> cardBySlot;
        public Dictionary<int, ItemInfo> itemInfoByID;
    }

    public interface IDistrictsManager
    {
        event Action<ICardSlot, ICardView> onAddCard;
        event Action<ICardSlot, ICardView> onRemoveCard;
        void AddCard(ICardView card, ICardSlot slot);
        void DeleteCard(ICardView card);
        void PermanentDelete(ICardView card);
        DistrictData GetData(DistrictName name);
    }

    //TODO: добавить проверку условий на то можно ли добавлять карту на слот и можно ли вообще ее двигать
    internal class DistrictsManager : IDistrictsManager, IStartable, IDispose
    {
        [Inject] private IEventsManager eventsManager;

        [Inject] private ISlotMouseManager slotMouseManager;
        [Inject] private ISlotsManager slotsManager;
        [Inject] private ICardSpawner cardSpawner;
        [Inject] private ICardMouseManager cardMouseManager;

        private ICardView choosedCard;

        public event Action<ICardSlot, ICardView> onAddCard;
        public event Action<ICardSlot, ICardView> onRemoveCard;

        private static Dictionary<DistrictName, DistrictData> dataByDistrict = new Dictionary<DistrictName, DistrictData>()
        {
            {DistrictName.OldDocks, new DistrictData()},
            {DistrictName.HistoricalCenter, new DistrictData()},
            {DistrictName.Outskirts, new DistrictData()}
        };
        private bool IsSlotDisabled(ICardSlot slot)
        {
            var cardBySlot = dataByDistrict[slot._DistrictName].cardBySlot;
            return cardBySlot.ContainsKey(slot) && !cardBySlot[slot].isEnabled;
        }

        public void AddCard(ICardView card, ICardSlot slot)
        {
            if (card == null)
                return; 


            if (slot == null || IsSlotDisabled(slot))
            {
                DeleteCard(card);
                UpdateItemInfoBySlot();
                return;
            }

            DistrictData data = dataByDistrict[slot._DistrictName];
            
            ICardView newCard = card;
            ICardView oldCard = null;

            ICardSlot oldSlot = GetCardSlot(newCard); //слот где до этого была новая карта
            ICardSlot newSlot = slot; //слот куда хочет отправиться новая карта

            if (data.cardBySlot.ContainsKey(slot) && data.cardBySlot[slot] != null)
            {
                oldCard = data.cardBySlot[slot];
            }

            //Смена позиций если до этого в слоте была карта
            if(oldCard != null)
            {
                //проверяем были ли до этого новая карта в инвентаре
                if (oldSlot == null)
                {
                    DeleteCard(oldCard);
                }
                else
                {
                    data.cardBySlot[oldSlot] = oldCard;
                    onAddCard?.Invoke(oldSlot, oldCard);
                }
            }  
            else if(oldSlot != null)
            {
                DeleteCard(newCard);
            }
            
            data.cardBySlot[newSlot] = newCard;

            onAddCard?.Invoke(newSlot, card);
            UpdateItemInfoBySlot();
        }

        private void AddCardBySlotID(ICardView card, int id)
        {
            ICardSlot slot = slotsManager.GetSlotByID(id);

            onRemoveCard?.Invoke(slot, card);
            AddCard(card, slot);
        }

        public void DeleteCard(ICardView card)
        {
            //удаляем карту из слота

            bool isGot = false;
            foreach(var data in dataByDistrict.Values)
            {
                foreach (var slot in data.cardBySlot.Keys)
                {
                    if (data.cardBySlot[slot] == card)
                    {
                        AddCard(null, slot);

                        data.cardBySlot.Remove(slot);

                        onRemoveCard?.Invoke(slot, card);

                        isGot = true;
                        break;
                    }
                }

                if (isGot)
                    break;
            }

            UpdateItemInfoBySlot();
        }

        /// <summary>
        /// Логика того когда и как выставляются карты на районы
        /// </summary>
        public void Start()
        {
            dataByDistrict[DistrictName.Outskirts].cardBySlot.Clear();
            dataByDistrict[DistrictName.OldDocks].cardBySlot.Clear();
            dataByDistrict[DistrictName.HistoricalCenter].cardBySlot.Clear();

            if (cardSpawner != null)
                cardSpawner.onUpdateCardInSlotID += AddCardBySlotID;

            if (cardMouseManager != null)
                cardMouseManager.onPointerDown += SetCard;

            if (slotMouseManager != null)
                slotMouseManager.onPointerUp += CheckInSlot;

            if (eventsManager != null)
                eventsManager.onChoiceActivate += OnChoice;
        }

        public void Dispose()
        {
            if (cardSpawner != null)
                cardSpawner.onUpdateCardInSlotID -= AddCardBySlotID;
         
            if (cardMouseManager != null)
                cardMouseManager.onPointerDown -= SetCard;

            if (slotMouseManager != null)
                slotMouseManager.onPointerUp -= CheckInSlot;

            if (eventsManager != null)
                eventsManager.onChoiceActivate -= OnChoice;
        }

        private void CheckInSlot(ICardSlot slot)
        {
            if (choosedCard == null)
                return;

            AddCard(choosedCard, slot);
            choosedCard = null;
        }

        private void SetCard(ICardView card)
        {
            DeleteCard(card);
            choosedCard = card;
        }

        public ICardSlot GetCardSlot(ICardView card)
        {
            ICardSlot cardSlot = null;

            bool isGot = false;
            foreach (var data in dataByDistrict.Values)
            {
                foreach (var slot in data.cardBySlot.Keys)
                {
                    if (data.cardBySlot[slot] == card)
                    {
                        cardSlot = slot;
                        isGot = true;
                        break;
                    }
                }

                if (isGot)
                    break;
            }

            return cardSlot;
        }

        public void PermanentDelete(ICardView card)
        {
            bool isGot = false;
            foreach (var data in dataByDistrict.Values)
            {
                foreach (var slot in data.cardBySlot.Keys)
                {
                    if (data.cardBySlot[slot] == card)
                    {
                        AddCard(null, slot);

                        data.cardBySlot.Remove(slot);

                        card.Delete();

                        isGot = true;
                        break;
                    }
                }

                if (isGot)
                    break;
            }
        }

        private void UpdateItemInfoBySlot()
        {
            foreach(var data in dataByDistrict.Values)
            {
                foreach(var  slot in data.cardBySlot.Keys)
                {
                    ICardView card = data.cardBySlot[slot];
                    if(card != null)
                    {
                        data.itemInfoByID[slot.GetID()] = card.GetItemInfo();
                    }
                    else
                    {
                        ItemInfo info = new ItemInfo();
                        info.uniqueID = -100;

                        data.itemInfoByID[slot.GetID()] = info;
                    }

                }
            }
        }

        DistrictData IDistrictsManager.GetData(DistrictName name)
        {
            return dataByDistrict[name];
        }

        private void RemoveCardFromSlotByID(DistrictName districtName, int slotID)
        {
            var data = dataByDistrict[districtName];
            //clearing card slot
            foreach (var slot in data.cardBySlot.Keys)
            {
                if (slot.GetID() == slotID)
                {
                    var card = data.cardBySlot[slot];
                    AddCard(null, slot);
                    data.cardBySlot.Remove(slot);
                    card.Delete();
                    break;
                }
            }
        }

        private void OnChoice(ChoiceData data)
        {
            List<ActionData> actions = data.actionsData;
            DistrictData districtData = dataByDistrict[data.districtName];
            Dictionary<int, ItemInfo> items = districtData.itemInfoByID;

            foreach (var action in actions)
            {
                int count = 0;
                foreach (int id in items.Keys)
                {
                    if (count >= action.value)
                        break;

                    ItemInfo item = items[id];
                    
                    if (action.type != StatType.card && action.type.ToString().ToLower() != item.type.ToString().ToLower())
                        continue;

                    GD.Print("Invoked action: " + data.name + ": " + action.type.ToString() + " " + action.changeStatType.ToString() + " " + action.value.ToString());

                    switch (action.changeStatType)
                    {
                        case ChangeStatType.randDestroy:
                            dataByDistrict[data.districtName].itemInfoByID.Remove(id);
                            RemoveCardFromSlotByID(data.districtName, id);
                            count++;
                            break;
                        default:
                            break;
                    }

                }
                
            }

        }
    }
}
