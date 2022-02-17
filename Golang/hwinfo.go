package main

import (
  "github.com/shirou/gopsutil/cpu"
  "github.com/shirou/gopsutil/mem"
  "github.com/rs/zerolog/log"
)

// Use zeroes as defaults in case the CPU usage queries error for some reason.
var defaultCpuPercentages []float64

func setDefaults() {
  cpus, errCpu := cpu.Counts(true)
  if errCpu != nil {
    log.Panic().Msgf("ERROR getting CPU counts: %q", errCpu)
  }
  defaultCpuPercentages = make([]float64, cpus)
}

func pollCPUs() []float64 {
  if defaultCpuPercentages == nil {
    setDefaults()
  }
  cpuPercentages, errCpu := cpu.Percent(0, true)
  if errCpu != nil {

    log.Error().Msgf("ERROR getting CPU stats: %q", errCpu)
    cpuPercentages = defaultCpuPercentages
  }

  return cpuPercentages
}

func pollMem() float64 {
  mem, errMem := mem.VirtualMemory()
  memPercent := float64(0)
  if errMem != nil {
    log.Error().Msgf("ERROR getting Mem stats: %q", errMem)
  } else {
    memPercent = mem.UsedPercent
  }

  return memPercent
}
