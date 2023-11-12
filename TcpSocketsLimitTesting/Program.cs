using System.Net;
using System.Net.Sockets;

internal class Program
{
    public static Dictionary<IPEndPoint, List<DateTime>> Connections { get; } = new();

    private static async Task Main(string[] args)
    {
        System.Timers.Timer timer = new();
        timer.Elapsed += (_,_) => displayReceivedConnections();
        timer.Interval = 2000; // in miliseconds
        
        TcpListener listener = new(IPAddress.Parse(args[0]), 16000);

        timer.Start();
        listener.Start();

        try
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;

                bool connectionExists = Connections.TryGetValue(clientEndPoint, out var connectionDateTimes);

                if (connectionExists)
                {
                    connectionDateTimes!.Add(DateTime.Now);
                }
                else
                {
                    Connections.Add(clientEndPoint, new List<DateTime>() { DateTime.Now });
                }

                _ = ProcessTcpClient(client);
            }
        }
        finally
        {
            displayReceivedConnections();
        }

        static void displayReceivedConnections()
        {
            const int timeSpan = 2;

            DateTime dateTimeOffset = DateTime.Now - TimeSpan.FromMinutes(timeSpan);

            int connectionsReceived = 0;

            foreach (KeyValuePair<IPEndPoint, List<DateTime>> connection in Connections)
            {
                foreach (DateTime dateTime in connection.Value)
                {
                    if (dateTime >= dateTimeOffset)
                    {
                        connectionsReceived++;
                    }
                }
            }

            Console.WriteLine($"Connections received for last {timeSpan} min(s): {connectionsReceived}");
        }
    }

    public static async Task ProcessTcpClient(TcpClient client)
    {
        Memory<byte> buffer = new byte[512];

        await client.Client.ReceiveAsync(buffer, SocketFlags.None);

        client.Close();
    }
}