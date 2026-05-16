using System.Net.Sockets;
using overrides;
using Assistant;
public class client
{
    byte[] a = new byte[1];
    public bool removeme { get; private set; }
    public TcpClient _tcpClient { get; private set; }
    public bool myturn = false;
    public bool iswhiteplayer = false;
    public bool played = false;
    public client(object obj)
    {
        removeme = false;
        _tcpClient = (TcpClient)obj;
        RunRecievedata();
    }
    public void RunRecievedata()
    {
        Task task = new Task(() => receiveData());
        task.RunSynchronously();
    }

    public async void receiveData()
    {
        NetworkStream stream = _tcpClient.GetStream();
        while (true)
        {
            if (!Helper.SocketConnected(stream.Socket))
                break;
            try
            {
                if (stream.DataAvailable)
                {

                    byte[] responseData = await stream.ReadAsync();
                    if (responseData != null)
                    {
                        Console.WriteLine(responseData.ArrayToString());
                        if (responseData.First() != 0 && responseData.First() != 1)
                        {
                            await Program.SendToAllUsers(responseData, this);
                        }
                        else if (responseData.First() == 1 && iswhiteplayer && myturn)
                        {
                            await Program.SendToAllUsers(responseData, this);
                            played = true;
                        }
                        else if (responseData.First() == 0 && !iswhiteplayer && myturn)
                        {
                            await Program.SendToAllUsers(responseData, this);
                            played = true;
                        }
                    }
                }
            }
            catch (System.Exception exp)
            {
                this.removeme = true;
                Program.RemoveMe(this);
                return;
            }
            await Task.Delay(60);
            GC.Collect();
        }
        this.removeme = true;
        Program.RemoveMe(this);
        return;
    }

    public async Task WriteAsync(byte[] message)
    {
        NetworkStream stream = _tcpClient.GetStream();
        try
        {
            await stream.WriteAsync(message);
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }

    }
    public void dispose()
    {
        _tcpClient.Dispose();
    }
}