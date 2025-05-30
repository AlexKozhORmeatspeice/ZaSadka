using Cards;
using DI;
using Godot;
using Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ZaSadka;

namespace Game_events
{
    public struct EventInfo
    {
        public EventInfo() { }

        public int ID = -1;

        public string name = "";
        
        public bool canActivate = false;

        public ChoiceData yes_choice;
        public ChoiceData no_choice;
    }

    public interface IEventsFabric
    {
        EventInfo GetRandomEvent(DistrictName district);
    }

    public partial class JsonEventsFabric : IEventsFabric, IStartable, ILateStartable
    {
        [Inject] private IChoiceFabric choiceFabric;
        [Inject] private IInfluenceManager influenceManager;
        [Inject] private ISuspicionManager suspicionManager;
        [Inject] private IDistrictsManager districtsManager;

        private Godot.Collections.Array events;
        private List<EventInfo> eventInfos;

        public void Start()
        {
            LoadJson();
        }

        public void LateStart()
        {

        }

        //сохраняет все возможные ивенты, с вероятностью 50/50 выдаёт рандомный из них
        public EventInfo GetRandomEvent(DistrictName district)
        {
            EventInfo info = new();
            List<EventInfo> possibleEvents = [];

            for (int i = 0; i < events.Count; i++)
            {
                Godot.Collections.Dictionary _event = events[i].AsGodotDictionary();

                info = LoadInfo(_event, district);

                if (info.canActivate)
                {
                    possibleEvents.Add(info);
                }
            }

            int EventChance = GD.RandRange(0, 1);

            if (possibleEvents.Count == 0 || EventChance == 0)
            {
                return new EventInfo();
            }

            int randIndex = GD.RandRange(0, possibleEvents.Count - 1);

            foreach (var posEv in possibleEvents)
            {
                GD.Print(posEv.name);
            }

            return possibleEvents[randIndex];
        }

        private EventInfo LoadInfo(Godot.Collections.Dictionary _event, DistrictName district)
        {
            EventInfo info = new EventInfo();

            if (_event.ContainsKey("id"))
            {
                info.ID = _event["id"].AsInt16();
            }
            if (_event.ContainsKey("name"))
            {
                info.name = _event["name"].AsString();
            }

            if (_event.ContainsKey("condition"))
            {
                info.canActivate = CheckConditions(_event["condition"].AsString(), district);
            }

            if(_event.ContainsKey("choice_yes"))
            {
                info.yes_choice = choiceFabric.GetChoiceByID(_event["choice_yes"].AsInt16(), district);
            }

            if (_event.ContainsKey("choice_no"))
            {
                info.no_choice = choiceFabric.GetChoiceByID(_event["choice_no"].AsInt16(), district);
            }

            return info;
        }

        //я в рот ебал парсить эти json-ы блять
        private bool CheckConditions(string conditionsString, DistrictName district)
        {
            if (conditionsString.Contains("&"))
            {
                string[] conditions = conditionsString.Split('&');
                bool result = true;
                foreach (var condition in conditions)
                {
                    result = CheckCondition(condition, district) && result;
                }

                return result;
            }
            else if (conditionsString.Contains("|"))
            {
                string[] conditions = conditionsString.Split('|');

                bool result = false;
                foreach (var condition in conditions)
                {
                    result = CheckCondition(condition, district) && result;

                    result = CheckCondition(condition, district) || result;
                }

                return result;
            }
            else
            {
                return CheckCondition(conditionsString, district);
            }
        }


        private bool CheckCondition(string condition, DistrictName district)
        {
            string[] symbols = [">", "<", "="];
            string checkSym = "";

            foreach (var symbol in symbols)
            {
                if (condition.Contains(symbol))
                {
                    checkSym = symbol;
                    break;
                }
            }

            string[] vals = condition.Replace(" ", "").Split(checkSym);
            switch (checkSym)
            {
                case ">":
                    List<int> values = GetValuesByName(vals[0], district);
                    return values.Any(x => x > int.Parse(vals[1]));
                case "<":
                    return GetValuesByName(vals[0], district).Any(x => x < int.Parse(vals[1]));
                case "=":
                    return GetValuesByName(vals[0], district).Any(x => x == int.Parse(vals[1]));
                default:
                    return false;
            }
        }

        private List<int> GetValuesByName(string name, DistrictName district)
        {
            List<int> values = new List<int>();
            DistrictData data = districtsManager.GetData(district);

            switch (name)
            {
                case "infl":
                    values.Add(influenceManager.GetValue(district));
                    break;
                case "units":
                    foreach (var info in data.itemInfoByID.Values)
                    {
                        if (info.type == ItemType.unit)
                        {
                            values.Add(info.spriteId);
                        }
                    }
                    break;
                case "sus":
                    values.Add(suspicionManager.GetValue(district));
                    break;
                case "build":
                    foreach (var info in data.itemInfoByID.Values)
                    {
                        if (info.type == ItemType.building)
                        {
                            values.Add(info.spriteId);
                        }
                    }
                    break;
                default:
                    break;
            }

            return values;
        }

        private void LoadJson()
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

            events = loadedData["events"].AsGodotArray();
        }
    }
}


