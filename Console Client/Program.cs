using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
        private static bool _isKeySet = false;

        static async Task Main(string[] args)
        {
            Guid guid = Guid.NewGuid();
            string conn = "127.0.0.1:5000";
            string? user = "Default User";
            string room = "1";
            bool mainMenuActive = true;
            
            ASCII mainMenu = new (new List<string>
            {
                "Change Settings",
                "Enter user info (and shared secret)",
                "Select room",
                "Chat!",
                "-- EXIT --"
            });
            do
            {
                switch (mainMenu.Draw())
                {
                    case 0:
                        conn = ChangeSettings();
                        break;
                    case 1:
                        user = UserInformation();
                        break;
                    case 2:
                        room = SelectRoom();
                        break;
                    case 3:
                        mainMenuActive = false;
                        break;
                    case 4:
                        Console.WriteLine();
                        Environment.Exit(0);
                        break;
                    case 9: // Quit the menu if user presses either X or ESC
                        Console.WriteLine();
                        Environment.Exit(0);
                        break;
                }
            } while (mainMenuActive);
            Console.WriteLine();
            Console.CursorVisible = true;
            if (!_isKeySet)
            {
                Console.Write("Please input the shared secret: ");
                _isKeySet = _crypto.SetUnlockKey(_hide.HideInput());
            }
            var channel = new Channel(conn, ChannelCredentials.Insecure);
            var client = new ChatRoom.ChatRoomClient(channel);

            using (var chat = client.join())
            {
                #region Listen for messages (async)
                _ = Task.Run(async () =>
                {
                    while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                    {
                        var response = chat.ResponseStream.Current;
                        var _output = response.Text;
                        try
                        { _output = _crypto.Decrypt(Convert.FromHexString(response.Text)); }
                        catch (Exception)
                        { _output = response.Text; }

                        Console.WriteLine($"{response.User}: {_output}");
                    }
                });
                #endregion

                #region Write messages
                
                // User joins the server and handles the server it's name, room and guid
                await chat.RequestStream.WriteAsync(new Message
                {
                    User = user, Text = $"{user} has joined the room", Room = room, Guid = guid.ToString()
                });

                string? line; // prepares for input
                while ((line = Console.ReadLine()) != null)
                {
                    if (line.ToLower() == ":q!") // Quit dammit!
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = user, Text = line, Room = room, Guid = guid.ToString() });
                        break;
                    }

                    if (line.ToLower() == "ls") // List users in the room
                    {
                        await chat.RequestStream.WriteAsync(new Message
                            { User = user, Text = line, Room = room, Guid = guid.ToString() });
                    }
                    else if (line.StartsWith("mv")) // Rename username or move to room
                    {
                        if (line.Length == 2)
                        {
                            room = SelectRoom();
                        }
                        else
                        {
                            user = line.Substring(3);
                        }
                        await chat.RequestStream.WriteAsync(new Message
                            { User = user, Text = line, Room = room, Guid = guid.ToString() });
                    }
                    else if (line.StartsWith("pwd")) // changes the shared secret/encryption key
                    {
                        if (line.Length == 3)
                        {
                            Console.Write("Enter the new encryption key: ");
                            _crypto.SetUnlockKey(_hide.HideInput());
                        }
                        else
                        {
                            _crypto.SetUnlockKey(line.Substring(4));
                        }
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
                #endregion
            }

            Console.WriteLine("Disconnecting");
            await channel.ShutdownAsync();
            Console.WriteLine("Thank you ...");
            ASCII.DOOM(); // Doom :P
        }

        private static string ChangeSettings()
        {
            Console.Clear();
            bool _bool;
            string? input;
            
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
            string ip = input ?? "127.0.0.1";

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
            } while (_bool);
            string port = input ?? "5000";
            
            return ip + ":" + port;
        }

        static string? UserInformation()
        {
            Console.Clear();
            Console.Write("Please type in your username: ");
            string? user = Console.ReadLine();
            user ??= "Default user"; // Compound assignment (Is user not null, if it is then =)
            Console.Write("Please input the shared secret: ");
            _isKeySet = _crypto.SetUnlockKey(_hide.HideInput());
            return user;
        }

        private static string SelectRoom()
        {
            do
            {
                ASCII subMenu = new(new List<string>()
                {
                    "Room 1",
                    "Room 2",
                    "Room 3",
                    "Room 4"
                });
                switch (subMenu.Draw(true))
                {
                case 0:
                    Console.WriteLine();
                    Console.CursorVisible = true;
                    return "1";
                case 1:
                    Console.WriteLine();
                    Console.CursorVisible = true;
                    return "2";
                case 2:
                    Console.WriteLine();
                    Console.CursorVisible = true;
                    return "3";
                case 3:
                    Console.WriteLine();
                    Console.CursorVisible = true;
                    return "4";
                }
            } while (true);
        }
    }
}