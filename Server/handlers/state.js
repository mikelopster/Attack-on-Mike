var debug = require('debug')('app:state');
var packet = require('../packet');

var type = packet.type;
var ids = [];
var table = {};

module.exports.onMessage = function (wss, socket) {

  function join() {
    ids.push(socket.shortid);
  }

  function sync(message) {
    ids.forEach(function (id) {
      if (id !== socket.shortid) {
        var packet = {
          type: type.synchronize,
          message: id + '|' + table[id]
        };
        socket.send(JSON.stringify(packet));
      }
    });
    table[socket.shortid] = message;
    debug('synchronize');
  }

  return function (message) {
    var packet = JSON.parse(message);

    if (packet.type === type.join)
      join();

    if (packet.type === type.synchronize)
      sync(packet.message);
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
