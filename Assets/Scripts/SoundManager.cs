using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource activeAudioSource;
    [SerializeField] private AudioSource soundFXObject;
    private Coroutine fadeCoroutine;
    private Transform uiTransform;
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField] private AudioClip buttonClickClip;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        uiTransform = transform;
    }


    // playing clips //
    public void PlaySFXClip(AudioClip audioClip, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, uiTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioClip.length);
    }

    public void PlayButtonHover()
    {
        AudioSource audioSource = Instantiate(soundFXObject, uiTransform.position, Quaternion.identity);
        audioSource.clip = buttonHoverClip;
        audioSource.volume = 0.4f;
        audioSource.Play();
        Destroy(audioSource.gameObject, buttonHoverClip.length);
    }

    public void PlayButtonClick()
    {
        AudioSource audioSource = Instantiate(soundFXObject, uiTransform.position, Quaternion.identity);
        audioSource.clip = buttonClickClip;
        audioSource.volume = 0.4f;
        audioSource.Play();
        Destroy(audioSource.gameObject, buttonClickClip.length);
    }


    // playing music //
    public void PlayLoopMusic(AudioClip audioClip, float volume)
    {
        if (activeAudioSource != null)
        {
            activeAudioSource.Stop();
            Destroy(activeAudioSource.gameObject);
        }

        activeAudioSource = Instantiate(soundFXObject, uiTransform.position, Quaternion.identity);
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

    
    // fade out music
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

    private IEnumerator FadeOutCoroutine(float duration)
    {
        if (activeAudioSource == null) yield break;
        
        float startVolume = activeAudioSource.volume;
        float timer = 0f;
        
        while (timer < duration && activeAudioSource != null)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;
            activeAudioSource.volume = Mathf.Lerp(startVolume, 0f, normalizedTime);
            yield return null;
        }
        
        if (activeAudioSource != null)
        {
            activeAudioSource.volume = 0f;
            activeAudioSource.Stop();
            Destroy(activeAudioSource.gameObject);
            activeAudioSource = null;
        }
        
        fadeCoroutine = null;
    }
    

    // Fade to a specific volume without stopping, for duration --> use case = dialogue/cutscene maybe
    public void FadeLoopingMusicTo(float targetVolume, float fadeDuration = 1f)
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
    
    private IEnumerator FadeToVolumeCoroutine(float targetVolume, float duration)
    {
        if (activeAudioSource == null) yield break;
        
        float startVolume = activeAudioSource.volume;
        float timer = 0f;
        
        while (timer < duration && activeAudioSource != null)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / duration;
            activeAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, normalizedTime);
            yield return null;
        }
        
        if (activeAudioSource != null)
        {
            activeAudioSource.volume = targetVolume;
        }
        
        fadeCoroutine = null;
    }
}
