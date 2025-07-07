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
        uiDocument = GetComponent<UIDocument>();
        mainMenuButton = uiDocument.rootVisualElement.Q<Button>("BackButton");
        youDied = uiDocument.rootVisualElement.Q<Label>("Dead");
        mainMenuButton.style.display = DisplayStyle.None;
        youDied.style.display = DisplayStyle.None;
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
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeIn()
    {
        Debug.Log("Fading");
        StartCoroutine(FadeAnimation());
    }

    IEnumerator FadeAnimation()
    {
        youDied.style.display = DisplayStyle.Flex;
        mainMenuButton.style.display = DisplayStyle.Flex;

        youDied.style.opacity = 0f;
        mainMenuButton.style.opacity = 0f;
        
        mainMenuButton.SetEnabled(true);
        
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

            mainMenuButton.SetEnabled(true);

            yield return null;
        }
    }
}
