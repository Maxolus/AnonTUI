using UnityEngine;
using System.Collections;

public class AppearAfterDelayManager : MonoBehaviour
{
    [Tooltip("Object to activate after delay.")]
    [SerializeField] private GameObject target;

    private float delay = 30f;

    private void Start()
    {
        if (target != null)
        {
            target.SetActive(false); // Hide target initially
            StartCoroutine(EnableAfterDelay());
        }
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(delay); // Will pause with Time.timeScale = 0
        target.SetActive(true); // Show it after the delay
    }
}
