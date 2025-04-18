using Godot;
using Godot.NativeInterop;
using System;
using Market;
using System.Linq;

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
		public string name {get; set;}
		public int supply {get; set;}
		public int demand {get; set;}
		public int influence {get; set;}
		public int suspicion {get; set;}
	}

	public struct UnitAdditionalInfo
	{
		public float eventChance {get; set;}
		public string debuffName {get; set;}
		public int debuffDistrictId {get; set;}
		public StatsType debuffStat {get; set;}
		public int debuffAmount {get; set;}
	}
	public interface IJsonCardManager
	{
		ItemInfo GetItemInfo(ItemType type, int id);
		UnitAdditionalInfo GetUnitAdditionalInfo(int id);
		int GetCardsAmount(ItemType type);
	}

	public partial class JsonCardManager : Node2D, IJsonCardManager
	{
		private Godot.Collections.Dictionary cards;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
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
			ItemInfo info = new ItemInfo();
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
				info.eventChance = (float)neededCard["event_chance"].AsDouble();
			}
			if (neededCard.ContainsKey("debuff_name"))
			{
				info.debuffName = neededCard["debuff_name"].AsString();
			}
			if (neededCard.ContainsKey("debuff_district_id"))
			{
				info.debuffDistrictId = neededCard["debuff_district_id"].AsInt16();
			}
			if (neededCard.ContainsKey("debuff_item"))
			{
				string debuffStat = neededCard["debuff_item"].AsString();
				switch (debuffStat)
				{
					case "supply":
					info.debuffStat = StatsType.Supply;
					break;
					case "demand":
					info.debuffStat = StatsType.Demand;
					break;
					case "suspicion":
					info.debuffStat = StatsType.Suspicion;
					break;
					default:
					info.debuffStat = StatsType.Influence;
					break;
				}
			}
			if (neededCard.ContainsKey("debuff_amount"))
			{
				info.debuffAmount = neededCard["debuff_amount"].AsInt16();
			}

			return info;
		}
		public int GetCardsAmount(ItemType type)
		{
			string cardType = type == ItemType.Building ? "buildings" : "units";
			return cards[cardType].AsGodotArray().ToArray().Length;
		}
	}
}

