var debug = require('debug')('app:client');
var packet = require('../packet');

var type = packet.type;

module.exports.onMessage = function (wss, socket) {

  function join(message) {
    var packet = {
      type: type.join,
      message: socket.shortid + '|' + message
    };
    socket.send(JSON.stringify(packet));
    debug('joined');
  }

  return function (message) {
    var packet = JSON.parse(message);

    if (packet.type === type.join) {
      join(packet.message);
      socket.shortname = packet.message;
    }
  };
};
