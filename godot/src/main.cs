using Godot;
using overrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

public partial class main : mainClass
{

	public override void _Ready()
	{
		mainController = (MainController)GetNode("/root/MainController");
		grid[6, 0] = true;
		grid[6, 12] = true;
		Restartbutton.Pressed += Restartgame;
		Restartbutton2.Pressed += Restartgame;
		Length = (int)Math.Sqrt(grid.Length);
		if (!mainController.offlinemode)
			mainController.client.Reqreceived += OnReqRecieved;
	}
	public void gamestart()
	{
		turn = false;
		grid = new bool[13, 13];
		grid[6, 0] = true;
		grid[6, 12] = true;
		RotationDegrees = lightside ? 0 : 180;
		Position = lightside ? Position : new Vector2(540, 980);
		EmitSignal(SignalName.TurnChanged, false);
		EmitSignal(SignalName.Restart);
	}
	public void gamestop()
	{
		Restartgame();
		playersready = false;
		lightside = true;
		GD.Print("other player left");
		//handle reconnecting logic or whatever if neccery
	}

	public void Restartgame()
	{
		WinningScreen.Visible = false;
		turn = false;
		grid = new bool[13, 13];
		grid[6, 0] = true;
		grid[6, 12] = true;
		RotationDegrees = lightside ? 0 : 180;
		Position = lightside ? Position : new Vector2(540, 960);

		Restartbutton.RotationDegrees = lightside ? 0 : 180;
		Restartbutton.Position = lightside ? new Vector2(60, 30) : new Vector2(480, 930);
		EmitSignal(SignalName.TurnChanged, false);
		EmitSignal(SignalName.Restart);
	}
	public void movepeice(byte[] move)
	{
		if (move.Length != 6)
			return;

		if (move[0] == 0 && turn && move[0] == 1 && !turn)
			return;
		if (move[1] == 0)
		{
			wall tmp = move[0] == 1 ? (wall)light_walls.GetChild(move[2] - 1) : (wall)dark_walls.GetChild(move[2] - 1);
			if (tmp == null)
				return;
			GD.Print(move[4], move[5]);
			if (!tmp.movepiece(new Vector2(move[4], move[5]), ref this.grid))
				return;

		}
		else
		if (move[1] == 1)
		{
			wall tmp = (wall)dark_walls.GetChild(move[2] - 1);
			if (tmp == null)
				return;
			if (!tmp.movepiece(new Vector2(move[4], move[5]), ref this.grid))
				return;

		}
		else return;
		EmitSignal(SignalName.TurnChanged, !turn);
		turn = !turn;
	}
	public void move_peice_to_grid(Vector2 newpos)
	{

		ButtonGroup group;
		group = turn ? group_dark : group_light;
		piece tmp = (piece)group.GetPressedButton();
		if (tmp == null)
			return;
		Vector2 oldpos = tmp is wall ? new Vector2(int.Parse(tmp.Name.ToString().Last().ToString()), 0) : tmp.occupies[0];

		if (newpos.X % 2 != 0 && newpos.Y % 2 != 0) return;

		if (!tmp.movepiece(newpos, ref this.grid))
			return;

		int type = tmp is wall ? 0 : 1;
		if (!mainController.offlinemode)
			sendstate((!turn).booltobyte(), (byte)type, oldpos, newpos);
		if (tmp is Player)
		{
			if (tmp.type == "light")
			{
				WinningScreen.Visible = tmp.occupies[0].Y == 0;
				RichTextLabel text = WinningScreen as RichTextLabel;
				text.Text = "[center]WHITE WON";
			}
			else
			{
				WinningScreen.Visible = tmp.occupies[0].Y == 12;
				RichTextLabel text = WinningScreen as RichTextLabel;
				text.Text = "[center]BLACK WON";
			}
		}
		EmitSignal(SignalName.TurnChanged, !turn);
		group.GetButtons().ToList().ForEach(x => x.ButtonPressed = false);
		turn = !turn;
		tmp.ReleaseFocus();
	}
	public async void sendstate(byte color, byte type, Vector2 oldpos, Vector2 newpos)
	{
		try
		{
			await mainController.client.Send(MTcpClient.Type.game, new byte[] { color, type, (byte)oldpos.X, (byte)oldpos.Y, (byte)newpos.X, (byte)newpos.Y });
		}
		catch (System.Exception)
		{
			errmsg = "something went wrong";
		}
	}


	void OnReqRecieved(object sender, Received rec)
	{
		try
		{

			switch (rec.type)
			{
				case (int)ReqType.join:
					gamestart();
					break;
				case (int)ReqType.left:
					gamestop();
					break;

				case (int)ReqType.game:
					reqrecieved(rec);
					break;
				default: return;
			}
		}
		catch (Exception e)
		{
			GD.Print(e);
		}
	}
	public void reqrecieved(Received rec)
	{
		if (rec.message.SequenceEqual(new byte[] { 1, 1, 1, 1, 1, 1 }) && rec.type == 2)
		{
			lightside = true;
			gamestart();
			return;
		}
		if (rec.message.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0 }) && rec.type == 2)
		{
			lightside = false;
			gamestart();
			return;
		}
		movepeice(rec.message);
		GD.Print(rec.message.ArrayToString());
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.R))
		{
			Restartgame();
		}
		if (Input.IsActionJustReleased("mouseclick"))
		{
			GD.Print(convert_mouse_to_grid(GetLocalMousePosition()));
			move_peice_to_grid(convert_mouse_to_grid(GetLocalMousePosition()));

		}
	}

}
