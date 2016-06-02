var debug = require('debug')('app:lobby');
var packet = require('../packet');

var type = packet.type;
var ids = [];
var table = {};

module.exports.onMessage = function (wss, socket) {

  function register(message) {
    var objId = message.split('?')[0];
    if (!(objId in table[socket.shortid])) {
      table[socket.shortid][objId] = message;
    }
  }

  function join() {
    ids.forEach(function (id) {
      for (var objId in table[id])
        if (table[id].hasOwnProperty(objId)) {
          var packet = {
            type: type.command,
            message: table[id][objId]
          };
          socket.send(JSON.stringify(packet));
        }
    });
    ids.push(socket.shortid);
    table[socket.shortid] = {};
  }

  function broadcast(message) {
    var packet = {
      type: type.command,
      message: message
    };
    wss.broadcast(socket, JSON.stringify(packet));
    register(message);
    debug('broadcast command');
  }

  return function (message) {
    var packet = JSON.parse(message);

    if (packet.type === type.join)
      join();

    if (packet.type === type.command)
      broadcast(packet.message);
  };
};

module.exports.onClose = function (socket) {
  return function () {
    var index = ids.indexOf(socket.shortid);
    if (index !== -1) {
      ids.splice(index, 1);
    }
    delete table[socket.shortid];
  };
};
