using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource activeAudioSource;
    [SerializeField] private AudioSource soundFXObject;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioClip.length);
    }

    public void PlayLoopMusic(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (activeAudioSource != null)
        {
            activeAudioSource.Stop();
            Destroy(activeAudioSource.gameObject);
        }

        activeAudioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        activeAudioSource.clip = audioClip;
        activeAudioSource.volume = volume;
        activeAudioSource.loop = true;
        activeAudioSource.Play();
    }

    public void StopLoopingMusic()
    {
        if (activeAudioSource != null)
        {
            activeAudioSource.Stop();
            Destroy(activeAudioSource.gameObject);
            activeAudioSource = null;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
    
    public void FadeOutLoopingMusic(float fadeDuration)
    {
        if (activeAudioSource != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOutCoroutine(fadeDuration));
        }
    }
    
    // Fade to a specific volume without stopping
    public void FadeLoopingMusicTo(float targetVolume, float fadeDuration)
    {
        if (activeAudioSource != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeToVolumeCoroutine(targetVolume, fadeDuration));
        }
    }
    
    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = activeAudioSource.volume;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;
            activeAudioSource.volume = Mathf.Lerp(startVolume, 0f, normalizedTime);
            yield return null;
        }
        
        activeAudioSource.volume = 0f;
        StopLoopingMusic();
        fadeCoroutine = null;
    }
    
    private IEnumerator FadeToVolumeCoroutine(float targetVolume, float duration)
    {
        float startVolume = activeAudioSource.volume;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;
            activeAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, normalizedTime);
            yield return null;
        }
        
        activeAudioSource.volume = targetVolume;
        fadeCoroutine = null;
    }
}
