using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models;

namespace WebServer.Controllers
{

    public class HomeController : Controller
    {

        [Route("/")]                // Rutas para la pagina principal
        [Route("/Home")]
        [Route("/Home/Index")]
        public IActionResult Index()
        {
            return View();  // Regresamos la pagina princpal
        }

        public Server Server { get; }

        public HomeController(Server server)
        {
            Server = server;    // Obtenemos nuestro servidor de los servicios
        }

        [Route("/Join")]
        public async Task Join(string name)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest && !string.IsNullOrWhiteSpace(name))
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Player player = new()
                {
                    Name = name,
                    Socket = socket
                };
                if (Server.JoinPlayer(player))
                {
                    await Server.OnPlaying(player);
                }
            }
        }

    }

}