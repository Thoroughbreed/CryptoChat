// See https://aka.ms/new-console-template for more information

using System;
using Grpc.Net.Client;
using Web_Server;

namespace Console_Client
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello, World!");
            using var channel = GrpcChannel.ForAddress("https://127.0.0.1:5000");
            var greeter = new Greeter.GreeterClient(channel);
        }
    }
}