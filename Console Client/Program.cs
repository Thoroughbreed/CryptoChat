using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chat;
using Console_Client.Crypto;
using Grpc.Core;
using Channel = Grpc.Core.Channel;

namespace Console_Client
{
    public class Program
    {
        private static string _room = "";
        private static Guid _guid;
        private static CryptoClass _crypto = new();

        static async Task Main(string[] args)
        {
            _guid = Guid.NewGuid();
            Console.Write("Welcome! Please type in your username: ");
            var userName = Console.ReadLine();
            // Include port of the gRPC server as an application argument
            var port = args.Length > 0 ? args[0] : "5000";
            Console.Write("Please type in IP of the server as xxx.xxx.xxx.xxx: ");
            var input = Console.ReadLine();
            var ip = input?.Length > 0 ? input : "127.0.0.1";
            Console.Write("Please enter your secret: ");
            _crypto.SetUnlockKey(Console.ReadLine());
            Console.WriteLine("Please select chatroom:");
            Console.WriteLine("[1] Room 1\n[2] Room 2\n[3] Room 3\n[4] Room 4\n");
            _room = Console.ReadLine();
            var channel = new Channel(ip + ":" + port, ChannelCredentials.Insecure);
            var client = new ChatRoom.ChatRoomClient(channel);

            using (var chat = client.join())
            {
                _ = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                    {
                        var response = chat.ResponseStream.Current;
                        var _output = response.Text;
                        try
                        {
                            _output = _crypto.Decrypt(Convert.FromHexString(response.Text));
                        }
                        catch (Exception e)
                        {
                            _output = response.Text;
                        }
                        Console.WriteLine($"{response.User}: {_output}");
                    }
                });

                await chat.RequestStream.WriteAsync(new Message
                {
                    User = userName, Text = $"{userName} has joined the room", Room = _room, Guid = _guid.ToString()
                });

                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToLower() == ":q!")
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = userName, Text = line, Room = _room, Guid = _guid.ToString() });
                        break;
                    }
                    if (line.ToLower() == "ls")
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = userName, Text = line, Room = _room, Guid = _guid.ToString() });
                    }
                    else if (line.StartsWith("mv"))
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = userName, Text = line, Room = _room, Guid = _guid.ToString() });
                        userName = line.Substring(2);
                    }
                    else
                    {
                        await chat.RequestStream.WriteAsync(new Message
                        {
                            User = userName, Text = _crypto.Encrypt(line), Room = _room, Guid = _guid.ToString()
                        });
                    }
                }

                await chat.RequestStream.CompleteAsync();
            }

            Console.WriteLine("Disconnecting");
            await channel.ShutdownAsync();
        }
    }
}