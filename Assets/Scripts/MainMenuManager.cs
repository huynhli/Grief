using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections; 
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player player;

    [Header("UX")]
    private UIDocument uiDocument;
    private Label titleElement;
    private string titleText;
    private Button startButton;
    private Button creditsButton;
    private Button exitButton;

    void Start()
    {
        SetUI();
        StartCoroutine(AnimateSequence());
    }


    // Update is called once per frame
    void Update()
    {

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
        exitButton = uiDocument.rootVisualElement.Q<Button>("ExitGame");
        startButton.style.opacity = 0f;
        creditsButton.style.opacity = 0f;
        exitButton.style.opacity = 0f;
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

        for (int i = 0; i < titleText.Length; i++)
        {
            titleElement.text += titleText[i];
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator FadeButtons()
    {
        yield return new WaitForSeconds(1.0f);
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
}
