using Godot;
using System;

public partial class Offline : Button
{
	public override void _Ready()
	{
		Pressed += GoOff;
	}
	public void GoOff()
	{
		MainController controller = (MainController)GetNode("/root/MainController");
		controller.Offline();
	}
	public override void _Process(double delta)
	{
	}
}
