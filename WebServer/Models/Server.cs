using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebServer.Models
{

    public class Server
    {

        public List<Player> Players { get; set; } = new();

        private object monitor = new();

        public bool JoinPlayer(Player player)
        {
            if (Players.Any(x => x.Name == player.Name))
            {
                return false;
            }
            Players.Add(player);
            return true;
        }

        public async Task OnPlaying(Player player)
        {
            var buffer = new byte[1024];
            foreach (var p in Players.ToList())
            {
                if (p != player && p.Socket.State == WebSocketState.Open)
                {
                    var json = JsonSerializer.Serialize(p);
                    var data = Encoding.UTF8.GetBytes(json);
                    await player.Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                    //
                    json = JsonSerializer.Serialize(player);
                    data = Encoding.UTF8.GetBytes(json);
                    await p.Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            //
            while (player.Socket.State == WebSocketState.Open)
            {
                var result = await player.Socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var state = JsonSerializer.Deserialize<Player>(json);
                    player.X = state.X;
                    player.Y = state.Y;
                    //
                    json = JsonSerializer.Serialize(player);
                    var data = Encoding.UTF8.GetBytes(json);
                    foreach (var p in Players.ToList())
                    {
                        if (p != player && p.Socket.State == WebSocketState.Open)
                        {
                            await p.Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            //
            var name = Encoding.UTF8.GetBytes(player.Name);
            foreach (var p in Players.ToList())
            {
                if (p != player && p.Socket.State == WebSocketState.Open)
                {
                    await p.Socket.SendAsync(name, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            Players.Remove(player);
        }

    }

}