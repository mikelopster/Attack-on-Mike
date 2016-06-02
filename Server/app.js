var http = require('http');
var ws = require('ws');
var shortid = require('shortid');
var debug = require('debug')('app');

var client = require('./handlers/client');
var lobby = require('./handlers/lobby');
var state = require('./handlers/state');
var rank = require('./handlers/rank');

var host = process.env.HOST || 'localhost';
var port = process.env.PORT || '3000';
var server = http.createServer();
var wss = new ws.Server({ server: server });

wss.broadcast = function broadcast(socket, message) {
  wss.clients.forEach(function (client) {
    if (client !== socket)
      client.send(message);
  });
};

wss.on('connection', function (socket) {

  var clientOnMessage = client.onMessage(wss, socket);
  var lobbyOnMessage = lobby.onMessage(wss, socket);
  var stateOnMessage = state.onMessage(wss, socket);
  var rankOnMessage = rank.onMessage(wss, socket);

  var lobbyOnClose = lobby.onClose(socket);
  var stateOnClose = state.onClose(socket);
  var rankOnClose = rank.onClose(socket);

  socket.on('message', function (message) {
    rankOnMessage(message);
    clientOnMessage(message);
    lobbyOnMessage(message);
    stateOnMessage(message);
  });

  socket.on('close', function () {
    lobbyOnClose();
    stateOnClose();
    rankOnClose();
    debug('socket disconnect');
  });

  socket.shortid = shortid.generate();
  debug('new socket connected');

});

server.listen(port, host);
debug('listening on %s %s', host, port);
