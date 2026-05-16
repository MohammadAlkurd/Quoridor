using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class piece : Button
{
	public Vector2[] occupies = new Vector2[3];
	[Export]
	public Vector2 movementconst;
	public string type;
	[Export]
	public main main;
	public MainController mainController;
	public bool used;
	Vector2 defaultpos;
	public override void _Ready()
	{
		mainController = (MainController)GetNode("/root/MainController");
		defaultpos = Position;
		main.TurnChanged += turnchanged;
		ButtonGroup.Pressed += groupstatechanged;
		main.Restart += backtodefaultpos;
		ReleaseFocus();
		if (type == "light")
		{
			Disabled = mainController.offlinemode ? false : !main.lightside;
			StyleBoxFlat tmp = (StyleBoxFlat)GetThemeStylebox("normal");
			tmp.ShadowSize = this is Player ? 500 : 20;
			AddThemeStyleboxOverride("normal", tmp);
		}
		else
		{
			if (!main.lightside)
			{
				Disabled = false;
				StyleBoxFlat tmp = (StyleBoxFlat)GetThemeStylebox("normal");
				tmp.ShadowSize = this is Player ? 500 : 20;
				AddThemeStyleboxOverride("normal", tmp);
			}

		}

	}

	public virtual void backtodefaultpos()
	{
		Position = defaultpos;
		RotationDegrees = 0;
		used = false;
		turnchanged(false);
	}
	public virtual bool movepiece(Vector2 newpos, ref bool[,] grid)
	{
		return false;
	}
	public virtual void groupstatechanged(object a)
	{
		if (ButtonGroup.GetPressedButton() != this)
		{
			StyleBoxFlat tmp = (StyleBoxFlat)GetThemeStylebox("normal");
			tmp.ShadowSize = 0;
			AddThemeStyleboxOverride("normal", tmp);
		}
	}
	public virtual void turnchanged(bool turn)
	{
		ReleaseFocus();
		if (used) return;
		if (type == "light")
		{
			if (!main.lightside)
			{
				if (!mainController.offlinemode)
				{
					Disabled = true;
					return;
				}
			}

			Disabled = turn;
			StyleBoxFlat tmp = (StyleBoxFlat)GetThemeStylebox("normal");
			tmp.ShadowSize = !turn ? this is Player ? 500 : 20 : 0;
			AddThemeStyleboxOverride("normal", tmp);
		}
		else
		{
			if (main.lightside)
			{
				if (!mainController.offlinemode)
				{
					Disabled = true;
					return;
				}
			}
			Disabled = !turn;
			StyleBoxFlat tmp = (StyleBoxFlat)GetThemeStylebox("normal");
			tmp.ShadowSize = turn ? this is Player ? 500 : 20 : 0;
			AddThemeStyleboxOverride("normal", tmp);
		}
	}
	public virtual void change_pos(Vector2 grid, bool vertical)
	{
	}
}
