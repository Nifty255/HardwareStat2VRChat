require('dotenv').config()

const winston = require("winston");
const osc = require("./osc");

const { pollCpu, pollMem } = require("./hardware");

const addr      = process.env.ADDR              || "localhost";
const port      = parseInt(process.env.PORT     || "9000");
const interval  = parseInt(process.env.INTERVAL || "1000");
const logLevel  = parseInt(process.env.LOG      || "4");

const getLogLevelName = () => {
  for (const [name, num] of Object.entries(winston.config.syslog.levels)) {
    if (num == logLevel) {
      return name;
    }
  }

  throw new Error(`Invalid log level ${logLevel}`);
}
const basicFormat = winston.format.printf(({ level, message, timestamp }) => {
  return `${timestamp} [${level}] ${message}`;
});
global.logger = winston.createLogger({
  levels: winston.config.syslog.levels,
  level: getLogLevelName(),
  format: winston.format.combine(
    winston.format.colorize(),
    winston.format.timestamp(),
    basicFormat
  ),
  transports: [new winston.transports.Console()]
});

logger.alert("Starting up!!! Press Ctrl+C to exit.");

if (interval < 1000) {
  if (interval < 250) {
    logger.fatal(`Great Scott!!! Interval of ${interval}ms is FAR too short to be useful and won't update that quickly to others in VRChat anyway.`);
    process.exit();
  }
  logger.warn(`Interval of ${interval}ms is less than 1 second. This could get heavy, Doc. Hardware stats may be jumpy, and others might not see your avatar params as smoothly as you think.`);
}

let ticker = null;

osc.init({
  addr,
  port,
  onReady: () => {
    logger.alert(`OSC Client ready to toss data to "${addr}:${port}"`);
    ticker = setInterval(() => {
      logger.info("So anyways, I started blastin'...");
      let coreStats = pollCpu();
      let memStats = pollMem();
  
      for (const coreNum in coreStats) {
        osc.sendFloat({
          path: `/avatar/parameters/core${coreNum}`,
          data: Math.round(coreStats[coreNum] * 100) / 100
        });
      }

      osc.sendFloat({
        path: "/avatar/parameters/mem",
        data: Math.round(memStats * 100) / 100
      });
    }, interval);
  },
  onError: (err) => {
    logger.fatal(`The OSC driver experienced an error! ${err}`);
    process.exit();
  }
});