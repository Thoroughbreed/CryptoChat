using System;
using Grpc.Core;
using Web_Server.Services;

namespace Web_Server.Classes
{
    public class User
    {
        public string Name { get; set; }
        public string Secret { get; set; }
        public IServerStreamWriter<Message> Message { get; set; }
        public Guid Guid { get; set; }

        public User(string name, IServerStreamWriter<Message> message, string secret, string guid)
        {
            Name = name;
            Message = message;
            Secret = secret;
            Guid = Guid.Parse(guid);
        }
    }
}