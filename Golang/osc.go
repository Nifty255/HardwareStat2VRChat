package main

import (
  "github.com/hypebeast/go-osc/osc"
  "github.com/rs/zerolog/log"
)

var oscClient *osc.Client

func initOscClient(addr string, port int) {
  oscClient = osc.NewClient(addr, port)
  log.Info().Msgf("OSC Client ready to toss data to \"%s:%d\"", addr, port)
}

func sendFloat32(path string, data float32) {
  if oscClient == nil {
    log.Panic().Msg("OSC Client not initialized!")
  }

  log.Debug().Msgf("Sending to path %q: %f", path, data)
  msg := osc.NewMessage(path, data)
  oscClient.Send(msg)
}
