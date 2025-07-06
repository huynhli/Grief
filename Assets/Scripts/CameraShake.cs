using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Camera References")]
    public CinemachineCamera cineCamera;

    [Header("Shake Settings")]
    public float defaultShakeIntensity = 1f;
    public float defaultShakeDuration = 0.5f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get the noise component from the virtual camera
        InitializeNoiseComponent();
    }

    void Start()
    {
        // Additional initialization if needed after all objects are created
        if (noise == null)
        {
            InitializeNoiseComponent();
        }
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (noise != null)
            {
                // Decrease intensity over time for smooth fade out
                float currentIntensity = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
                noise.AmplitudeGain = currentIntensity;
            }

            if (shakeTimer <= 0f)
            {
                // Stop shaking
                StopShake();
            }
        }
    }

    private void InitializeNoiseComponent()
    {
        if (cineCamera != null)
        {
            // Get the noise component from the camera's noise stage
            noise = cineCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
            
            // If noise component doesn't exist, you'll need to add it manually in the inspector
            if (noise == null)
            {
                Debug.LogWarning("CameraShake: No CinemachineBasicMultiChannelPerlin component found on the camera. Please add a Noise component to your CinemachineCamera in the inspector.");
            }
        }
        else
        {
            Debug.LogError("CameraShake: No CinemachineCamera assigned!");
        }
    }

    public void Shake()
    {
        Shake(defaultShakeIntensity, defaultShakeDuration);
    }

    public void Shake(float intensity, float duration)
    {
        if (noise == null)
        {
            Debug.LogWarning("CameraShake: Cannot shake camera - no noise component found!");
            InitializeNoiseComponent(); // Try to reinitialize
            if (noise == null) return;
        }

        shakeTimer = duration;
        shakeTimerTotal = duration;
        startingIntensity = intensity;

        noise.AmplitudeGain = intensity;
    }

    public void StopShake()
    {
        shakeTimer = 0f;
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
        }
    }

    public void ShakeOnce(float intensity)
    {
        Shake(intensity, defaultShakeDuration);
    }

    public bool IsShaking()
    {
        return shakeTimer > 0f;
    }

    void OnValidate()
    {
        // Ensure values are reasonable in the inspector
        defaultShakeIntensity = Mathf.Max(0f, defaultShakeIntensity);
        defaultShakeDuration = Mathf.Max(0.1f, defaultShakeDuration);
    }
}