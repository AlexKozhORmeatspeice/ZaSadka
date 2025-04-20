using DI;
using Godot;
using Market;
using System;
using static Godot.OpenXRHand;

namespace ZaSadka
{
    public struct DistrictInfo
    {
        public DistrictInfo() { }

        public int id = 0;
        public string name = "";
        public int supply = 0;
        public int demand = 0;
        public int influence = 0;
        public int suspicion = 0;
    }

    public interface IJsonDistrictManager
    {
        DistrictInfo GetDistrictInfo(int id);
    }

    public partial class JsonDistrictManager : IJsonDistrictManager, IStartable
    {
        private Godot.Collections.Array districts;


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
            
            districts = loadedData["districts"].AsGodotArray();
        }


        public DistrictInfo GetDistrictInfo(int id)
        {
            DistrictInfo info = new();
            
            var neededCard = districts[id].AsGodotDictionary();
            
            if (neededCard.ContainsKey("id"))
            {
                info.id = neededCard["id"].AsInt16();
            }
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
    }

}

