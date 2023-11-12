using System.ComponentModel;
using System.Net.Sockets;

Memory<byte> buffer = new byte[] { 0,1,2,3,4,5,6,7 };
int openedConnections = 0;

while (true)
{
    try
    {
        TcpClient client = new();

        await client.ConnectAsync(args[0], 16000);

        await client.Client.SendAsync(buffer, SocketFlags.None);

        client.Close();

        openedConnections++;
    }
    catch (Exception ex)
    { 
        Console.WriteLine(ex);
    }

    if (openedConnections % 1000 == 0)
    {
        Console.WriteLine(openedConnections);
    }
}
