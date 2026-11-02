using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetNetworkTcpChatListenerApp
{
    public class ChatServer
    {
        int port;
        IPAddress address;
        TcpListener listener;

        List<ChatClient> clients;

        public ChatServer()
        {
            port = 5000;
            address = IPAddress.Any;
            listener = new(address, port);
            clients = new List<ChatClient>();
        }

        public async Task ListenAsync()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Server started. Waiting connnections...");
                string hostName = Dns.GetHostName();
                Console.WriteLine($"Server Host Name: {hostName}");
                IPAddress[] addresses = Dns.GetHostAddresses(hostName, AddressFamily.InterNetwork);
                Console.WriteLine($"Server IPv4 Address: {addresses[0]}");

                while(true)
                {
                    TcpClient remoteClient = await listener.AcceptTcpClientAsync();

                    ChatClient client = new(this, remoteClient);

                    clients.Add(client);

                    Task.Run(client.ProcessAsync);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DisconnectAll();
            }
        }

        public void ClientRemove(string id)
        {
            ChatClient client = clients.FirstOrDefault(c => c.Id == id);
            if(client is not null)
                clients.Remove(client);
            client?.Close();
        }
        public void DisconnectAll()
        {
            foreach (ChatClient client in clients)
                client.Close();
            listener.Stop();
        }

        public async Task BroadcastMessageAsync(string id, string message)
        {
            foreach(ChatClient client in clients)
                if(client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
        }
    }
}
