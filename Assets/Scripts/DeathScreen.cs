using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    private UIDocument uiDocument;
    private Transform screenTransform;
    private Button mainMenuButton;
    private Label youDied;
    [SerializeField] private AudioClip buttonHoverClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(InitializeUI());
    }
    
    IEnumerator InitializeUI()
    {
        // Wait a frame to ensure UI Document is ready
        yield return null;
        
        uiDocument = GetComponent<UIDocument>();
        
        // Add null checks and debug info
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found!");
            yield break;
        }
        
        if (uiDocument.rootVisualElement == null)
        {
            Debug.LogError("Root visual element is null!");
            yield break;
        }
        
        mainMenuButton = uiDocument.rootVisualElement.Q<Button>("BackButton");
        youDied = uiDocument.rootVisualElement.Q<Label>("Dead");
        
        // Check if elements were found
        if (mainMenuButton == null)
        {
            Debug.LogError("Button with name 'BackButton' not found in UI Document!");
            yield break;
        }
        
        if (youDied == null)
        {
            Debug.LogError("Label with name 'Dead' not found in UI Document!");
            yield break;
        }
        
        // Properly initialize the UI state
        mainMenuButton.style.display = DisplayStyle.None;
        youDied.style.display = DisplayStyle.None;
        mainMenuButton.SetEnabled(false); // Important: disable initially
        
        mainMenuButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        mainMenuButton.clicked += MainMenuButtonClicked;
        screenTransform = GetComponent<Transform>();
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SoundManager.instance.PlaySFXClip(buttonHoverClip, screenTransform, 0.4f);
    }

    private void MainMenuButtonClicked()
    {
        // Disable button to prevent multiple clicks
        mainMenuButton.SetEnabled(false);
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FadeIn()
    {
        StartCoroutine(FadeAnimation());
    }

    IEnumerator FadeAnimation()
    {
        youDied.style.display = DisplayStyle.Flex;
        mainMenuButton.style.display = DisplayStyle.Flex;

        youDied.style.opacity = 0f;
        mainMenuButton.style.opacity = 0f;
        
        // Ensure button is disabled at start of animation
        mainMenuButton.SetEnabled(false);

        float duration = 1.5f;
        float elapsed = 0f;
        float mainMenuStartTime = duration * 0.5f;
        float mainMenuElapsed = 0f;
        float mainMenuDuration = 0.75f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            float opacity = Mathf.Lerp(0f, 1f, percent);
            youDied.style.opacity = opacity;

            if (elapsed >= mainMenuStartTime)
            {
                mainMenuElapsed += Time.deltaTime;
                float mainMenuPercent = Mathf.Clamp01(mainMenuElapsed / mainMenuDuration);
                float mainMenuOpacity = Mathf.Lerp(0f, 1f, mainMenuPercent);
                mainMenuButton.style.opacity = mainMenuOpacity;
            }

            yield return null;
        }
        
        // Only enable the button after the animation completes
        mainMenuButton.SetEnabled(true);
    }
}