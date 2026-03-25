using UnityEngine;
using System.Collections;

public class OrientationAtDestination : MonoBehaviour
{
    private GameObject triggeringObject;
    private int moduleIndex = 0;
    private Coroutine triggerCoroutine;

    public string avObjectName = "AV Box Collider";
    public string otherObjectName = "Final Destination";
    private GameObject avObject;
    private GameObject otherObject;

    private void Awake()
    {
        if (triggeringObject == null)
        {
            triggeringObject = GameObject.Find("AV Box Collider");
            if (triggeringObject == null)
            {
                Debug.LogWarning("Could not find GameObject named 'AV Box Collider' in the scene.");
            }
            else
            {
                Debug.Log("'AV Box Collider' GameObject found automatically.");
            }
        }

        avObject = GameObject.Find(avObjectName);
        otherObject = GameObject.Find(otherObjectName);

        if (avObject == null) Debug.LogWarning($"GameObject '{avObjectName}' not found.");
        if (otherObject == null) Debug.LogWarning($"GameObject '{otherObjectName}' not found.");
    }

    private void Update()
    {
        var module = ModuleSettingsLoader.Instance.Modules[moduleIndex];

        // rotation servo behaviour
        if (avObject == null || otherObject == null) return;
        int rotAngle = CalculateRelativeAngle();
        // Optional: Display angle in console
        //Debug.Log($"Orientation at Destination: Relative angle to Destination: {rotAngle:F1}°");
        if (module.useRotation) module.rotationServoAngle = rotAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == triggeringObject)
        {
            Debug.Log("Triggered by the specific object: " + triggeringObject.name);

            if (ModuleSettingsLoader.Instance != null &&
                moduleIndex >= 0 && moduleIndex < ModuleSettingsLoader.Instance.Modules.Length)
            {
                var module = ModuleSettingsLoader.Instance.Modules[moduleIndex];

                // Initial changes
                module.verticalServoAngle = ModuleSettingsLoader.Instance.ExtensionAngle;
                if (module.useVibration)
                    module.vibMotorActive = true;
                if(module.useRotation)
                    module.rotationServoAngle = 165;
                if(module.useLinear)
                    module.linearServoAngle = 20;

                // Start the coroutine to handle timed changes
                if (triggerCoroutine != null) StopCoroutine(triggerCoroutine);
                triggerCoroutine = StartCoroutine(DisableVibrationAfterDelay(module, 4f));
            }
            else
            {
                Debug.LogWarning("ModuleSettingsLoader instance missing or invalid moduleIndex");
            }
        }
    }

    private int CalculateRelativeAngle()
    {
        Vector3 toOther = (otherObject.transform.position - avObject.transform.position).normalized;

        // Original signed angle (forward = 0°, right = 90°, etc.)
        float angle = Vector3.SignedAngle(avObject.transform.forward, toOther, Vector3.up);

        // Convert to [0, 360) range
        angle = (angle + 360f) % 360f;

        // Rotate the reference so that 0 = behind, 180 = ahead
        float adjustedAngle = (angle + 180f) % 360f;

        // Adjust it to the 0-180 range of the servo
        int adjustedAngleForServo = (int)(adjustedAngle / 2);

        return adjustedAngleForServo;
    }

    private IEnumerator DisableVibrationAfterDelay(ModuleSettingsLoader.ModuleData module, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(module.useVibration)
            module.vibMotorActive = false;
    }
}