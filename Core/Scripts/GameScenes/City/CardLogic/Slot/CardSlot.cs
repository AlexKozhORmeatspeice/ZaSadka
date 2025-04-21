using Godot;
using System;
using System.Collections.Generic;
using ZaSadka;

namespace Cards
{
    public interface ICardSlot
    {
        Vector2 WorldPosition { get; set;}
        DistrictInfo _DistrictInfo { get; set;}

        DistrictName _DistrictName { get; }

        string SlotName { set; }
        string SupplyText { set; }
        string DemandText { set; }
        string InfluenceText { set; }
        string SusText { set; }

    }

    public partial class CardSlot : Node2D, ICardSlot
    {
        [Export] private DistrictName districtName;

        [Export] private Label name;

        [Export] private Label supplyText;
        [Export] private Label demandText;
        [Export] private Label influenceText;
        [Export] private Label suspicienceText;

        private DistrictInfo districtInfo;

        private ICardView cardOnSlot;

        public Vector2 WorldPosition 
        {
            get => Position;
            set
            {
                Position = value;
            }
        }

        public string SlotName { set => name.Text = value; }
        public string SupplyText { set => supplyText.Text = "Сп: " + value; }
        public string DemandText { set => demandText.Text = "Пр: " + value; }
        public string InfluenceText { set => influenceText.Text = "Вл: " + value; }
        public string SusText { set => suspicienceText.Text = "Пд: " + value; }

        public DistrictInfo _DistrictInfo 
        { 
            get => districtInfo;
            set
            {
                districtInfo = value;

                SlotName = districtInfo.name;

                SupplyText = districtInfo.supply.ToString();
                DemandText = districtInfo.demand.ToString();
                InfluenceText = districtInfo.influence.ToString();
                SusText = districtInfo.suspicion.ToString();
            }
        }

        public DistrictName _DistrictName => districtName;

        public override void _Ready()
        {
            //
        }

    }
}


