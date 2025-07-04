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

        // player walk right and up, then shift background to left, sfx
        StartCoroutine(AnimateWalkIn());

        // enable buttons + text and music, sfx
        StartCoroutine(AnimateTitle());
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
    }

    IEnumerator AnimateWalkIn()
    {
        Vector2 start = player.transform.position;
        float elapsed = 0f;
        float duration = 2f;
        Vector2 target = start + Vector2.right * duration;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            player.transform.position = Vector3.Lerp(start, target, progress); // Lerp smoothly goes from one position to another
            yield return null; // Wait one frame
        }
    }

    IEnumerator AnimateTitle()
    {
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < titleText.Length; i++)
        {
            titleElement.text += titleText[i];
            yield return new WaitForSeconds(0.1f);
        }
    }
}
