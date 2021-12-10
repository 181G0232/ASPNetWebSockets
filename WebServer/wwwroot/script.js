var webSocket = null;
var player = null;
var players = [];

function update() {
    var canvas = document.getElementById("canvas");
    var ctX = canvas.getContext('2d');
    ctX.clearRect(0, 0, 500, 500);
    if (player != null) {
        player.draw();
    }
    players.forEach(x => {
        x.draw();
    });
}

document.addEventListener("keypress", (event) => {
    var name = event.key;
    var code = event.code;

    if (player != null) {
        if (name == "a") {
            player.X -= 10;
        }
        else if (name == "d") {
            player.X += 10;
        }
        else if (name == "w") {
            player.Y -= 10;
        }
        else if (name == "s") {
            player.Y += 10;
        }
        update();
        webSocket.send(JSON.stringify(player));
    }

}, false);

function join() {
    var name = document.getElementById("name").value
    var url = "wss://localhost:7244/Join?name=" + name;
    webSocket = new WebSocket(url);
    //
    player = NewRect();
    player.Name = name;
    update();
    //
    webSocket.onmessage = function (message) {
        var m = message.data;

        var d = null;
        players.forEach(X => {
            if (X.Name == m) {
                d = X;
            }
        });
        if (d != null) {
            var copy = [];
            players.forEach(x => {
                if (x != d) {
                    copy.push(x);
                }
            });
            players = copy;
        } else {
            var p = JSON.parse(m);

            var found = false;
            players.forEach(X => {
                if (X.Name == p.Name) {
                    X.X = p.X;
                    X.Y = p.Y;
                    found = true;
                }
            });
            if (found == false) {
                var n = NewRect();
                n.Name = p.Name;
                n.X = p.X;
                n.Y = p.Y;
                players.push(n);
            }
        }
        update();

    };
    webSocket.send(JSON.stringify(player));
}

function NewRect() {
    return {
        Name: null,
        X: 0.0,
        Y: 0.0,
        draw: function () {
            var canvas = document.getElementById("canvas");
            var ctX = canvas.getContext('2d');
            ctX.fillRect(this.X, this.Y, 100, 100);
            ctX.clearRect(this.X + 20, this.Y + 20, 60, 60);
            ctX.strokeRect(this.X + 25, this.Y + 25, 50, 50);
        }
    }
}
