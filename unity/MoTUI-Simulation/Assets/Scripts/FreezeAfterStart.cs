using UnityEngine;

public class FreezeAfterStart : MonoBehaviour
{
    private bool gamePaused = false;

    private GameObject yawUi;

    void Start()
    {
        yawUi = GameObject.Find("YawUI");

        // Freeze the game AFTER Start has run
        StartCoroutine(FreezeNextFrame());
    }

    private System.Collections.IEnumerator FreezeNextFrame()
    {
        yield return null; // Wait one frame to let everything initialize
        Time.timeScale = 0f;
        gamePaused = true;
        Debug.Log("Game is paused. Press Space to resume.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gamePaused)
            {
                // Resume the game
                yawUi.SetActive(false);
                Time.timeScale = 1f;
                gamePaused = false;
                Debug.Log("Game resumed.");
            }
            else
            {
                // Pause the game
                yawUi.SetActive(true);
                Time.timeScale = 0f;
                gamePaused = true;
                Debug.Log("Game paused.");
            }
        }
    }
}
