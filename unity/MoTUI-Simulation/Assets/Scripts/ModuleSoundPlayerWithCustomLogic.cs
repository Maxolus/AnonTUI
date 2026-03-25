using UnityEngine;
using UnityEngine.Audio;

public class ModuleSoundPlayerWithCustomLogic : MonoBehaviour
{
    public AudioClip[] fallbackClips = new AudioClip[7];

    public AudioClip module0_Orientation;

    public AudioClip module1_SafeExitDirection;

    public AudioClip module2_ObstaclesAtExit;

    public AudioClip module3_45sLeft;
    public AudioClip module3_30sLeft;
    public AudioClip module3_15sLeft;
    public AudioClip module3_DestinationReached;

    public AudioClip module4_VehicleStopping;

    public AudioClip module5_OtherTrafficMemberLeft;

    public AudioClip module6_LeftTurn;
    public AudioClip module6_RightTurn;


    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioMixer audioMixer; // Reference to Audio Mixer with Pitch Shifter exposed
    private const string pitchParam = "Pitch"; // Must match exposed param name

    private int currentlyPlayingIndex = -1;
    private byte[] previousButtonStates = new byte[7];

    //bool values for logic:
    public bool destinationReached = false;

    public bool _45sLeft = true;
    public bool _30sLeft = false;
    public bool _15sLeft = false;

    public bool vehicleIsStopping = false;

    public bool otherTrafficMemberLeft = false;

    public bool leftTurn = false;
    public bool rightTurn = false;

    void Start()
    {
        if (audioSource == null)
        {
            GameObject go = GameObject.Find("UI Audio Source");
            if (go != null)
                audioSource = go.GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource assigned or found.");
            return;
        }
    }

    void Update()
    {
        if (audioSource == null || ModuleSettingsLoader.Instance == null)
            return;

        for (int i = 0; i < 7; i++)
        {
            var module = ModuleSettingsLoader.Instance.Modules[i];
            byte current = module.buttonState;
            byte previous = previousButtonStates[i];
            previousButtonStates[i] = current;

            bool justPressed = previous == 0 && current == 1;

            if (justPressed)
            {
                if (i == currentlyPlayingIndex && !audioSource.isPlaying)
                    PlayModuleClip(i, module);
                else if (i != currentlyPlayingIndex)
                    PlayModuleClip(i, module);

                break;
            }

            if (i == currentlyPlayingIndex && current == 0 && !audioSource.isPlaying)
                currentlyPlayingIndex = -1;
        }
    }

    void PlayModuleClip(int index, ModuleSettingsLoader.ModuleData module)
    {
        AudioClip clip = DetermineClipForModule(index, module);

        if (clip == null && fallbackClips.Length > index)
        {
            Debug.LogWarning($"No specific clip for module {index}, using fallback.");
            clip = fallbackClips[index];
        }

        if (clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;

            // Playback speed based on SoundOutputSpeed
            float speed = Mathf.Clamp(ModuleSettingsLoader.Instance.SoundOutputSpeed / 100f, 0.5f, 3.0f);
            Debug.Log($"Audio speed: {speed}");
            audioSource.pitch = speed;

            // Compensate pitch shift via exposed AudioMixer parameter
            if (audioMixer != null)
            {
                float inversePitch = 1f / speed;
                Debug.Log($"Audio inversePitch: {inversePitch}");
                audioMixer.SetFloat(pitchParam, inversePitch);
            }

            audioSource.Play();
            currentlyPlayingIndex = index;
        }
    }

    AudioClip DetermineClipForModule(int index, ModuleSettingsLoader.ModuleData module)
    {
        var modules = ModuleSettingsLoader.Instance.Modules;

        switch (index)
        {
            case 0:
                if (destinationReached) return module0_Orientation;
                break;

            case 1:
                if (destinationReached) return module1_SafeExitDirection;
                break;

            case 2:
                if (destinationReached) return module2_ObstaclesAtExit;
                break;

            case 3:
                if (destinationReached) return module3_DestinationReached;
                if (_45sLeft && !_30sLeft && !_15sLeft) return module3_45sLeft;
                if (_30sLeft && !_15sLeft) return module3_30sLeft;
                if (_15sLeft) return module3_15sLeft;
                break;

            case 4:
                if (vehicleIsStopping) return module4_VehicleStopping;
                if (destinationReached) return module3_DestinationReached;
                break;

            case 5:
                if (otherTrafficMemberLeft) return module5_OtherTrafficMemberLeft;
                break;
            
            case 6:
                if (leftTurn) return module6_LeftTurn;
                if (rightTurn) return module6_RightTurn;
                break;
        }

        return null;
    }
}
