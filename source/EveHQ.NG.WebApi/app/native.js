// It's a hack to solve a problem with angular-cli when fs (and other node.js modules) isn't available in the renderer
// because of optimizing them out by angular-cli.
fs = require('fs');
os = require('os');
