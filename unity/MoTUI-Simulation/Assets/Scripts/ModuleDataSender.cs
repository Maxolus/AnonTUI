using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;

public class ModuleDataSender : MonoBehaviour
{
    private string portName = "COM7";
    private int baudRate = 115200;
    private int messageDelayMs = 55;
    private int sendIntervalMs = 250;

    private SerialPort serial;
    private Thread serialThread;
    private bool isRunning = false;

    private bool initialValuesSent = false;

    private ConcurrentQueue<string> incomingMessages = new ConcurrentQueue<string>();

    private void Start()
    {
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.Open();
            serial.ReadTimeout = 100;

            isRunning = true;
            serialThread = new Thread(SerialLoop);
            serialThread.Start();

            Debug.Log("Serial thread started.");
        }
        catch (Exception e)
        {
            Debug.LogError("Serial port error: " + e.Message);
        }
    }

    private void SerialLoop()
    {
        var modules = ModuleSettingsLoader.Instance.Modules;

        while (isRunning)
        {
            try
            {
                // Send initial values only once after startup
                if (!initialValuesSent)
                {
                    
                    SendArray("R", modules, m => m.r);
                    Thread.Sleep(messageDelayMs);

                    SendArray("G", modules, m => m.g);
                    Thread.Sleep(messageDelayMs);

                    SendArray("B", modules, m => m.b);
                    Thread.Sleep(messageDelayMs);
                    
                    
                    SendArray("VIBONTIME", modules, m => m.vibrationOnTime);
                    Thread.Sleep(messageDelayMs);

                    SendArray("VIBOFFTIME", modules, m => m.vibrationOffTime);
                    Thread.Sleep(messageDelayMs);

                    SendArray("VIBINTENSITY", modules, m => m.vibrationIntensity);
                    Thread.Sleep(messageDelayMs);

                    SendGlobal("SPD", ModuleSettingsLoader.Instance.ServoSpeed);
                    Thread.Sleep(messageDelayMs);
                    

                    initialValuesSent = true;
                }

                // Regular loop data
                SendArray("VIB", modules, m => m.vibMotorActive ? 1 : 0);
                Thread.Sleep(messageDelayMs);
                
                SendArray("LINSERVO", modules, m => m.linearServoAngle);
                Thread.Sleep(messageDelayMs);

                serial.WriteLine("BTN"); // Request button states
                Thread.Sleep(messageDelayMs);

                if (serial.BytesToRead > 0)
                {
                    string line = serial.ReadLine();
                    incomingMessages.Enqueue(line);
                }

                SendArray("VERTSERVO", modules, m => m.verticalServoAngle);
                Thread.Sleep(messageDelayMs);

                SendArray("ROTSERVO", modules, m => m.rotationServoAngle);
                Thread.Sleep(messageDelayMs);


                serial.WriteLine("BTN"); // Request button states
                Thread.Sleep(messageDelayMs);

                if (serial.BytesToRead > 0)
                {
                    string line = serial.ReadLine();
                    incomingMessages.Enqueue(line);
                }
                

                int totalDelay = 5 * messageDelayMs;
                int remainingSleep = sendIntervalMs - totalDelay;
                if (remainingSleep > 0)
                    Thread.Sleep(remainingSleep);
            }
            catch (TimeoutException) { }
            catch (Exception e)
            {
                Debug.LogWarning("Serial thread error: " + e.Message);
            }
        }
    }

    private void Update()
    {
        while (incomingMessages.TryDequeue(out string line))
        {
            if (line.StartsWith("BTN:"))
            {
                string[] states = line.Substring(4).Split(',');
                var modules = ModuleSettingsLoader.Instance.Modules;

                int halfLength = states.Length / 2;

                for (int i = 0; i < modules.Length && i < halfLength; i++)
                {
                    byte val1 = 0;
                    byte val2 = 0;

                    if (byte.TryParse(states[i], out byte parsed1)) val1 = parsed1;
                    if (byte.TryParse(states[i + halfLength], out byte parsed2)) val2 = parsed2;

                    modules[i].buttonState = (val1 > 0 || val2 > 0) ? (byte)1 : (byte)0;
                }
            }
        }
    }

    private void SendArray(string label, ModuleSettingsLoader.ModuleData[] modules, Func<ModuleSettingsLoader.ModuleData, object> selector)
    {
        string line = label + ":";
        for (int i = 0; i < modules.Length; i++)
        {
            line += selector(modules[i]);
            if (i < modules.Length - 1)
                line += ",";
        }
        serial.WriteLine(line);

        /*
        // Debug
        if (label == "ROTSERVO")
            Debug.Log("[Unity -> Arduino] " + line);
        */
    }

    private void SendGlobal(string label, object value)
    {
        serial.WriteLine($"{label}:{value}");

        /*
        // Debug
        if (label == "ROTSERVO")
        {
            string line = $"{label}:{value}";
            Debug.Log("[Unity -> Arduino] " + line);
            Debug.Log("[Unity -> Arduino] " + line);
        }
        */
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        if (serialThread != null && serialThread.IsAlive)
            serialThread.Join();

        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}

