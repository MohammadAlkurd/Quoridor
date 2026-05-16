using Godot;
using GodotPlugins.Game;
using System;
using System.Text.Json;
public partial class MainController : Node
{

    public MTcpClient client;
    public bool offlinemode;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }
    public void Offline()
    {
        offlinemode = true;
        GetTree().CallDeferred("change_scene_to_file", "res://game.tscn");
    }
    public void Join(string ip)
    {
        try
        {
            client = new MTcpClient(ip);
            client.Connected += Connected;
            client.Disconnected += Disconnected;
        }
        catch (System.Exception ex)
        {
            GD.Print(ex);
        }
    }
    void Disconnected(object sender, BetterEventArgs args)
    {
        GD.Print("DisConnected");
        GetTree().CallDeferred("change_scene_to_file", "res://menu.tscn");
    }
    void Connected(object sender, BetterEventArgs args)
    {
        GD.Print("Connected");
        GetTree().CallDeferred("change_scene_to_file", "res://game.tscn");
    }

}