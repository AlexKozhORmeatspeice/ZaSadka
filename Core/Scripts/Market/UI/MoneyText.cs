using Godot;
using System;


namespace Market
{
    public interface IMoneyText
    {
        string MoneyText { set; }
    }

    public partial class MoneyText : Label, IMoneyText
    {
        [Export] private Label label;
        string IMoneyText.MoneyText { set => label.Text = value; }
    }

}
