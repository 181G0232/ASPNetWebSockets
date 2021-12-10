using System.Net;
using System.Net.WebSockets;
using WebServer.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();                  // Habilitar el servicio del modelo MVC
builder.Services.AddSingleton<Server>();    // Habilitamos nuestro servidor como un servicio permanente

var app = builder.Build();

WebSocketOptions options = new();
options.KeepAliveInterval = TimeSpan.FromDays(1);

app.UseFileServer();
app.UseWebSockets(options);         // Habilitar el uso de websockets
app.MapDefaultControllerRoute();    // Habilitar el mapeo de contrladores MVC

app.Run();
