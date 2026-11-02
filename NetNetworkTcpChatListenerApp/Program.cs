using NetNetworkTcpChatListenerApp;

ChatServer server = new();
await server.ListenAsync();