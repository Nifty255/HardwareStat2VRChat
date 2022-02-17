# Hardware Stat 2 VRChat: Node.js Driver

## Overview

Outputs CPU and Memory usage to VRChat avatar parameters via OSC.

CPU cores are output to `"/avatar/parameters/coreN"` where N is the zero-indexed core number. Mem usage is output to `"/avatar/parameters/mem"`. To allow this driver to affect your avatar, add Avatar and Animator parameters named "core0", "core1", "mem", etc.

## How to Run

**Requires Node.js, minimum version 14.19.0.**

Remember to install the required modules with `npm i` first! Run using variables in the .env file by typing `node index.js`. Exit by pressing `Ctrl+C`.

## Customization

Use the .env file to modify the driver's target address and port, as well as poll/send interval and logging level.

```
Name          Default         Description

ADDR          "localhost"     Address of OSC target.
PORT          9000            Port of the OSC target.
INTERVAL      1000            Interval in milliseconds to poll and send hardware stats. Minimum value: 250ms
LOG           1               Log level. See below for list of levels.
```

The following are the available logging levels:

```javascript
{
  emerg: 0,
  alert: 1,
  crit: 2,
  error: 3,
  warning: 4,
  notice: 5,
  info: 6,
  debug: 7
}
```

## Misc. Information

Usually, this driver would be run on the same computer (technically, "host") as VRChat, meaning the target address should be "localhost". Additionally, by default, VRChat listens for OSC data on port 9000.

The interval's default is set to 1 second (and set to error at lower than 250ms) because avatars' parameters are by default set to sync at a slower rate than movement and IK data, about every 0.2-1.0 seconds. Besides this, hardware stats in general tend to look a bit jumpy when polled too frequently anyway.