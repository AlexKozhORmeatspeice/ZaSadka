using Godot;
using Godot.NativeInterop;
using System;
using Market;
using System.Linq;
using DI;

namespace ZaSadka
{
	public enum StatsType
	{
		Supply,
		Demand,
		Influence,
		Suspicion
	}
	public struct ItemInfo
	{
		public ItemInfo() {}
		public string name = "";
		public int supply = 0;
		public int demand = 0;
		public int influence = 0;
		public int suspicion = 0;
	}

	public struct UnitAdditionalInfo
	{
		public UnitAdditionalInfo() {}
		public float EventChance = 0;
		public string DebuffName = "";
		public int DebuffDistrictId = -1;
		public StatsType DebuffStat = StatsType.Demand;
		public int DebuffAmount = 0;
	}
	public interface IJsonCardManager
	{
		ItemInfo GetItemInfo(ItemType type, int id);
		UnitAdditionalInfo GetUnitAdditionalInfo(int id);
		int GetCardsAmount(ItemType type);
	}

	public partial class JsonCardManager : IJsonCardManager, IStartable
	{
		private Godot.Collections.Dictionary cards;
        // Called when the node enters the scene tree for the first time.

        void IStartable.Start()
        {
            using var file = FileAccess.Open("./Core/Data/data.json", FileAccess.ModeFlags.Read);
			string json_as_text = file.GetAsText();
			Json jsonLoader = new Json();
			Error error = jsonLoader.Parse(json_as_text);
			if (error != Error.Ok)
			{
				GD.Print(error);
				return;
			}
				
			Godot.Collections.Dictionary loadedData = (Godot.Collections.Dictionary)jsonLoader.Data;

			cards = loadedData["cards"].AsGodotDictionary();

			file.Close();
        }

		public ItemInfo GetItemInfo(ItemType type, int id)
		{
			ItemInfo info = new();
			string itemType = type == ItemType.Building ? "buildings" : "units";
			var neededCard = cards[itemType].AsGodotArray()[id].AsGodotDictionary();
			if (neededCard.ContainsKey("name"))
			{
				info.name = neededCard["name"].AsString();
			}
			if (neededCard.ContainsKey("supply"))
			{
				info.supply = neededCard["supply"].AsInt16();
			}
			if (neededCard.ContainsKey("demand"))
			{
				info.demand = neededCard["demand"].AsInt16();
			}
			if (neededCard.ContainsKey("influence"))
			{
				info.influence = neededCard["influence"].AsInt16();
			}
			if (neededCard.ContainsKey("suspicion"))
			{
				info.suspicion = neededCard["suspicion"].AsInt16();
			}

			return info;
		}
		public UnitAdditionalInfo GetUnitAdditionalInfo(int id)
		{
			var info = new UnitAdditionalInfo();
			var neededCard = cards["units"].AsGodotArray()[id].AsGodotDictionary();
			if (neededCard.ContainsKey("event_chance"))
			{
				info.EventChance = (float)neededCard["event_chance"].AsDouble();
			}
			if (neededCard.ContainsKey("debuff_name"))
			{
				info.DebuffName = neededCard["debuff_name"].AsString();
			}
			if (neededCard.ContainsKey("debuff_district_id"))
			{
				info.DebuffDistrictId = neededCard["debuff_district_id"].AsInt16();
			}
			if (neededCard.ContainsKey("debuff_item"))
			{
				string debuffStat = neededCard["debuff_item"].AsString();
                info.DebuffStat = debuffStat switch
                {
                    "supply" => StatsType.Supply,
                    "demand" => StatsType.Demand,
                    "suspicion" => StatsType.Suspicion,
                    _ => StatsType.Influence,
                };
            }
			if (neededCard.ContainsKey("debuff_amount"))
			{
				info.DebuffAmount = neededCard["debuff_amount"].AsInt16();
			}

			return info;
		}
        int IJsonCardManager.GetCardsAmount(ItemType type)
        {
            GD.Print("we are here");
			string cardType = type == ItemType.Building ? "buildings" : "units";
			return cards[cardType].AsGodotArray().ToArray().Length;
        }
	}
}

