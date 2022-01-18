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
        private static string _secret = "";
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
            _secret = _crypto.SetUnlockKey(Console.ReadLine());
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
                            var _cipher = Convert.FromHexString(response.Text);
                            _output = _crypto.Decrypt(_cipher);
                        }
                        finally
                        {
                            Console.WriteLine($"{response.User}: {_output}");
                        }
                    }
                });

                await chat.RequestStream.WriteAsync(new Message
                {
                    User = userName, Text = $"{userName} has joined the room", Secret = _secret, Guid = _guid.ToString()
                });

                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToLower() == ":q!")
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = userName, Text = line, Secret = _secret, Guid = _guid.ToString() });
                        break;
                    }

                    if (line.ToLower() == "/list")
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = userName, Text = line, Secret = _secret, Guid = _guid.ToString() });
                    }
                    else
                    {
                        await chat.RequestStream.WriteAsync(new Message
                        {
                            User = userName, Text = _crypto.Encrypt(line), Secret = _secret, Guid = _guid.ToString()
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