using UnityEngine;
using System.Collections;

public class OtherTrafficMembers : MonoBehaviour
{
    public string avObjectName = "AV Box Collider";
    public string otherObjectName = "Other Traffic Member";
    private float activationDistance = 40f;
    private int moduleIndex = 5;

    private GameObject avObject;
    private GameObject otherObject;
    private Coroutine triggerCoroutine;
    private bool isTriggered = false;
    private int proximityMappedValue;

    private void Awake()
    {
        avObject = GameObject.Find(avObjectName);
        otherObject = GameObject.Find(otherObjectName);

        if (avObject == null) Debug.LogWarning($"GameObject '{avObjectName}' not found.");
        if (otherObject == null) Debug.LogWarning($"GameObject '{otherObjectName}' not found.");
    }

    private void Update()
    {
        if (avObject == null || otherObject == null) return;

        float distance = Vector3.Distance(avObject.transform.position, otherObject.transform.position);

        if (distance <= activationDistance && !isTriggered)
        {
            isTriggered = true;
            TriggerBehavior();
        }
        else if (distance > activationDistance && isTriggered)
        {
            isTriggered = false;
            ResetBehavior();
        }


        if (isTriggered)
        {
            int angle = CalculateRelativeAngle();
            UpdateProximityMappedValue();
            var module = ModuleSettingsLoader.Instance.Modules[moduleIndex];
            
            // Optional: Display angle in console
            //Debug.Log($"Relative angle to AV: {angle:F1}°");

            if (module.useRotation) module.rotationServoAngle = angle;
            if (module.useLinear) module.linearServoAngle = proximityMappedValue;
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

    private void UpdateProximityMappedValue()
    {
        if (avObject == null || otherObject == null) return;

        Vector3 toOther = (otherObject.transform.position - avObject.transform.position).normalized;

        // Compute signed angle between forward and direction to otherObject
        float angle = Vector3.SignedAngle(avObject.transform.forward, toOther, Vector3.up);
        float adjustedAngle = (angle + 360f + 180f) % 360f;  // 0 = behind, 180 = in front

        // Calculate distance
        float distance = Vector3.Distance(avObject.transform.position, otherObject.transform.position);
        distance = Mathf.Clamp(distance, 4f, activationDistance); // Clamp to [4,40]

        // Determine if in front or behind
        bool isInFront = adjustedAngle > 90f && adjustedAngle < 270f;

        // Map distance accordingly
        if (isInFront)
        {
            // Front: map [0,10] -> [90,180]
            float t = Mathf.InverseLerp(4f, activationDistance, distance);
            proximityMappedValue = Mathf.RoundToInt(Mathf.Lerp(90f, 180f, t));
        }
        else
        {
            // Behind: map [0,10] -> [90,0]
            float t = Mathf.InverseLerp(4f, activationDistance, distance);
            proximityMappedValue = Mathf.RoundToInt(Mathf.Lerp(90f, 0f, t));
        }

        // Optional debug log
        //Debug.Log($"Object is {(isInFront ? "in front" : "behind")} | Distance: {distance:F2} | Value: {proximityMappedValue}");
    }

    private void TriggerBehavior()
    {
        Debug.Log("Other object entered range.");

        if (ModuleSettingsLoader.Instance != null &&
            moduleIndex >= 0 && moduleIndex < ModuleSettingsLoader.Instance.Modules.Length)
        {
            var module = ModuleSettingsLoader.Instance.Modules[moduleIndex];

            module.verticalServoAngle = ModuleSettingsLoader.Instance.ExtensionAngle;
            if (module.useVibration) 
                module.vibMotorActive = true;

            if (triggerCoroutine != null) StopCoroutine(triggerCoroutine);
            triggerCoroutine = StartCoroutine(DisableVibrationAfterDelay(module, 4f));
        }
        else
        {
            Debug.LogWarning("ModuleSettingsLoader instance missing or module index invalid.");
        }
    }

    private void ResetBehavior()
    {
        Debug.Log("Other object left range.");

        if (ModuleSettingsLoader.Instance != null &&
            moduleIndex >= 0 && moduleIndex < ModuleSettingsLoader.Instance.Modules.Length)
        {
            var module = ModuleSettingsLoader.Instance.Modules[moduleIndex];

            module.verticalServoAngle = 0;
            if (module.useLinear) module.linearServoAngle = 180;
            if (module.useRotation) module.rotationServoAngle = 90;
            if (module.useVibration) module.vibMotorActive = false;
        }
    }

    private IEnumerator DisableVibrationAfterDelay(ModuleSettingsLoader.ModuleData module, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (module.useVibration)
            module.vibMotorActive = false;
    }
}
