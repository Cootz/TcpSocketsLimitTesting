using System.ComponentModel;
using System.Net.Sockets;

internal class Program
{
    public static volatile int openedConnections = 0;

    private static async Task Main(string[] args)
    {
        Memory<byte> buffer = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        

        while (true)
        {
            _ = CreateNewConnection();

            if (openedConnections % 1000 == 0)
            {
                Console.WriteLine(openedConnections);
            }
        }

        async Task CreateNewConnection()
        {
            try
            {
                TcpClient client = new();

                await client.ConnectAsync(args[0], 16000);

                await client.Client.SendAsync(buffer, SocketFlags.None);

                client.Close();

                Interlocked.Increment(ref openedConnections);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}