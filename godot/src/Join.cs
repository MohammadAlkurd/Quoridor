using Godot;
using System;

public partial class Join : Button
{

    public override void _Ready()
    {
        Pressed += JoinS;
    }
    public void JoinS()
    {
        LineEdit code = (LineEdit)GetNode("../CodeI");
        if (String.IsNullOrEmpty(code.Text.Trim()))
        {
            GD.Print("please enter a valid code");
            return;
        }
        MainController controller = (MainController)GetNode("/root/MainController");
        controller.Join(Helper.ConvertStringToIP(code.Text.Trim()));
        GD.Print(Helper.ConvertStringToIP(code.Text.Trim()));
        GD.Print(Helper.ConvertIPTOString(Helper.ConvertStringToIP(code.Text.Trim())));
    }
    public override void _Process(double delta)
    {
    }
}
