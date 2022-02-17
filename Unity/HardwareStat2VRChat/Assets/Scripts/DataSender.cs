using System.Collections;
using UnityEngine;

public class DataSender : MonoBehaviour
{
    public OSC osc;
    private float intervalMs = 1000f;
    public bool noisyLog = false;
    private float sinceLastSend = 0f;

    // Update is called once per frame
    void Update()
    {
        // Only send once per interval.
        if (sinceLastSend < intervalMs)
        {
            sinceLastSend += (Time.unscaledDeltaTime * 1000);
        }
        else
        {
            if (noisyLog)
            {
                Debug.Log("So anyways, I started blastin'...");
            }

            // Get the hardware data.
            float[] corePercentages = HardwarePoller.PollCores();
            float memPercentage = HardwarePoller.PollMem();

            // Send it!
            StartCoroutine(SendStats(corePercentages, memPercentage));

            sinceLastSend -= intervalMs;
        }
    }

    private IEnumerator SendStats(float[] cores, float mem)
    {
        for (int i = 0; i < cores.Length; i++)
        {
            SendFloat32($"/avatar/parameters/core{i}", cores[i]);
            yield return new WaitForSecondsRealtime(0.001f); // I know it'll probably never be this accurate, but I'm nothing if not consistent. Other drivers wait 1ms, and so shall this one.
        }

        SendFloat32($"/avatar/parameters/mem", mem);
    }

    public void SetInterval(float newInterval)
    {
        intervalMs = newInterval;
        sinceLastSend = 0f;
    }

    private void SendFloat32(string path, float data)
    {
        if (noisyLog)
        {
            Debug.Log($"Sending to path \"{path}\": {data}");
        }
        OscMessage msg = new OscMessage();
        msg.address = path;
        msg.values.Add(data);
        osc.Send(msg);
    }
}
