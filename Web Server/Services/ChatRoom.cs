using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Web_Server.Classes;

namespace Web_Server.Services
{
    public partial class ChatRoom
    {
        private List<User?> _users = new();

        public void Join(string name, IServerStreamWriter<Message> response, string room, string guid)
        {
            if (_users.Find(u => u.Guid == Guid.Parse(guid)) == null)
            {
                _users.Add(new User(name, response, room, guid));
            }
        }

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
                    User? foundUser = null;
                    try
                    {
                        foundUser = await SendMessageToSubscriber(user, message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (foundUser != null) Remove(foundUser.Guid);
                    }

                    if (foundUser != null) Remove(foundUser.Guid);
                }
            }

            foreach (var user in _users.Where(x => x.Guid != Guid.Parse(message.Guid)))
            {
                if (message.Room == user?.Room)
                {
                    User? foundUser = null;
                    try
                    {
                        foundUser = await SendMessageToSubscriber(user, message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (foundUser != null) Remove(foundUser.Guid);
                    }

                    if (foundUser != null) Remove(foundUser.Guid);
                }
            }
        }

        private async Task RenameUser(Message message)
        {
            var foundUser = _users.FirstOrDefault(x => x.Guid == Guid.Parse(message.Guid));
            var tempName = foundUser?.Name;
            if (foundUser != null)
            {
                foundUser.Name = message.Text.Substring(3);
                await SendMessageToSubscriber(foundUser,
                    new Message { Text = $"Your username is now {foundUser.Name} - it was changed from {tempName}" });
                var userList = _users.Where(u => u.Room == message.Room);
                foreach (var user in userList.Where(u => u != foundUser))
                {
                    await SendMessageToSubscriber(user,
                        new Message { Text = $"{tempName} is henceforth known as {foundUser.Name}" });
                }
            }
        }

        private async Task ChangeRoom(Message message)
        {
            var foundUser = _users.FirstOrDefault(u => u.Guid == Guid.Parse(message.Guid));
            if (foundUser != null)
            {
                var tempRoom = foundUser.Room;
                foundUser.Room = message.Room;
                await SendMessageToSubscriber(foundUser,
                    new Message { Text = $"Your room is now {foundUser.Room} - it was changed from {tempRoom}" });

                var userList = _users.Where(u => u.Room == message.Room);
                foreach (var user in userList.Where(u => u != foundUser))
                {
                    await SendMessageToSubscriber(user,
                        new Message { Text = $"{foundUser.Name} entered the sphere!" });
                }
            }
        }

        private async Task GetList(Message message)
        {
            var foundUser = _users.FirstOrDefault(x => x.Guid == Guid.Parse(message.Guid));
            int i = 0;
            foreach (var user in _users.Where(user => user?.Room == message.Room))
            {
                i++;
                await SendMessageToSubscriber(foundUser, new Message { Text = $"User number {i} - {user?.Name}" });
            }
        }

        private async Task<User?> SendMessageToSubscriber(User? user, Message message)
        {
            try
            {
                await user.Message.WriteAsync(message);
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