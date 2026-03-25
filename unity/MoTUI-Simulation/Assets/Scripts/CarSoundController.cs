using UnityEngine;

public class CarSoundController : MonoBehaviour
{
    public AudioSource engineAudio;   // Drag your engine AudioSource here in the Inspector
    public GameObject AV;             // Assign the AV GameObject in the Inspector

    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float maxSpeed = 100f;     // Max speed your car can go (tune this to your game's range)

    private Rigidbody avRigidbody;

    void Start()
    {
        if (AV != null)
        {
            avRigidbody = AV.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("AV GameObject not assigned!");
        }
    }

    void Update()
    {
        if (avRigidbody != null)
        {
            float speed = avRigidbody.linearVelocity.magnitude;
            float pitch = Mathf.Lerp(minPitch, maxPitch, speed / maxSpeed);
            engineAudio.pitch = pitch;
        }
    }
}
