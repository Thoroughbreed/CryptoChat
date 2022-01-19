using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Web_Server;
using Web_Server.Classes;

namespace Web_Server.Services
{
    public partial class ChatRoom
    {
        // private ConcurrentDictionary<string, IServerStreamWriter<Message>> _users = new();
        private List<User> _users = new();

        public void Join(string name, IServerStreamWriter<Message> response, string room, string guid)
        {
            if (_users.Find(u => u.Guid == Guid.Parse(guid)) == null)
            {
                _users.Add(new User(name, response, room, guid));
            }
            // _users.TryAdd(name, response);
        }

        // public void Remove(string name) => _users.TryRemove(name, out var s);
        public void Remove(Guid guid) => _users.Remove(_users.FirstOrDefault(u => u.Guid == guid));

        public async Task BroadcastMessageAsync(Message message) => await BroadcastMessages(message);

        private async Task BroadcastMessages(Message message)
        {
            if (message.Text == "ls")
            {
                await GetList(message);
                return;
            }
            else if (message.Text.StartsWith("mv"))
            {
                if (message.Text.Length == 2)
                {
                    await ChangeRoom(message);
                }
                else
                {
                    await RenameUser(message);
                }
                return;
            }
            else if (message.Text == ":q!")
            {
                Remove(Guid.Parse(message.Guid));
                message.Text = $"{message.User} just left the room ...";
            }

            Guid _guid;
            if (!Guid.TryParse(message.Guid, out _guid))
            {
                foreach (var user in _users)
                {
                    User _user = null;
                    try
                    {
                        _user = await SendMessageToSubscriber(user, message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Remove(_user.Guid);
                    }

                    if (_user != null)
                    {
                        Remove(_user.Guid);
                    }
                }
            }

            foreach (var user in _users.Where(x => x.Guid != Guid.Parse(message.Guid)))
            {
                if (message.Room == user.Room)
                {
                    User _user = null;
                    try
                    {
                        _user = await SendMessageToSubscriber(user, message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Remove(_user.Guid);
                        break;
                    }

                    if (_user != null)
                    {
                        Remove(_user.Guid);
                        break;
                    }
                }
            }
        }

        private async Task RenameUser(Message message)
        {
            var _tempName = message.User;
            var _user = _users.FirstOrDefault(x => x.Guid == Guid.Parse(message.Guid));
            _user.Name = message.Text.Substring(2);
            await SendMessageToSubscriber(_user,
                new Message { Text = $"Your username is now {_user.Name} - it was changed from {_tempName}" });
            var _userList = _users.Where(u => u.Room == message.Room);
            foreach (var user in _userList)
            {
                await SendMessageToSubscriber(user,
                    new Message { Text = $"{_tempName} is henceforth known as {_user.Name}" });
            }
        }

        private async Task ChangeRoom(Message message)
        {
            var _user = _users.FirstOrDefault(u => u.Guid == Guid.Parse(message.Guid));
            var _tempRoom = _user.Room;
            _user.Room = message.Room;
            await SendMessageToSubscriber(_user,
                new Message { Text = $"Your room is now {_user.Room} - it was changed from {_tempRoom}" });
        }
        
        private async Task GetList(Message message)
        {
            var _user = _users.FirstOrDefault(x => x.Guid == Guid.Parse(message.Guid));
            int i = 0;
            foreach (var user in _users)
            {
                if (user.Room == message.Room)
                {
                    i++;
                    await SendMessageToSubscriber(_user, new Message { Text = $"User number {i} - {user.Name}" });
                }
            }
        }

        private async Task<User> SendMessageToSubscriber(User user, Message message)
        {
            try
            {
                await user.Message.WriteAsync(message);
                // await user.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Current users are: ");
                foreach (var _user in _users)
                {
                    Console.WriteLine(_user.Name);
                }

                Console.WriteLine("Removing user " + user.Name);
                return user;
            }
        }
    }
}