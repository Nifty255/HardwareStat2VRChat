const os = require("os");

const pollCpu = () => {
  return os.cpus().map((coreData) => {
    const times = coreData.times;
    const total = times.idle + times.user + times.sys + times.nice + times.irq;
    return 1 - (times.idle / total);
  });
};

const pollMem = () => {
  return 1 - (os.freemem() / os.totalmem());
};

module.exports = {
  pollCpu,
  pollMem
};