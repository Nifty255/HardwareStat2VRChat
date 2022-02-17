package main

import (
  "flag"
  "fmt"
  "math"
  "os"
  "os/signal"
  "syscall"
  "time"

  "github.com/rs/zerolog"
  "github.com/rs/zerolog/log"
)

// VRChat by default receives OSC data on port 9000.
var oscAddr = "localhost"
var oscPort = 9000

// Most custom avatar parameters use a "slow sync", about every 0.2 - 1 seconds,
// plus hardware averages are super jumpy when queried more often than this anyway.
var intervalMs = 1000

// Let the user raise or lower the log level. Default 1, "INFO"
var logLevel = int(zerolog.InfoLevel)

const HELP_TEXT = `
Outputs CPU and Memory usage to VRChat avatar parameters via OSC.
CPU cores are output to "/avatar/parameters/coreN" where N is the zero-indexed core number.
Mem usage is output to "/avatar/parameters/mem".
To allow this driver to affect your avatar, add Avatar and Animator parameters named "core0", "core1", "mem", etc.

`

func main() {
  flag.Usage = func() {
    fmt.Fprint(
      flag.CommandLine.Output(), HELP_TEXT)

    flag.PrintDefaults()
  }

  // Let the user customize the target address and port, as well as poll interval.
  flag.StringVar(&oscAddr, "addr", "localhost", "Address of OSC target. Usually, VRChat will be on the \"localhost\" address.")
  flag.IntVar(&oscPort, "port", 9000, "Port of the OSC target. By default, VRChat receives OSC data on port 9000.")
  flag.IntVar(&intervalMs, "interval", 1000, "Interval in milliseconds to poll and send hardware stats. Minimum value: 250ms")
  flag.IntVar(&logLevel, "log", int(zerolog.InfoLevel), "Log level. 1 is \"INFO\", 0 is \"DEBUG\"")

  flag.Parse()

  if intervalMs < 1000 {
    if intervalMs < 250 {
      log.Panic().Msgf("Great Scott!!! Interval of %dms is FAR too short to be useful and won't update that quickly to others in VRChat anyway.", intervalMs)
    }
    log.Warn().Msgf("Interval of %dms is less than 1 second. This could get heavy, Doc. Hardware stats may be jumpy, and others might not see your avatar params as smoothly as you think.", intervalMs)
  }

  log.Logger = log.Output(zerolog.ConsoleWriter{Out: os.Stderr})
  zerolog.SetGlobalLevel(zerolog.Level(logLevel))

  log.Info().Msg("Starting up!!! Press Ctrl+C to exit.")
  shouldQuit := false

  // Declare variables for keeping the process running.
  sigs := make(chan os.Signal, 1)
  signal.Notify(sigs)

  // Initialize the OSC client.
  initOscClient(oscAddr, oscPort)

  // Make an interval on which to send hardware stats.
  ticker := time.NewTicker(time.Duration(intervalMs) * time.Millisecond)
  log.Info().Msgf("We're doing it on an interval of 1 update per %d milliseconds.", intervalMs)

  // Start the main process loop.
  for !shouldQuit {
    
    // Block until something happens.
    select {

      case <-ticker.C:
        blastData()

      // OS signal. See if it's to stop the process.
      case s := <-sigs:
        log.Debug().Msgf("Received OS signal: %q", s)
        if (s == syscall.SIGTERM ||
            s == syscall.SIGKILL ||
            s == syscall.SIGQUIT ||
            s == syscall.SIGINT) {

              log.Info().Msgf("Got the signal to quit! We're making like a tree.")
          shouldQuit = true
        }
    }
  }
}

func blastData() {
  log.Debug().Msg("So anyways, I started blastin'...")

  cpuPercentages := pollCPUs()
  memPercent := pollMem()

  for core, percent := range(cpuPercentages) {
    sendFloat32(fmt.Sprintf("/avatar/parameters/core%d", core), float32(math.Floor(percent))/100)
    time.Sleep(time.Millisecond)
  }
  sendFloat32("/avatar/parameters/mem", float32(math.Floor(memPercent))/100)
}
