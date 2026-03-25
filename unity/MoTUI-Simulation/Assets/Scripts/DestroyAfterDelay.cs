using UnityEngine;
using System.Collections;

public class DestroyAfterDelay : MonoBehaviour
{
    private float lifetime = 24f;

    private void Start()
    {
        StartCoroutine(DestroyAfter());
    }

    private IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(lifetime); // Pauses with timeScale
        Destroy(gameObject);
    }
}