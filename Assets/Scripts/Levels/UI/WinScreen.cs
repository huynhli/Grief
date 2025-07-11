using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    private UIDocument uiDocument;
    private Transform screenTransform;
    private Button winButton;
    private Button mainButton;
    [SerializeField] private AudioClip buttonHoverClip;
    
    private bool isUIInitialized = false;
    
    void Start()
    {
        StartCoroutine(InitializeUI());
    }
   
    IEnumerator InitializeUI()
    {
        yield return null;
       
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found");
            yield break;
        }
        
        if (uiDocument.rootVisualElement == null)
        {
            Debug.LogError("Root visual element is null");
            yield break;
        }
       
        winButton = uiDocument.rootVisualElement.Q<Button>("NextButton");
        mainButton = uiDocument.rootVisualElement.Q<Button>("MainButton");
        
        if (winButton == null)
        {
            Debug.LogError("Button with name 'NextButton' not found");
            yield break;
        }
        
        if (mainButton == null)
        {
            Debug.LogError("Button with name 'MainButton' not found");
            yield break;
        }
       
        // DEBUG: Check button states
        Debug.Log($"WinButton found: {winButton != null}");
        Debug.Log($"MainButton found: {mainButton != null}");
        
        winButton.style.display = DisplayStyle.None;
        winButton.SetEnabled(false);
        winButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        winButton.clicked += WinButtonClicked;
        
        mainButton.style.display = DisplayStyle.None;
        mainButton.SetEnabled(false);
        mainButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        mainButton.clicked += MainMenuButtonClicked;
        
        // DEBUG: Verify callbacks are registered
        Debug.Log("Button callbacks registered");
        
        screenTransform = GetComponent<Transform>();
        
        isUIInitialized = true;
        Debug.Log("UI Initialization complete");
    }
    
    private void OnMouseEnter(MouseEnterEvent evt)
    {
        Debug.Log($"Mouse entered button: {evt.target}");
        SoundManager.instance.PlaySFXClip(buttonHoverClip, screenTransform, 0.4f);
    }
    
    private void WinButtonClicked()
    {
        Debug.Log("Win button clicked!");
        winButton.SetEnabled(false);
        mainButton.SetEnabled(false);
        SceneManager.LoadScene(2);
    }
    
    private void MainMenuButtonClicked()
    {
        Debug.Log("Main menu button clicked!");
        winButton.SetEnabled(false);
        mainButton.SetEnabled(false);
        SceneManager.LoadScene(0);
    }
    
    public void FadeIn()
    {
        Debug.Log($"FadeIn called from: {System.Environment.StackTrace}");
        StartCoroutine(FadeAnimation());
    }
    
    IEnumerator FadeAnimation()
    {
        Debug.Log("FadeAnimation started");
        
        while (!isUIInitialized)
        {
            yield return null;
        }
        
        if (winButton == null || mainButton == null)
        {
            Debug.LogError("Buttons are null in FadeAnimation");
            yield break;
        }
        
        mainButton.style.display = DisplayStyle.Flex;
        winButton.style.display = DisplayStyle.Flex;
        winButton.style.opacity = 0f;
        mainButton.style.opacity = 0f;
        
        Debug.Log("Starting fade animation");
        
        float duration = 1.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            float opacity = Mathf.Lerp(0f, 1f, percent);
            mainButton.style.opacity = opacity;
            winButton.style.opacity = opacity;
            yield return null;
        }
        
        mainButton.style.opacity = 1f;
        winButton.style.opacity = 1f;
        
        mainButton.SetEnabled(true);
        winButton.SetEnabled(true);
        
        Debug.Log("Buttons should now be enabled and clickable");
        
        // DEBUG: Final button states
        Debug.Log($"WinButton enabled: {winButton.enabledSelf}");
        Debug.Log($"MainButton enabled: {mainButton.enabledSelf}");
        Debug.Log($"WinButton display: {winButton.style.display}");
        Debug.Log($"MainButton display: {mainButton.style.display}");
    }
}