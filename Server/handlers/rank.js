var debug = require('debug')('app:lobby');
var packet = require('../packet');
var Titan = require('../models/titan');

var type = packet.type;
var list = {};
var count = 5;

module.exports.onMessage = function (wss, socket) {

  function sync(message) {
    // Unpack message and calculate high score store in the listen

    var pack = JSON.parse(message);
    var data = JSON.parse(pack.message);
    var result = "";

    if(data.keys) {
      var levelIndex = data.keys.indexOf("level");
      result = data.values[levelIndex];
    }



    if(result != "") {
      if(typeof list[socket.shortid] == 'undefined')
        list[socket.shortid] = result;
      else {
        var previousValue = list[socket.shortid];
        var currentValue = result;

        if(currentValue > previousValue)
          list[socket.shortid] = currentValue;

      }
    }

    var sortable = [];

    for (var l in list) {
      if(typeof list[l] != 'undefined')
        sortable.push([l, list[l]]);
      else {
        list[l] = "10000";
        sortable.push([l, list[l]]);
      }
    }

    sortable.sort(function(a, b) {return b[1] - a[1]});

    var packet = {
      type: type.command,
      message: "none?highscore?" + sortable.slice(0, count).join(':').toString()
    };

    console.log(packet);

    socket.send(JSON.stringify(packet));
  }

  return function (message) {
    var packet = JSON.parse(message);

    if (packet.type === type.synchronize)
      sync(message);

  };
};

module.exports.onClose = function (socket) {
  return function () {
    if(typeof list[socket.shortid] != 'undefined') {
      var name = socket.shortname;
      var level = list[socket.shortid];
      console.log(name + "-" + level);

      var newTitan = new Titan ({
         name: name,
         score: level
       });

       newTitan.save(function (err) {
         if (err)
          console.log ('Error on save!');
        else {
          console.log('Add Completed!');
        }
       });

      delete typeof list[socket.shortid];

    }
  };
};
