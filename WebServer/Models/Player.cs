using System;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace WebServer.Models
{

    public class Player
    {

        [JsonIgnore]
        public WebSocket Socket { get; set; }

        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

    }

}