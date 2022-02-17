# Hardware Stat 2 VRChat: Golang Driver

## Overview

Outputs CPU and Memory usage to VRChat avatar parameters via OSC.

CPU cores are output to `"/avatar/parameters/coreN"` where N is the zero-indexed core number. Mem usage is output to `"/avatar/parameters/mem"`. To allow this driver to affect your avatar, add Avatar and Animator parameters named "core0", "core1", "mem", etc.

## How to Run

Run with defaults by double-clicking. Exit by pressing `Ctrl+C`.

## How to Build

**Requires Golang, minimum version 1.15.0**

`go build -o <path/name>` where `name` includes ".exe" on Windows.

## Customization

Use command line parameters to modify the driver's target address and port, as well as poll/send interval and logging level. Below are a list of the parameters:

```
Name          Default         Description

-addr         "localhost"     Address of OSC target.
-port         9000            Port of the OSC target.
-interval     1000            Interval in milliseconds to poll and send hardware stats. Minimum value: 250ms
-log          1               Log level. See below for list of levels.
```

You can also see these by typing `HWStat2VRChat.exe -h`

The following are the available logging levels:

```javascript
{
  panic: 5,
  fatal: 4,
  error: 3,
  warn: 2,
  info: 1,
  debug: 0,
  trace: -1
}
```

## Misc. Information

Usually, this driver would be run on the same computer (technically, "host") as VRChat, meaning the target address should be "localhost". Additionally, by default, VRChat listens for OSC data on port 9000.

The interval's default is set to 1 second (and set to error at lower than 250ms) because avatars' parameters are by default set to sync at a slower rate than movement and IK data, about every 0.2-1.0 seconds. Besides this, hardware stats in general tend to look a bit jumpy when polled too frequently anyway.