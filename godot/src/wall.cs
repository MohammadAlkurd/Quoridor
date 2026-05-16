using Godot;
using System;
using System.Drawing;

public partial class wall : piece
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		occupies = new Vector2[3];
		type = GetParent().Name == "Light_walls" ? "light" : "dark";
		base._Ready();
	}
	public override void groupstatechanged(object a)
	{
		base.groupstatechanged(a);
	}
	public override void turnchanged(bool turn)
	{
		base.turnchanged(turn);
	}
	public override bool movepiece(Vector2 newpos, ref bool[,] grid)
	{
		bool[,] tmp = (bool[,])grid.Clone();
		if (used) return false;
		if (newpos.X % 2 == 0 && (newpos.Y % 2 == 0)) return false;
		if (newpos.X % 2 != 0) //vertical
		{
			if (newpos.Y + 2 > 12) return false;
			for (int i = 0; i < 3; i++) { if (tmp[(int)newpos.X, (int)newpos.Y + i] == true) return false; }
			for (int i = 0; i < 3; i++) { tmp[(int)newpos.X, (int)newpos.Y + i] = true; }
			if (!main.DoesPathExist(tmp, false, main.playposlight))
				return false;
			if (!main.DoesPathExist(tmp, true, main.playposdark))
				return false;
			change_pos(newpos, true);
			grid = tmp;
			return true;
		}
		else//horizontal
		{
			if (newpos.X + 2 > 12) return false;
			for (int i = 0; i < 3; i++) { if (tmp[(int)newpos.X + i, (int)newpos.Y] == true) return false; }
			for (int i = 0; i < 3; i++) { tmp[(int)newpos.X + i, (int)newpos.Y] = true; }
			if (!main.DoesPathExist(tmp, false, main.playposlight))
				return false;
			if (!main.DoesPathExist(tmp, true, main.playposdark))
				return false;
			change_pos(newpos, false);
			grid = tmp;
			return true;
		}
	}
	public override void change_pos(Vector2 grid, bool vertical)
	{
		used = true;
		Disabled = true;
		ButtonPressed = false;
		if (vertical)
		{
			for (int i = 0; i < 3; i++) { occupies[i] = grid + (Vector2.Down * i); }
			Position = main.initial_pos + movementconst * ((grid - new Vector2(1, 0)) / 2);
			RotationDegrees = 0;
		}
		else
		{
			for (int i = 0; i < 3; i++) { occupies[i] = grid + (Vector2.Right * i); }
			Position = main.initial_pos_rotated + movementconst * ((grid - new Vector2(0, 1)) / 2);
			RotationDegrees = -90;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
