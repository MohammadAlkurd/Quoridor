using Godot;
using System;
using System.Threading.Channels;

public partial class Player : piece
{
	public override void _Ready()
	{
		occupies = new Vector2[1];
		type = Name == "LightPiece" ? "light" : "dark";
		if (type == "light")
			occupies[0] = new Vector2(6, 12);
		else
			occupies[0] = new Vector2(6, 0);

		main.playposlight = new Vector2(6, 12);
		main.playposdark = new Vector2(6, 0);
		base._Ready();
	}
	public override void groupstatechanged(object a)
	{
		base.groupstatechanged(a);
	}

	//TODO:make the player piece move 
	public override bool movepiece(Vector2 newpos, ref bool[,] grid)
	{
		bool[,] tmp1 = (bool[,])grid.Clone();
		Vector2 currpos = main.convert_mouse_to_grid(Position);
		if (newpos.X % 2 != 0 || (newpos.Y % 2 != 0)) return false;
		if (tmp1[(int)newpos.X, (int)newpos.Y] == true) return false;
		if (currpos.DistanceTo(newpos) >= 2.5)
			return false;
		Vector2 tmp = currpos - newpos;
		if (tmp1[(int)(currpos.X - (int)tmp.X / 2), (int)(currpos.Y - tmp.Y / 2)] == true)
		{
			return false;
		}
		if (occupies != null)
		{
			tmp1[(int)occupies[0].X, (int)occupies[0].Y] = false;
		}
		tmp1[(int)newpos.X, (int)newpos.Y] = true;

		if (!main.DoesPathExist(tmp1, false, main.playposlight) && type != "light")
			return false;
		if (!main.DoesPathExist(tmp1, true, main.playposdark) && type == "light")
			return false;
		change_pos(newpos, false);

		grid = tmp1;
		if (type == "light")
		{
			main.playposlight = newpos;
		}
		else
			main.playposdark = newpos;
		return true;
	}
	public override void change_pos(Vector2 grid, bool vertical)
	{
		occupies[0] = grid;
		GD.Print(grid);
		Position = main.initial_pos_player + movementconst * (grid / 2);
	}
}