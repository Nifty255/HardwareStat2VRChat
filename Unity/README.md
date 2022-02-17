# Hardware Stat 2 VRChat: Unity C# Driver

## Nifty Needs Help!

This version isn't done yet! Nifty was having trouble figuring out how to poll the CPU and RAM stats. See `CpuMemTest.cs` for my attempt. If you can get it working, modify `HardwarePoller.PollCores` and `HardwarePoller.PollMem`.

## Overview

Outputs CPU and Memory usage to VRChat avatar parameters via OSC. This version of the driver runs similar to a game.

CPU cores are output to `"/avatar/parameters/coreN"` where N is the zero-indexed core number. Mem usage is output to `"/avatar/parameters/mem"`. To allow this driver to affect your avatar, add Avatar and Animator parameters named "core0", "core1", "mem", etc.

## How to Run

Run with defaults by double-clicking. Exit by closing the window.

## How to Build

**Requires Unity, minimum version 2019.4.31f1**

1. Click _File_ -> _Build Settings..._
2. Change your build settings as desired. Defaults should work.
3. Click _Build_ and select an output location.

## Customization

Use command line parameters to modify the driver's target address and port, as well as poll/send interval and logging level. Below are a list of the parameters:

```
Name                  Default         Description

Target Address        "localhost"     Address of OSC target.
Target Port           9000            Port of the OSC target.
Send Interval (ms)    1000            Interval in milliseconds to poll and send hardware stats. Minimum value: 250ms
Auto-Scroll           ON              Auto-scrolls the log window if on.
Noisy Log             OFF             Enables more verbose logging when on.
```

## Misc. Information

Usually, this driver would be run on the same computer (technically, "host") as VRChat, meaning the target address should be "localhost". Additionally, by default, VRChat listens for OSC data on port 9000.

The interval's default is set to 1 second (and set to error at lower than 250ms) because avatars' parameters are by default set to sync at a slower rate than movement and IK data, about every 0.2-1.0 seconds. Besides this, hardware stats in general tend to look a bit jumpy when polled too frequently anyway.