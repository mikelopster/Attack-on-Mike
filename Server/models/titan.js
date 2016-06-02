var mongoose = require('mongoose');

var titanSchema = mongoose.Schema({
  name: String,
  height: Number,
  score: Number
});

var Titan = mongoose.model('Titan', titanSchema);

mongoose.connect('mongodb://localhost/titals');

module.exports = Titan;
