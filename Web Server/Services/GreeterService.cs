using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Web_Server;

namespace Web_Server.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name + " and welcome to the system.\nYou are now in chat-mode."
        });
    }

    public override Task<HelloReply> StartTalking(HelloRequest request, ServerCallContext context)
    {
        Console.Write(request.Name + ": ");
        Console.WriteLine(request.Message);
        return Task.FromResult(new HelloReply { Message = "Message received" });
    }
}