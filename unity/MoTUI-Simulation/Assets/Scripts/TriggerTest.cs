using UnityEngine;
using System.Collections;

public class TriggerTest : MonoBehaviour
{
    private GameObject triggeringObject;
    private int moduleIndex = 6;
    private Coroutine triggerCoroutine;

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
                    module.rotationServoAngle = 45;
                if(module.useLinear)
                    module.linearServoAngle = 0;

                // Start the coroutine to handle timed changes
                if (triggerCoroutine != null) StopCoroutine(triggerCoroutine);
                triggerCoroutine = StartCoroutine(HandleTimedChanges(module));
            }
            else
            {
                Debug.LogWarning("ModuleSettingsLoader instance missing or invalid moduleIndex");
            }
        }
    }

    private IEnumerator HandleTimedChanges(ModuleSettingsLoader.ModuleData module)
    {
        // Start parallel coroutines for vibration and reset
        StartCoroutine(DisableVibrationAfterDelay(module, 4f));
        StartCoroutine(ResetAngleAfterDelay(module, 6f));

        // Animate linearServoAngle from 0 to 180 over 4.5 seconds
        float elapsed = 0f;
        float duration = 4.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if(module.useLinear)
                module.linearServoAngle = (int)Mathf.Lerp(0f, 180f, t);
            yield return null;
        }
    }

    private IEnumerator DisableVibrationAfterDelay(ModuleSettingsLoader.ModuleData module, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(module.useVibration)
            module.vibMotorActive = false;
    }

    private IEnumerator ResetAngleAfterDelay(ModuleSettingsLoader.ModuleData module, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (module.useLinear)
        {
            module.verticalServoAngle = 0;
            module.linearServoAngle = 0;
            module.rotationServoAngle = 90;
        }
            
    }
}