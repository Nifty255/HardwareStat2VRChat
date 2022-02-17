using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    // Defaults
    public string defaultSendAddress = "localhost";
    public int defaultSendPort = 9000;
    public float defaultIntervalMs = 1000f;
    public bool defaultScrollDown = true;
    public bool defaultNoisyLog = false;

    // Currently set
    private string currentSendAddress = "localhost";
    private int currentSendPort = 9000;
    private float currentIntervalMs = 1000f;
    private bool currentScrollDown = true;
    private bool currentNoisyLog = false;

    // UI handling
    public InputField sendAddressField;
    public InputField sendPortField;
    public InputField intervalField;
    public Toggle scrollDownToggle;
    public Toggle noisyLogToggle;

    // Services to configure
    public OSC osc;
    public Logger logger;
    public DataSender sender;

    // Start is called before the first frame update
    void Start()
    {
        osc.outIP = defaultSendAddress;
        osc.outPort = defaultSendPort;
        logger.scrollDown = defaultScrollDown;

        sendAddressField.text = defaultSendAddress;
        sendPortField.text = defaultSendPort.ToString();
        intervalField.text = defaultIntervalMs.ToString();
        scrollDownToggle.isOn = defaultScrollDown;
        noisyLogToggle.isOn = defaultNoisyLog;
    }

    // Save applies data set in UI to currents and the services.
    void Save()
    {
        // First step is to try and parse out the values before trying to assign them.
        string toSetSendAddress = sendAddressField.text;
        int toSetSendPort = 0;
        float toSetInterval = 0f;
        if (!int.TryParse(sendPortField.text, out toSetSendPort))
        {
            Debug.LogError("Error parsing port to number.");
            Cancel();
            return;
        }
        if (!float.TryParse(intervalField.text, out toSetInterval))
        {
            Debug.LogError("Error parsing interval to number.");
            Cancel();
            return;
        }

        toSetInterval = Mathf.Round(toSetInterval);
        if (toSetInterval < 1000)
        {
            if (toSetInterval < 250)
            {
                Debug.LogError($"Great Scott!!! Interval of {toSetInterval}ms is FAR too short to be useful and won't update that quickly to others in VRChat anyway.");
                Cancel();
                return;
            }
            Debug.LogWarning($"Interval of {toSetInterval}ms is less than 1 second. This could get heavy, Doc. Hardware stats may be jumpy, and others might not see your avatar params as smoothly as you think.");
        }

        // Close the OSC driver if it's open and some of its configuration will be changing.
        if (osc.IsOpen && (currentSendAddress != toSetSendAddress || currentSendPort != toSetSendPort))
        {
            Debug.Log("OSC is alive! Closing... (It'll restart on its own.)");
            osc.Close();
        }
        currentSendAddress = sendAddressField.text;
        currentSendPort = toSetSendPort;
        currentIntervalMs = toSetInterval;
        currentScrollDown = scrollDownToggle.isOn;
        currentNoisyLog = noisyLogToggle.isOn;

        osc.outIP = currentSendAddress;
        osc.outPort = currentSendPort;
        sender.SetInterval(currentIntervalMs);
        logger.scrollDown = currentScrollDown;

        // Set noisy logging in various places.
        sender.noisyLog = currentNoisyLog;

        Debug.Log("Settings saved!");
    }

    // Cancel reverts unsaved changes in the UI to the currently set config.
    void Cancel()
    {
        sendAddressField.text = currentSendAddress;
        sendPortField.text = currentSendPort.ToString();
        intervalField.text = currentIntervalMs.ToString();
        scrollDownToggle.isOn = currentScrollDown;
    }

    // RestoreDefaults reverts all current config and the config of each service to their defaults.
    void RestoreDefaults()
    {
        if (osc.IsOpen && (currentSendAddress != defaultSendAddress || currentSendPort != defaultSendPort))
        {
            Debug.Log("OSC is alive! Closing... (It'll restart on its own.)");
            osc.Close();
        }
        currentSendAddress = defaultSendAddress;
        currentSendPort = defaultSendPort;
        currentIntervalMs = defaultIntervalMs;
        currentScrollDown = defaultScrollDown;
        currentNoisyLog = defaultNoisyLog;

        sendAddressField.text = defaultSendAddress;
        sendPortField.text = defaultSendPort.ToString();
        intervalField.text = defaultIntervalMs.ToString();
        scrollDownToggle.isOn = defaultScrollDown;
        noisyLogToggle.isOn = defaultNoisyLog;

        osc.outIP = defaultSendAddress;
        osc.outPort = defaultSendPort;
        sender.SetInterval(defaultIntervalMs);
        logger.scrollDown = defaultScrollDown;

        Debug.Log("Defaults restored!");
    }
}
