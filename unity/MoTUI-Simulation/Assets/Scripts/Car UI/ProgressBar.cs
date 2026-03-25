using UnityEngine;
using TMPro;

public class TMPProgressBar : MonoBehaviour
{
    public TextMeshPro textMeshPro; // Drag your TMP object here
    public float duration = 42f;
    public int barLength = 340;      // Number of characters for full bar

    private float timer = 0f;

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = 0.5f + (0.5f * Mathf.Clamp01(timer / duration));

            int filledChars = Mathf.RoundToInt(progress * barLength);
            int emptyChars = barLength - filledChars;

            string bar = new string('|', filledChars) + new string(' ', emptyChars);
            textMeshPro.text = bar;
        }
    }
}