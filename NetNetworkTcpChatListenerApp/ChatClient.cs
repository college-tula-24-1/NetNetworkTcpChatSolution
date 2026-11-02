using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetNetworkTcpChatListenerApp
{
    public class ChatClient
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }

        Dictionary<string, ConsoleColor> colors = new()
        {
            { "black", ConsoleColor.Black },
            { "blue", ConsoleColor.Blue },
            { "green", ConsoleColor.Green },
            { "cyan", ConsoleColor.Cyan },
            { "red", ConsoleColor.Red },
            { "magenta", ConsoleColor.Magenta },
            { "yellow", ConsoleColor.Yellow }
        };

        TcpClient client;
        ChatServer server;

        public ChatClient(ChatServer server, TcpClient remoteClient)
        {
            this.server = server;
            this.client = remoteClient;

            var stream = client.GetStream();
            Writer = new StreamWriter(stream);
            Reader = new StreamReader(stream);
        }

        public async Task ProcessAsync()
        {
            string? message;
            string name = "Anonim";
            ConsoleColor color = ConsoleColor.White;
            string command;

            try
            {
                while(true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();

                        if (message is null) continue;

                        if (message.Trim()[0] == '-')
                        {
                            command = message.Substring(1, message.IndexOf(' ') - 1);
                            if(command == "name")
                            {
                                name = message.Substring(message.IndexOf(' ') + 1).Trim();
                                await server.BroadcastMessageAsync(Id, $"{name} LogIn To Chat!");
                                Console.ForegroundColor = color;
                                Console.WriteLine($"{name} LogIn To Chat!");
                            }
                            if(command == "color")
                            {
                                string colorName = message.Substring(message.IndexOf(' ') + 1).Trim();
                                color = colors[colorName];
                            }
                        }

                        message = $"{name}: {message}";
                        await server.BroadcastMessageAsync(Id, message);
                        Console.ForegroundColor = color;
                        Console.WriteLine(message);
                    }
                    catch
                    {
                        message = $"{name} LogOut From Chat";

                        await server.BroadcastMessageAsync(Id, message);
                        Console.ForegroundColor = color;
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.ClientRemove(Id);
            }
        }

        public void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
