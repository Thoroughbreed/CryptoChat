using System;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chat;
using Console_Client.Crypto;
using Console_Client.Utils;
using Grpc.Core;
using Channel = Grpc.Core.Channel;

namespace Console_Client
{
    public class Program
    {
        private static CryptoClass _crypto = new();
        private static ObscureText _hide = new();

        static async Task Main(string[] args)
        {
            Guid guid = Guid.NewGuid();
            string conn = ChangeSettings();
            string user = UserInformation();
            string room = SelectRoom();
            
            var channel = new Channel(conn, ChannelCredentials.Insecure);
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
                    User = user, Text = $"{user} has joined the room", Room = room, Guid = guid.ToString()
                });

                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToLower() == ":q!")
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = user, Text = line, Room = room, Guid = guid.ToString() });
                        break;
                    }

                    if (line.ToLower() == "ls")
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = user, Text = line, Room = room, Guid = guid.ToString() });
                    }
                    else if (line.StartsWith("mv"))
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = user, Text = line, Room = room, Guid = guid.ToString() });
                        user = line.Substring(2);
                    }
                    else
                    {
                        await chat.RequestStream.WriteAsync(new Message
                        {
                            User = user, Text = _crypto.Encrypt(line), Room = room, Guid = guid.ToString()
                        });
                    }
                }

                await chat.RequestStream.CompleteAsync();
            }

            Console.WriteLine("Disconnecting");
            await channel.ShutdownAsync();
            Console.WriteLine("Thank you ...");
            ASCII.DOOM();
        }

        private static string ChangeSettings()
        {
            bool _bool = false;
            string? input = "";
            string ip = "";
            string port = "";
            
            do
            {
                Console.Write("Please enter port number: ");
                input = Console.ReadLine();
                _bool = int.TryParse(input, out var test);
                if (_bool && (test !< 65536 || test !> 0))
                {
                    _bool = false;
                }
                else
                {
                    Console.WriteLine("Out of range, please input a number between 1 and 65535");
                }
            } while (!_bool);
            port = input.Length > 0 ? input : "5000";

            do
            {
                Console.Write("Please enter IP of the server: ");
                input = Console.ReadLine();
                _bool = IPAddress.TryParse(input, out var test);
                if (!_bool)
                {
                    Console.WriteLine("Please try again, input format was wrong. it should be xxx.xxx.xxx.xxx");
                }
            } while (!_bool);
            ip = input.Length > 0 ? input : "127.0.0.1";
            
            return ip + ":" + port;
        }

        static string UserInformation()
        {
            Console.Write("Please type in your username: ");
            string user = Console.ReadLine();
            user = user.Length > 0 ? user : "Default user";
            Console.Write("Please input the shared secret: ");
            _crypto.SetUnlockKey(_hide.HideInput());
            return user;
        }

        private static string SelectRoom()
        {
            // TODO
            return "1";
        }
    }
}