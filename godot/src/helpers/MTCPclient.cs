using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using overrides;
using System.Collections.Generic;
using System.Linq;
using Godot;
public delegate void ReqHandler(object sender, Received e);
public delegate void BetterEventHandler(object sender, BetterEventArgs e);
public class MTcpClient
{
    public event BetterEventHandler Connected;
    public event BetterEventHandler Disconnected;
    public event ReqHandler Reqreceived;
    private TcpClient client { set; get; }
    private NetworkStream stream { set; get; }
    //setup the name later
    private string name { get; set; }
    public MTcpClient(string serverip)
    {
        client = new TcpClient(); Connect(serverip);
    }
    bool connected { get; set; }
    public enum Type
    {
        join,
        left,
        game,
        error
    }
    async void Connect(string serverip)
    {
        try
        {
            if (Helper.IsValidURL(serverip))
            {
                int port = serverip.Contains(':') ? int.TryParse(serverip.Split(':')[1], out port) ? port : 13000 : 13000;
                serverip = serverip.Contains(':') ? serverip.Split(':')[0] : serverip;
                GD.Print(serverip);
                GD.Print(port);
                await client.ConnectAsync(serverip.Trim(), 13000);
                stream = client.GetStream();
                await Send(Type.join);
                byte[] res = await stream.ReadAsync(6);
                GD.Print(res.ArrayToString());
                if (res.SequenceEqual(new byte[] { 10, 10, 10, 10, 10, 10 }))
                {
                    connected = true;
                    OnConnect(new BetterEventArgs(""));
                }
                else
                    throw new Exception("Did not Join");
            }
            else
            {
                throw new Exception("InvalidUrl");
            }
        }
        catch (System.Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        Receiver();
    }
    async void Receiver()
    {
        try
        {
            while (connected)
            {
                if (!Helper.SocketConnected(stream.Socket))
                    break;
                if (stream.DataAvailable)
                {
                    //handle data recieving
                    byte[] responseData = await stream.ReadAsync(6);
                    await Task.Delay(1);
                    OnReqReceived(new Received(responseData));
                }
                await Task.Delay(30);
            }
            connected = false;
            OnDisconnect(new BetterEventArgs("something went wrong , could the server be off ?"));
            client.Dispose();
        }
        catch (System.Exception ex)
        {
            connected = false;
            OnDisconnect(new BetterEventArgs("something went wrong"));
            GD.Print(ex.Message);
            client.Dispose();
            throw ex;
        }
    }

    public async Task Send(Type type, byte[] state = null)
    {
        GD.Print(state.ArrayToString());
        try
        {
            switch (type)
            {
                case Type.join: await stream.WriteAsync(Helper.GetToSend((int)Type.join)); break;
                case Type.left: await stream.WriteAsync(Helper.GetToSend((int)Type.left)); break;
                case Type.game: await stream.WriteAsync(Helper.GetToSend((int)Type.game, state)); break;
                default: return;
            }
        }
        catch (System.Exception ex)
        {
            connected = false;
            OnDisconnect(new BetterEventArgs("something went wrong"));
            client.Dispose();
            throw ex;
        }
    }
    protected virtual void OnReqReceived(Received received)
    {
        //if ProcessCompleted is not null then call delegate
        Reqreceived?.Invoke(this, received);
    }
    protected virtual void OnDisconnect(BetterEventArgs received)
    {
        //if ProcessCompleted is not null then call delegate
        Disconnected?.Invoke(this, received);
    }
    protected virtual void OnConnect(BetterEventArgs received)
    {
        //if ProcessCompleted is not null then call delegate
        Connected?.Invoke(this, received);
    }
    ~MTcpClient()
    {
        client.Dispose();
    }
}