using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelUIManager : MonoBehaviour
{
    [Header("UXML")]
    private UIDocument uiDocument;
    [Header("Player Bar")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy boss;
    private VisualElement playerHealthBarScreen;
    private ProgressBar playerProgressBar;
    [Header("Boss Bar")]
    private VisualElement bossHealthBarScreen;
    private ProgressBar bossProgressBar;
    [Header("Win Screen")]
    private VisualElement winScreen;
    private Button winNext;
    private Button winMain;
    [Header("Death Screen")]
    private VisualElement deathScreen;
    private Button deathMain;
    private Button deathRetry;
    [Header("Pause Screen")]
    private VisualElement pauseScreen;
    private Button pauseUnpause;
    private Button pauseMain;
    private Button pauseHelp;
    private Label pauseHelpEnabledLabel;


    // Query + Register //
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        playerHealthBarScreen = uiDocument.rootVisualElement.Q<VisualElement>("PlayerBar");
        playerProgressBar = uiDocument.rootVisualElement.Q<ProgressBar>("PlayerProgressBar");
        playerProgressBar.lowValue = 0f;
        playerProgressBar.highValue = (float)player.maxHealth;

        bossHealthBarScreen = uiDocument.rootVisualElement.Q<VisualElement>("BossBar");
        bossProgressBar = uiDocument.rootVisualElement.Q<ProgressBar>("BossProgressBar");
        playerProgressBar.lowValue = 0f;
        playerProgressBar.highValue = (float)boss.MaxHealth;

        winScreen = uiDocument.rootVisualElement.Q<VisualElement>("Win");
        winNext = uiDocument.rootVisualElement.Q<Button>("WinNextButton");
        winMain = uiDocument.rootVisualElement.Q<Button>("WinMainButton");

        deathScreen = uiDocument.rootVisualElement.Q<VisualElement>("Death");
        deathMain = uiDocument.rootVisualElement.Q<Button>("DeadBackButton");
        deathRetry = uiDocument.rootVisualElement.Q<Button>("DeadRetry");

        bossHealthBarScreen.style.opacity = 0f;
        bossHealthBarScreen.style.display = DisplayStyle.None;
        winScreen.style.opacity = 0f;
        winScreen.style.display = DisplayStyle.None;
        deathScreen.style.opacity = 0f;
        deathScreen.style.display = DisplayStyle.None;
        deathMain.style.opacity = 0f;
        deathRetry.style.opacity = 0f;
        // pauseScreen.style.opacity = 0f;
        // pauseScreen.style.display = DisplayStyle.None;

        winNext.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        winNext.clicked += NextClicked;
        winMain.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        winMain.clicked += MainClicked;
        deathMain.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        deathMain.clicked += MainClicked;
        deathRetry.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        deathRetry.clicked += RetryClicked;
    }


    // Button logic //
    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SoundManager.instance.PlayButtonHover();
    }

    void NextClicked()
    {
        StartCoroutine(WaitForSoundThenLoad(SceneManager.GetActiveScene().buildIndex + 1));
    }

    void RetryClicked()
    {
        StartCoroutine(WaitForSoundThenLoad(SceneManager.GetActiveScene().buildIndex));
    }

    void MainClicked()
    {
        StartCoroutine(WaitForSoundThenLoad(0));
    }

    IEnumerator WaitForSoundThenLoad(int levelNum)
    {
        SoundManager.instance.PlayButtonClick();
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(levelNum);
    }


    // Progress Bars //
    void Update()
    {
        playerProgressBar.value = (float)player.currentHealth;
        if (playerProgressBar.value <= 0)
        {
            playerProgressBar.style.display = DisplayStyle.None;
        }
        bossProgressBar.value = (float)boss.currentHealth;
        if (bossProgressBar.value <= 0)
        {
            bossProgressBar.style.display = DisplayStyle.None;
        }
    }


    // Appearing Overlays //
    public void ShowBossBar()
    {
        IEnumerator BossBarAnimation() {
            bossProgressBar.style.display = DisplayStyle.Flex;
            float elapsed = 0f;
            float duration = 1.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                bossProgressBar.style.opacity = Mathf.Lerp(0f, 1f, progress);
                yield return null;
            }
        }
        StartCoroutine(BossBarAnimation());
    }

    public void ShowWinScreen()
    {
        IEnumerator WinScreenAnimation() {
            winScreen.style.display = DisplayStyle.Flex;
            float elapsed = 0f;
            float duration = 3f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                winScreen.style.opacity = Mathf.Lerp(0f, 1f, progress);
                yield return null;
            }
        }
        StartCoroutine(WinScreenAnimation());
    }

    public void ShowDeathScreen()
    {
        IEnumerator DeathScreenAnimation() {
            deathScreen.style.display = DisplayStyle.Flex;
            float elapsed = 0f;
            float durationForScreen = 3f;
            float startButtons = 2f;
            float endForButtons = 4f;
            float totalDuration = 4f; //
            
            while (elapsed < totalDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / durationForScreen);
                deathScreen.style.opacity = Mathf.Lerp(0f, 1f, progress);
                if (elapsed > startButtons)
                {
                    float progressButton = Mathf.Clamp01((elapsed - startButtons) / (endForButtons - startButtons));
                    deathMain.style.opacity = Mathf.Lerp(0f, 1f, progressButton);
                    deathRetry.style.opacity = Mathf.Lerp(0f, 1f, progressButton);
                }
                
                yield return null;
            }
        }
        StartCoroutine(DeathScreenAnimation());
    }

    public void ShowPauseScreen()
    {
        Time.timeScale = 0f;
        pauseScreen.style.opacity = 1f;
        pauseScreen.style.display = DisplayStyle.Flex;
    }

    public void HidePauseScreen()
    {
        Time.timeScale = 1f;
        pauseScreen.style.opacity = 0f;
        pauseScreen.style.display = DisplayStyle.None;
    }
}
