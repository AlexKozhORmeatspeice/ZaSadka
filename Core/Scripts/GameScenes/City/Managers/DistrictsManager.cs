using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards;
using DI;
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
        }

        public Dictionary<ICardSlot, ICardView> cardBySlot;
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
        [Inject] private ISlotMouseManager slotMouseManager;
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

        public void AddCard(ICardView card, ICardSlot slot)
        {
            if (card == null)
                return; 

            if (slot == null)
            {
                DeleteCard(card);
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
        }

        /// <summary>
        /// Логика того когда и как выставляются карты на районы
        /// </summary>
        public void Start()
        {
            if(cardMouseManager != null)
                cardMouseManager.onPointerDown += SetCard;

            if (slotMouseManager != null)
                slotMouseManager.onPointerUp += CheckInSlot;
        }

        public void Dispose()
        {
            if (cardMouseManager != null)
                cardMouseManager.onPointerDown -= SetCard;

            if (slotMouseManager != null)
                slotMouseManager.onPointerUp -= CheckInSlot;
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

        DistrictData IDistrictsManager.GetData(DistrictName name)
        {
            return dataByDistrict[name];
        }
    }
}
