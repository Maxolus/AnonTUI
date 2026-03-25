using UnityEngine;
using System.Collections;

public class ModuleMonitor : MonoBehaviour
{
    [Header("Enabled")]
    public bool enable = false;

    [Header("Module Selection")]
    [Range(0, 6)]
    public int moduleIndex = 0;

    [Header("Data to Print")]
    public bool print_useModule = false;
    public bool print_useLinear = false;
    public bool print_RGB = false;
    public bool print_useVibration = false;
    public bool print_vibrationIntensity = false;
    public bool print_vibrationOnTime = false;
    public bool print_vibrationOffTime = false;
    public bool print_useRotation = false;
    public bool print_vibMotorActive = true;
    public bool print_linearServoAngle = true;
    public bool print_verticalServoAngle = true;
    public bool print_rotationServoAngle = true;
    public bool print_buttonState = true;

    private void Start()
    {
        if (ModuleSettingsLoader.Instance == null)
        {
            Debug.LogError("ModuleSettingsLoader instance not found in the scene.");
            return;
        }

        if (moduleIndex < 0 || moduleIndex >= ModuleSettingsLoader.Instance.Modules.Length)
        {
            Debug.LogError("Invalid module index.");
            return;
        }

        StartCoroutine(PrintModuleValues());
    }

    private IEnumerator PrintModuleValues()
    {
        while (true)
        {
            if (enable)
            {
                var module = ModuleSettingsLoader.Instance.Modules[moduleIndex];
                string output = $"--- Module {moduleIndex} Values ---\n";

                if (print_useModule) output += $"useModule: {module.useModule}\n";
                if (print_useLinear) output += $"useLinear: {module.useLinear}\n";
                if (print_RGB) output += $"RGB: ({module.r}, {module.g}, {module.b})\n";
                if (print_useVibration) output += $"useVibration: {module.useVibration}\n";
                if (print_vibrationIntensity) output += $"vibrationIntensity: {module.vibrationIntensity}\n";
                if (print_vibrationOnTime) output += $"vibrationOnTime: {module.vibrationOnTime}\n";
                if (print_vibrationOffTime) output += $"vibrationOffTime: {module.vibrationOffTime}\n";
                if (print_useRotation) output += $"useRotation: {module.useRotation}\n";
                if (print_vibMotorActive) output += $"vibMotorActive: {module.vibMotorActive}\n";
                if (print_linearServoAngle) output += $"linearServoAngle: {module.linearServoAngle}\n";
                if (print_verticalServoAngle) output += $"verticalServoAngle: {module.verticalServoAngle}\n";
                if (print_rotationServoAngle) output += $"rotationServoAngle: {module.rotationServoAngle}\n";
                if (print_buttonState) output += $"buttonState: {module.buttonState}\n";

                Debug.Log(output);
            }

            // Always yield to avoid freeze
            yield return new WaitForSeconds(1f);
        }
    }

}
