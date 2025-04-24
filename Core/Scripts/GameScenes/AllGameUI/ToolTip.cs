using Godot;
using System;
using System.ComponentModel;
using ZaSadka;
using static System.Net.Mime.MediaTypeNames;

public interface IToolTip
{
    void SetTitle(string title);
    void SetParameters(string parameters);
    void SetDescription(string description);
}

public partial class ToolTip : Control
{
    [Inject] private IPointerManager pointer;
    [Inject] private IInfluenceManager influenceManager;
    [Inject] private ISuspicionManager suspicionManager;
    [Inject] private IDistrictsManager districtsManager;
    
    [Export] private DistrictName districtName;
    [Export] private string title;

    [Export] private RichTextLabel label;
    [Export] private RichTextLabel parameters;
    [Export] private ColorRect rect;
    [Export] private MeshInstance2D meshInstance;
    [Export] private Node2D toolTip;

    [Export] private int headging_size = 50;
    [Export] private int text_size = 0;
    [Export] private Area2D control;
    [Export] private Color circle_color;

    public override void _Ready()
    {
        //meshInstance.SetInstanceShaderParameter("u_color", circle_color);

        string lines = "";
        lines += "Влияние: " + influenceManager.GetValue(districtName) + "\n";
        lines += "Подозрительность: " + suspicionManager.GetValue(districtName) + "\n";
        lines += "Поставки: " + districtsManager.GetSupply(districtName) + "\n";
        lines += "Спрос: " + districtsManager.GetDemand(districtName) + "\n";

        parameters.Text = lines;
        label.Text = title;
        toolTip.Visible = false;
        rect.Scale = new Vector2(1.0f, 1.0f);
        label.AddThemeFontSizeOverride("normal_font_size", headging_size);
        label.AddThemeColorOverride("default_color", circle_color);
        parameters.AddThemeFontSizeOverride("normal_font_size", text_size);

        control.MouseEntered += SetVisible;
        control.MouseExited += SetNotVisible;
    }

    public override void _Process(double delta)
    {
        string lines = "";
        if(influenceManager.GetValue(districtName) == int.MinValue)
        {
            lines += 0 + " влияние: " + "\n";
        }
        else
        {
            lines += influenceManager.GetValue(districtName) + " влияние" + "\n";
        }

        if (suspicionManager.GetValue(districtName) == int.MinValue)
        {
            lines += 0 + " подозрение: " + "\n";
        }
        else
        {
            lines += suspicionManager.GetValue(districtName) + " подозрение" + "\n";
        }

        lines += districtsManager.GetSupply(districtName) + " предложение" + "\n";
        lines += districtsManager.GetDemand(districtName) + " спрос" + "\n";

        parameters.Text = lines;
    }

    public override void _ExitTree()
    {
        control.MouseEntered -= SetVisible;
        control.MouseExited -= SetNotVisible;
    }

    private void SetVisible()
    {
        toolTip.Visible = true;
    }

    private void SetNotVisible()
    {
        toolTip.Visible = false;
    }
}
