using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Assistant;
using Microsoft.Extensions.Configuration;
using overrides;
public class Program
{
    public static config config { get; private set; } = new config();
    static Dictionary<int, client> clients = new Dictionary<int, client>();
    public static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false);

        IConfigurationRoot configuration = builder.Build();
        IConfigurationSection section = configuration.GetSection("server");

        section.Bind(config);
        TcpListener listener = new TcpListener(IPAddress.Parse("10.10.10.252"), config.Port);
        listener.Start();
        Console.WriteLine("Code To Use = " + Helper.ConvertIPTOString("10.10.10.252" + ":" + config.Port));
        new Task(() => GameLoop()).Start();
        while (true)
        {
            await ClientJoin(listener);
            await Task.Delay(60);
            GC.Collect();
        }
    }
    static bool gamestarted = false;
    static client white = null;
    static client black = null;
    static bool turn = false; //false = black true = white
    static Random random = new Random();
    static public async void GameLoop()
    {

        while (true)
        {
            if (Console.KeyAvailable)
            {
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
            if (clients.Count == 2 && !gamestarted)
            {
                gamestarted = true;
                int tmp = (int)MathF.Round(random.NextSingle());
                white = clients.ElementAt(tmp).Value;
                black = clients.ElementAt(1 - tmp).Value;

                Console.WriteLine("game started");
                await white.WriteAsync(new byte[] { 1, 1, 1, 1, 1, 1 });
                await black.WriteAsync(new byte[] { 0, 0, 0, 0, 0, 0 });
                white.iswhiteplayer = true;
                black.iswhiteplayer = false;
                white.myturn = true;
            }
            if (!gamestarted)
                continue;
            if (!turn && white.played)
            {
                turn = true;
                white.played = false;
                white.myturn = false;
                black.myturn = true;
            }
            if (turn && black.played)
            {
                turn = false;
                black.played = false;
                black.myturn = false;
                white.myturn = true;
            }
            await Task.Delay(60);
            GC.Collect();
        }

    }
    public static async void RemoveMe(client client)
    {
        clients.Remove(clients.Where(e => e.Value == client).First().Key);
        client.dispose();
        client = null;
        gamestarted = false;
        GC.Collect();
        Console.WriteLine("client disconnected");
    }
    private static async Task ClientJoin(TcpListener listener)
    {
        if (!listener.Pending())
        {
            return;
        }
        TcpClient temp = await listener.AcceptTcpClientAsync();
        await Task.Delay(15);
        NetworkStream tempStream = temp.GetStream();
        byte[] responseData = await tempStream.ReadAsync();
        if (!(clients.Count >= config.Maxplayers))
            if (!clients.ContainsKey(responseData.GetHashCode()))
            {
                await SendToAllUsers(new byte[] { 10, 10, 10, 10, 10, 10 }, null);
                clients.Add(responseData.GetHashCode(), new client(temp));
                await tempStream.WriteAsync(new byte[] { 10, 10, 10, 10, 10, 10 });
                Console.WriteLine("Successfully connected a client with a responseData of : " + responseData.ArrayToString());
            }
            else await tempStream.WriteAsync(new byte[] { 100, 100, 100, 100, 100, 100 }); //Someone connnected at the EXACT SAME TIME!!
        else await tempStream.WriteAsync(new byte[] { 200, 200, 200, 200, 200, 200 }); //Server full
    }
    public static async Task SendToAllUsers(byte[] message, client? sender)
    {
        foreach (var item in clients)
        {
            if (item.Value != sender)
                await item.Value.WriteAsync(message);
        }
    }
}