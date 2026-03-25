using UnityEngine;
using System.Collections;

public class ObstacleAtExit : MonoBehaviour
{
    private GameObject triggeringObject;
    private int moduleIndex = 2;
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
                    module.rotationServoAngle = 120;
                if(module.useLinear)
                    module.linearServoAngle = 140;

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

    private IEnumerator DisableVibrationAfterDelay(ModuleSettingsLoader.ModuleData module, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(module.useVibration)
            module.vibMotorActive = false;
    }
}