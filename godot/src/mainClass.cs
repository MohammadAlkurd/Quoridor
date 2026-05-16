using Godot;
using overrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

public partial class mainClass : Node2D
{
    [Signal]
    public delegate void TurnChangedEventHandler(bool turn);
    [Signal]
    public delegate void RestartEventHandler();
    public bool[,] grid = new bool[13, 13];
    [Export]
    public Button Restartbutton;
    [Export]
    public Button Restartbutton2;
    [Export]
    public Control WinningScreen;
    [Export]
    public ButtonGroup group_light;
    [Export]
    public ButtonGroup group_dark;
    [Export]
    public Node2D light_walls;
    [Export]
    public Node2D dark_walls;
    [Export]
    public Player light_player;
    [Export]
    public Player dark_player;
    public bool turn = false;
    [Export]
    public float block_const = 60;
    [Export]
    public float space_const = 20;
    [Export]
    public Vector2 grid_start;
    [Export]
    public Vector2 initial_pos;
    [Export]
    public Vector2 initial_pos_rotated;
    [Export]
    public Vector2 initial_pos_player;
    public bool lightside = true;


    public bool playersready = false;
    public MainController mainController;

    public string errmsg = "";
    public enum ReqType
    {
        join,
        left,
        game
    }
    public Vector2 convert_mouse_to_grid(Vector2 mouseclick)
    {
        Vector2 corrected = mouseclick - grid_start;
        if (corrected.X <= 0) return Vector2.Zero;
        if (corrected.Y <= 0) return Vector2.Zero;
        int x = corrected.X % 80 > 60 ? ((int)(corrected.X / 80) * 2) + 1 : (int)(corrected.X / 80) * 2;
        int y = corrected.Y % 80 > 60 ? ((int)(corrected.Y / 80) * 2) + 1 : (int)(corrected.Y / 80) * 2;
        return new Vector2(x, y);
    }


    public float distanceto(Vector2[] target, Vector2 currpos)
    {
        List<float> a = new List<float>();
        foreach (var item in target)
        {
            a.Add(currpos.DistanceTo(item));
        }
        return a.Min();
    }
    struct discheck
    {
        public float distance;
        public bool check;

        public discheck(float distance) : this()
        {
            this.distance = distance;
        }
    }
    public int Length = 0;
    public Vector2 playposlight;
    public Vector2 playposdark;
    public bool DoesPathExist(bool[,] grid, bool type, Vector2 playerpos)
    {

        int iterations = 0;
        discheck[,] dischecks;
        dischecks = new discheck[Length, Length];
        for (int i = 0; i < Length; i++)
        {
            for (int b = 0; b < Length; b++)
            {
                dischecks[i, b] = new discheck(0);
            }
        }
        Vector2[] targets = new Vector2[Length];
        Vector2 currpos = playerpos;
        for (int i = 0; i < Length; i++)
        {
            targets[i] = type ? new Vector2(i, Length - 1) : new Vector2(i, 0);
        }
        while (iterations < grid.Length / 5)
        {
            dischecks[(int)currpos.X, (int)currpos.Y] = new discheck(distanceto(targets, currpos))
            {
                check = true
            };
            if (currpos.X + 2 <= Length - 1 && grid[(int)currpos.X + 1, (int)currpos.Y] != true && grid[(int)currpos.X + 2, (int)currpos.Y] != true)
            {
                dischecks[(int)currpos.X + 2, (int)currpos.Y].distance = distanceto(targets, currpos + (Vector2.Right * 2));
                if (targets.Contains(currpos + (Vector2.Right * 2)))
                    return true;
            }
            if (currpos.X - 2 >= 0 && grid[(int)currpos.X - 1, (int)currpos.Y] != true && grid[(int)currpos.X - 2, (int)currpos.Y] != true)
            {
                dischecks[(int)currpos.X - 2, (int)currpos.Y].distance = distanceto(targets, currpos + (Vector2.Left * 2));
                if (targets.Contains(currpos + (Vector2.Left * 2)))
                    return true;
            }

            if (currpos.Y + 2 <= Length - 1 && grid[(int)currpos.X, (int)currpos.Y + 1] != true && grid[(int)currpos.X, (int)currpos.Y + 2] != true)
            {
                dischecks[(int)currpos.X, (int)currpos.Y + 2].distance = distanceto(targets, currpos + (Vector2.Down * 2));
                if (targets.Contains(currpos + (Vector2.Down * 2)))
                    return true;
            }

            if (currpos.Y - 2 >= 0 && grid[(int)currpos.X, (int)currpos.Y - 1] != true && grid[(int)currpos.X, (int)currpos.Y - 2] != true)
            {
                dischecks[(int)currpos.X, (int)currpos.Y - 2].distance = distanceto(targets, currpos + (Vector2.Up * 2));
                if (targets.Contains(currpos + (Vector2.Up * 2)))
                    return true;
            }
            int minDistanceIndex = 0;
            try
            {
                minDistanceIndex = dischecks.Cast<discheck>()
                                    .Select((value, index) => new { Value = value, Index = index })
                                    .Where(item => !item.Value.check && item.Value.distance != 0)
                                    .MinBy(item => item.Value.distance).Index;
            }
            catch (System.Exception)
            {
                string a = "";
                for (int i = 0; i < Length; i += 2)
                {
                    for (int b = 0; b < Length; b += 2)
                    {
                        a += dischecks[b, i].distance + " ";
                    }
                    GD.Print(a);
                    a = "";
                }
                GD.Print("-----------------------");
                for (int i = 0; i < Length; i += 2)
                {
                    for (int b = 0; b < Length; b += 2)
                    {
                        a += dischecks[b, i].check ? 1 + " " : 0 + " ";
                    }
                    GD.Print(a);
                    a = "";
                }
                return false;
            }

            currpos = convertinttogrid(minDistanceIndex);
            iterations++;

        }
        // string a = "";
        // for (int i = 0; i < Length; i += 2)
        // {
        // 	for (int b = 0; b < Length; b += 2)
        // 	{
        // 		a += dischecks[b, i].distance + " ";
        // 	}
        // 	GD.Print(a);
        // 	a = "";
        // }
        // GD.Print("-----------------------");
        // for (int i = 0; i < Length; i += 2)
        // {
        // 	for (int b = 0; b < Length; b += 2)
        // 	{
        // 		a += dischecks[b, i].check ? 1 + " " : 0 + " ";
        // 	}
        // 	GD.Print(a);
        // 	a = "";
        // }
        // GD.Print("-----------------------");
        // for (int i = 0; i < Length; i++)
        // {
        // 	for (int b = 0; b < Length; b++)
        // 	{
        // 		a += grid[b, i] ? 1 + " " : 0 + " ";
        // 	}
        // 	GD.Print(a);
        // 	a = "";
        // }

        return false;
    }
    Vector2 convertinttogrid(int input)
    {
        return new Vector2(Mathf.Floor(input / Length), input % Length);
    }

}
