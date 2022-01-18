using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Web_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton(typeof(ChatRoom));
// builder.Services.AddSingleton(Server.ChatRoom);
builder.WebHost.UseKestrel(
    options =>
    {
        options.Listen(IPAddress.Any, 5000, o => o.Protocols = HttpProtocols.Http2 );
    });

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<ChatService>();
// app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();