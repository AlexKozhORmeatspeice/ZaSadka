using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI;
using Godot;
using Market;
using ZaSadka;
using static System.Collections.Specialized.BitVector32;

namespace Game_events
{

    public enum StatType
    {
        none,
        supply,
        demand,
        influence,
        suspicion,
        building,
        card,
        unit,
        money
    }

    public enum ChangeStatType
    {
        subtract,
        add,
        equal,
        randDestroy
    }

    public struct ActionData
    {
        public StatType type;
        public ChangeStatType changeStatType;
        public int value;
    }

    public struct ChoiceData
    {
        public ChoiceData() {}

        public int ID = 0;
        public DistrictName districtName;
        public string name = "Xui";
        public float chance = 0.0f;

        public List<ActionData> actionsData;
    }

    public interface IChoiceFabric
    {
        ChoiceData GetChoiceByID(int id, DistrictName districtName);
    }

    internal class JsonChoiceFabric : IChoiceFabric, IStartable
    {
        [Inject] private IObjectResolver resolver;

        private Godot.Collections.Array choices;

        public void Start()
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

            choices = loadedData["choices"].AsGodotArray();
        }

        public ChoiceData GetChoiceByID(int id, DistrictName districtName)
        {
            ChoiceData data = new ChoiceData();
            data.districtName = districtName;

            Godot.Collections.Dictionary _choice = choices[id].AsGodotDictionary();

            if (_choice.ContainsKey("id"))
            {
                data.ID = _choice["id"].AsInt16();
            }
            if (_choice.ContainsKey("name"))
            {
                data.name = _choice["name"].AsString();
            }

            if (_choice.ContainsKey("chance"))
            {
                data.chance = (float)_choice["chance"].AsInt16() / 100.0f;
            }

            if (_choice.ContainsKey("action"))
            {
                data.actionsData = GetActionsData(_choice["action"].AsString(), districtName);
            }

            return data;
        }

        //я в рот ебал парсить эти json-ы блять
        private List<ActionData> GetActionsData(string actionsString, DistrictName district)
        {
            List<ActionData> actionsData = new List<ActionData>();

            if (actionsString.Contains("&"))
            {
                string[] actions = actionsString.Split('&');

                foreach (var action in actions)
                {
                    actionsData.Add(CreateAction(action, district));
                }
            }
            else
            {
                actionsData.Add(CreateAction(actionsString, district));
            }

            return actionsData;
        }


        private ActionData CreateAction(string condition, DistrictName district)
        {
            ActionData actionData = new ActionData();

            string[] symbols = { "+", "-", "=", "randDestroy"};
            string checkSym = "";

            foreach (var symbol in symbols)
            {
                if (condition.Contains(symbol))
                {
                    checkSym = symbol;
                    break;
                }
            }

            if (checkSym == "")
                return actionData;

            //get change stat type
            string[] vals = condition.Replace(" ", "").Split(checkSym);
            switch (checkSym)
            {
                case "+":
                    actionData.changeStatType = ChangeStatType.add;
                    break;
                case "-":
                    actionData.changeStatType = ChangeStatType.subtract;
                    break;
                case "=":
                    actionData.changeStatType = ChangeStatType.equal;
                    break;
                case "randDestroy":
                    actionData.changeStatType = ChangeStatType.randDestroy;
                    break;
                default:
                    GD.Print("WARNING: WRONG ACTION DATA - " + condition);
                    return actionData;
            }

            //get val type
            switch (vals[0])
            {
                case "buildings":
                    actionData.type = StatType.building;
                    break;
                case "cards":
                    actionData.type = StatType.card;
                    break;
                case "units":
                    actionData.type = StatType.unit;
                    break;
                case "money":
                    actionData.type = StatType.money;
                    break;
                case "sus":
                    actionData.type = StatType.suspicion;
                    break;
                case "infl":
                    actionData.type = StatType.influence;
                    break;
                case "supply":
                    actionData.type = StatType.supply;
                    break;
                case "demand":
                    actionData.type = StatType.demand;
                    break;
                default:
                    GD.Print("WARNING: WRONG ACTION DATA - " + condition);
                    return actionData;
            }

            //get val
            actionData.value = int.Parse(vals[1]);

            return actionData;
        }
    }
}
