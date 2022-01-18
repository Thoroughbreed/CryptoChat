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
        private ConcurrentDictionary<string, IServerStreamWriter<Message>> _users = new();

        public void Join(string name, IServerStreamWriter<Message> response)
        {
            _users.TryAdd(name, response);
        }

        public void Remove(string name) => _users.TryRemove(name, out var s);

        public async Task BroadcastMessageAsync(Message message) => await BroadcastMessages(message);

        private async Task BroadcastMessages(Message message)
        {
            if (message.Text == "/list")
            {
                await GetList(message);
                return;
            }

            foreach (var user in _users.Where(x => x.Key != message.User))
            {
                KeyValuePair<string, IServerStreamWriter<Message>>? item = null;
                try
                {
                    item = await SendMessageToSubscriber(user, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Remove(item?.Key);
                }

                if (item != null)
                {
                    Remove(item?.Key);
                }

                ;
            }
        }

        private async Task GetList(Message message)
        {
            var _user = _users.FirstOrDefault(x => x.Key == message.User);
            int i = 0;
            foreach (var user in _users)
            {
                i++;
                await SendMessageToSubscriber(_user, new Message { Text = $"User number {i} - {user.Key}" });
            }
        }

        private async Task<KeyValuePair<string, IServerStreamWriter<Message>>?> SendMessageToSubscriber(
            KeyValuePair<string, IServerStreamWriter<Message>> user, Message message)
        {
            try
            {
                await user.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Current users are: ");
                foreach (var _user in _users)
                {
                    Console.WriteLine(_user.Key);
                }

                Console.WriteLine("Removing user " + user.Key);
                return user;
            }
        }
    }
}