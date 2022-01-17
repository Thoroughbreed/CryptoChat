using System;

namespace Web_Server.Classes
{
    public class User
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }

        public User(string name)
        {
            Name = name;
            Guid = Guid.NewGuid();
        }
    }
}