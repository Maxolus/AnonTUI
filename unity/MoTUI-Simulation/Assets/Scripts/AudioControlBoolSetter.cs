using UnityEngine;
using System.Reflection;

public class BoolSetterTrigger : MonoBehaviour
{
    [Tooltip("Name of the GameObject that must enter the trigger (e.g., 'AV Box Collider')")]
    public string triggeringObjectName = "AV Box Collider";

    [Tooltip("Name of the GameObject that holds the script (e.g., 'UI Sound Manager')")]
    public string targetObjectName = "UI Sound Manager";

    [Tooltip("The name of the bool field to set in the ModuleSoundPlayerWithCustomLogic script")]
    public string boolFieldName;

    [Tooltip("The value to set the bool to when triggered")]
    public bool valueToSet = true;

    private void OnTriggerEnter(Collider other)
    {
        // Only react when the triggering object enters
        if (other.gameObject.name != triggeringObjectName)
            return;

        // Find the target GameObject
        GameObject targetObject = GameObject.Find(targetObjectName);
        if (targetObject == null)
        {
            Debug.LogWarning($"Target GameObject '{targetObjectName}' not found.");
            return;
        }

        // Get the component on that object
        var targetScript = targetObject.GetComponent<ModuleSoundPlayerWithCustomLogic>();
        if (targetScript == null)
        {
            Debug.LogWarning($"'{targetObjectName}' does not have ModuleSoundPlayerWithCustomLogic attached.");
            return;
        }

        // Set the specified bool field using reflection
        FieldInfo field = typeof(ModuleSoundPlayerWithCustomLogic).GetField(boolFieldName);
        if (field != null && field.FieldType == typeof(bool))
        {
            field.SetValue(targetScript, valueToSet);
            Debug.Log($"[{name}] Set '{boolFieldName}' to {valueToSet} on '{targetObjectName}'.");
        }
        else
        {
            Debug.LogWarning($"Field '{boolFieldName}' not found or not a bool on ModuleSoundPlayerWithCustomLogic.");
        }
    }
}
