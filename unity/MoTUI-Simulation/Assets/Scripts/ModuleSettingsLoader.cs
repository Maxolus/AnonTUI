using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ModuleSettingsLoader : MonoBehaviour
{
    // Singleton instance
    public static ModuleSettingsLoader Instance { get; private set; }

    [System.Serializable]
    public class ModuleData
    {
        public bool useModule;
        public bool useLinear;
        public int r;
        public int g;
        public int b;
        public bool useVibration;
        public int vibrationIntensity;
        public int vibrationOnTime;
        public int vibrationOffTime;
        public bool useRotation;

        // New fields
        public bool vibMotorActive;
        public int linearServoAngle;
        public int verticalServoAngle;
        public int rotationServoAngle;
        public byte buttonState;
    }

    // Public read-only properties
    public float ExtensionLength { get; private set; }
    public int ServoSpeed { get; private set; }
    public int SoundOutputSpeed { get; private set; }
    public int ExtensionAngle { get; private set; }

    // Public access to modules
    public ModuleData[] Modules { get; private set; } = new ModuleData[7];

    [System.Serializable]
    private class RawJson
    {
        public float extension_length;
        public int servo_speed;
        public int sound_output_speed;

        public bool use_module_0, use_module_1, use_module_2, use_module_3, use_module_4, use_module_5, use_module_6;
        public bool large_0, large_1, large_2, large_3, large_4, large_5, large_6;
        public int r_0, r_1, r_2, r_3, r_4, r_5, r_6;
        public int g_0, g_1, g_2, g_3, g_4, g_5, g_6;
        public int b_0, b_1, b_2, b_3, b_4, b_5, b_6;
        public bool use_vibration_0, use_vibration_1, use_vibration_2, use_vibration_3, use_vibration_4, use_vibration_5, use_vibration_6;
        public int vibration_intensity_0, vibration_intensity_1, vibration_intensity_2, vibration_intensity_3, vibration_intensity_4, vibration_intensity_5, vibration_intensity_6;
        public int vibration_on_time_0, vibration_on_time_1, vibration_on_time_2, vibration_on_time_3, vibration_on_time_4, vibration_on_time_5, vibration_on_time_6;
        public int vibration_off_time_0, vibration_off_time_1, vibration_off_time_2, vibration_off_time_3, vibration_off_time_4, vibration_off_time_5, vibration_off_time_6;
        public bool use_rotation_0, use_rotation_1, use_rotation_2, use_rotation_3, use_rotation_4, use_rotation_5, use_rotation_6;
    }

    private static readonly Dictionary<float, int> ExtensionAngleLookup = new Dictionary<float, int>
{
    { 1.5f, 17 },
    { 2.0f, 18 },
    { 2.5f, 22 },
    { 3.0f, 25 },
    { 3.5f, 30 },
    { 4.0f, 33 },
    { 4.5f, 35 },
    { 5.0f, 36 },
    { 5.5f, 38 },
    { 6.0f, 40 },
    { 6.5f, 43 },
    { 7.0f, 47 },
    { 7.5f, 50 },
    { 8.0f, 55 },
    { 8.5f, 90 }
};

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadJson("Assets/Settings/current_trial.json");

        // Debug output
        for (int i = 0; i < Modules.Length; i++)
        {
            if (Modules[i] == null)
            {
                Debug.LogWarning($"Module {i} is null.");
                continue;
            }

            Debug.Log($"Module {i}: Use={Modules[i].useModule}, RGB=({Modules[i].r},{Modules[i].g},{Modules[i].b})");
        }
    }

    void LoadJson(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("JSON file not found at: " + path);
            return;
        }

        string jsonString = File.ReadAllText(path);
        RawJson rawData = JsonUtility.FromJson<RawJson>(jsonString);

        ExtensionLength = rawData.extension_length;
        ServoSpeed = rawData.servo_speed;
        SoundOutputSpeed = rawData.sound_output_speed;
        if (ExtensionAngleLookup.TryGetValue(ExtensionLength, out int angle))
        {
            ExtensionAngle = angle;
            Debug.Log($"ExtensionLength {ExtensionLength} leads to angle {angle}");
        }
        else
        {
            Debug.LogWarning($"ExtensionLength {ExtensionLength} is not in the lookup table. Defaulting ExtensionAngle to 90.");
            ExtensionAngle = 90;
        }

        // Start values for each module
        bool[] vibMotorActiveArray = { false, false, false, false, false, false, false };

        int[] servoTargetAngle = {
            90, 0, 90,
            90, 0, 90,
            90, 0, 90,
            90, ExtensionAngle, 90,
            0, 0, 0,
            180, 0, 90,
            0, 0, 90
        };

        byte[] buttonStates = { 0, 0, 0, 0, 0, 0, 0 };

        // Flat JSON field arrays mapped manually
        bool[] useModule = { rawData.use_module_0, rawData.use_module_1, rawData.use_module_2, rawData.use_module_3, rawData.use_module_4, rawData.use_module_5, rawData.use_module_6 };
        bool[] useLinear = { rawData.large_0, rawData.large_1, rawData.large_2, rawData.large_3, rawData.large_4, rawData.large_5, rawData.large_6 };
        int[] r = { rawData.r_0, rawData.r_1, rawData.r_2, rawData.r_3, rawData.r_4, rawData.r_5, rawData.r_6 };
        int[] g = { rawData.g_0, rawData.g_1, rawData.g_2, rawData.g_3, rawData.g_4, rawData.g_5, rawData.g_6 };
        int[] b = { rawData.b_0, rawData.b_1, rawData.b_2, rawData.b_3, rawData.b_4, rawData.b_5, rawData.b_6 };
        bool[] useVibration = { rawData.use_vibration_0, rawData.use_vibration_1, rawData.use_vibration_2, rawData.use_vibration_3, rawData.use_vibration_4, rawData.use_vibration_5, rawData.use_vibration_6 };
        int[] vibrationIntensity = { rawData.vibration_intensity_0, rawData.vibration_intensity_1, rawData.vibration_intensity_2, rawData.vibration_intensity_3, rawData.vibration_intensity_4, rawData.vibration_intensity_5, rawData.vibration_intensity_6 };
        int[] vibrationOn = { rawData.vibration_on_time_0, rawData.vibration_on_time_1, rawData.vibration_on_time_2, rawData.vibration_on_time_3, rawData.vibration_on_time_4, rawData.vibration_on_time_5, rawData.vibration_on_time_6 };
        int[] vibrationOff = { rawData.vibration_off_time_0, rawData.vibration_off_time_1, rawData.vibration_off_time_2, rawData.vibration_off_time_3, rawData.vibration_off_time_4, rawData.vibration_off_time_5, rawData.vibration_off_time_6 };
        bool[] useRotation = { rawData.use_rotation_0, rawData.use_rotation_1, rawData.use_rotation_2, rawData.use_rotation_3, rawData.use_rotation_4, rawData.use_rotation_5, rawData.use_rotation_6 };

        for (int i = 0; i < 7; i++)
        {
            Modules[i] = new ModuleData
            {
                useModule = useModule[i],
                useLinear = useLinear[i],
                r = r[i],
                g = g[i],
                b = b[i],
                useVibration = useVibration[i],
                vibrationIntensity = vibrationIntensity[i],
                vibrationOnTime = vibrationOn[i],
                vibrationOffTime = vibrationOff[i],
                useRotation = useRotation[i],

                // New fields initialized
                vibMotorActive = vibMotorActiveArray[i],
                linearServoAngle = servoTargetAngle[i * 3],
                verticalServoAngle = servoTargetAngle[i * 3 + 1],
                rotationServoAngle = servoTargetAngle[i * 3 + 2],
                buttonState = buttonStates[i]
            };
        }

        Debug.Log("Module settings loaded successfully.");
    }
}
