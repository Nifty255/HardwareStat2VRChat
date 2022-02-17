const osc = require("osc");

let udpPort = null;

const init = ({
  addr,
  port,
  onReady,
  onError
}) => {
  
  udpPort = new osc.UDPPort({
    localAddress: "localhost",
    localPort: port+1,
    remoteAddress: addr,
    remotePort: port,
    broadcast: true
  });
  
  udpPort.on("ready", onReady);
  
  udpPort.on("error", onError);
  
  udpPort.open();
};

const sendFloat = ({
  path,
  data
}) => {
  if (udpPort == null) {
    throw new Error("OSC client not initialized!");
  }

  logger.debug(`Sending to path "${path}": ${data}`);

  udpPort.send({
    address: path,
    args: [
      {
        type: "f",
        value: data
      }
    ]
  });
}

module.exports = {
  init,
  sendFloat
}