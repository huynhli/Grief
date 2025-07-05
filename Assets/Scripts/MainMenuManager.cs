using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private Transform tilemapTransform;

    [Header("UX")]
    private UIDocument uiDocument;
    private Label titleElement;
    private string titleText;
    private Button startButton;
    private Button levelSelectButton;
    private Button creditsButton;
    private Button exitButton;

    [Header("Sounds")]
    [SerializeField] private AudioClip titleLoadClip;
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField] private AudioClip mainMenuThemeClip;
    

    void Start()
    {
        SetUI();
        StartCoroutine(AnimateSequence());
    }


    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SoundManager.instance.PlaySFXClip(buttonHoverClip, player.transform, 0.4f);
    }

     private void StartButtonClicked()
    {
        StartCoroutine(WaitForSoundThenLoad());
    }

    private IEnumerator WaitForSoundThenLoad()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);
    }

    private void CreditsButtonClicked()
    {
        Debug.Log("Credit");
    }

    private void LevelSelectButtonClicked()
    {
        Debug.Log("Lvl select");
    }

    private void ExitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying=false;
#endif
    }

    private void SetUI()
    {
        uiDocument = GetComponent<UIDocument>();

        // Set title
        titleElement = uiDocument.rootVisualElement.Q<Label>("Title");
        titleText = titleElement.text;
        titleElement.text = "";

        // Set buttons
        startButton = uiDocument.rootVisualElement.Q<Button>("StartGame");
        creditsButton = uiDocument.rootVisualElement.Q<Button>("Credits");
        levelSelectButton = uiDocument.rootVisualElement.Q<Button>("LevelSelect");
        exitButton = uiDocument.rootVisualElement.Q<Button>("ExitGame");
        startButton.style.opacity = 0f;
        creditsButton.style.opacity = 0f;
        exitButton.style.opacity = 0f;

        startButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        creditsButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelSelectButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        exitButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        
        startButton.clicked += StartButtonClicked;
        creditsButton.clicked += CreditsButtonClicked;
        levelSelectButton.clicked += LevelSelectButtonClicked;
        exitButton.clicked += ExitButtonClicked;
    }

    IEnumerator AnimateSequence()
    {
        // player walk right and up, then shift background to left, sfx
        yield return StartCoroutine(AnimateWalkIn(4f, 9f, Vector2.right));

        yield return StartCoroutine(AnimateWalkIn(2f, 1f, Vector2.up));

        // enable buttons + text and music, sfx
        yield return StartCoroutine(AnimateTitle());

        yield return StartCoroutine(FadeButtons());
    }

    IEnumerator AnimateWalkIn(float duration, float destination, Vector2 unitVector)
    {
        Vector2 start = player.transform.position;
        float elapsed = 0f;
        Vector2 target = start + unitVector * destination;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            player.transform.position = Vector2.Lerp(start, target, progress); // Lerp smoothly goes from one position to another
            yield return null; // Wait one frame
        }
    }

    IEnumerator AnimateTitle()
    {
        yield return new WaitForSeconds(1.0f);

        SoundManager.instance.PlaySFXClip(titleLoadClip, player.transform, 15f);

        for (int i = 0; i < titleText.Length; i++)
        {
            titleElement.text += titleText[i];
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator FadeButtons()
    {
        SoundManager.instance.PlayLoopMusic(mainMenuThemeClip, player.transform, 10f);

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MoveStuff(2.5f));
        float duration = 4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            float opacity = Mathf.Lerp(0f, 1f, percent);
            startButton.style.opacity = opacity;
            creditsButton.style.opacity = opacity;
            exitButton.style.opacity = opacity;
            yield return null;
        }
    }

    IEnumerator MoveStuff(float distance)
    {

        float duration = 2.5f;
        float elapsed = 0f;

        Vector3 playerStart = player.transform.position;
        Vector3 tilemapStart = tilemapTransform.position;
        Vector3 playerTarget = playerStart + Vector3.left * distance;
        Vector3 tilemapTarget = tilemapStart + Vector3.left * distance;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // Clamp to prevent overshoot

            // Smooth skewed curve using a single mathematical function
            float curveValue = SmoothSkewedCurve(t);

            player.transform.position = Vector3.Lerp(playerStart, playerTarget, curveValue);
            tilemapTransform.position = Vector3.Lerp(tilemapStart, tilemapTarget, curveValue);

            yield return null;
        }

    }

    private float SmoothSkewedCurve(float t)
    {
        // Method 4: Exponential-like curve
        return 1f - Mathf.Exp(-4f * t); // Very fast start, smooth approach to end
    }
    

}
