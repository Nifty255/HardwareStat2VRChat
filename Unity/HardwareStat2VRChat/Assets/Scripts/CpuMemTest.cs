using System.Diagnostics;
using UnityEngine;

public class CpuMemTest : MonoBehaviour
{

    //PerformanceCounter cpuCounter;
    //PerformanceCounter ramCounter;
    PerformanceCounter cpuCounter;
    PerformanceCounter ramCounter;


    void Start()
    {
        PerformanceCounterCategory.Exists("PerformanceCounter");

        cpuCounter = new PerformanceCounter();

        cpuCounter.CategoryName = "Processor";
        cpuCounter.CounterName = "% Processor Time";
        cpuCounter.InstanceName = "_Total";

        ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    }

    void Update()
    {
        UnityEngine.Debug.Log("> cpu: " + getCurrentCpuUsage() + "; >ram: " + getAvailableRAM());

    }

    public string getCurrentCpuUsage()
    {
        return cpuCounter.NextValue() + "%";
    }

    public string getAvailableRAM()
    {
        return ramCounter.NextValue() + "MB";
    }
}