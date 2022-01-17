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
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private static List<User> _users = new();

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _users.Add(new User(request.Name));
            users.TryAdd(request.Name, response)
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + " and welcome to the system.\nYou are now in chat-mode.",
                Guid = _users.Find(u => u.Name == request.Name).Guid.ToString()
            });
        }

        private ConcurrentDictionary<string, IServerStreamWriter<ChatMessage>> users = new();
        public async Task<HelloReply> StartTalking(ChatMessage request, ServerCallContext context)
        {
            foreach (var user in users.Where(u => u.Key != request.Guid))
            {
                var item = await SendMessageToSubscriber(user, message);
                if (item != null)
                {
                    Remove(item?.Key);
                };
            }
        }
        private async Task<Nullable<KeyValuePair<string, IServerStreamWriter<ChatMessage>>>> SendMessageToSubscriber(KeyValuePair<string, IServerStreamWriter<ChatMessage>> user, ChatMessage message)
        {
            try
            {
                await user.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return user;
            }
        }
    }
}