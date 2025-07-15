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
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform tilemapTransform;

    [Header("UXML")]
    private UIDocument uiDocument;

    [Header("Main Menu")]
    private Label titleElement;
    private string titleText;
    private Button startButton;
    private Button levelSelectButton;
    private Button creditsButton;
    private Button exitButton;

    [Header("Level Select")]
    private VisualElement levelSelectMenu;
    private Button unlockAllButton;
    private Button levelOneButton;
    private VisualElement levelOneCover;
    private Button levelTwoButton;
    private VisualElement levelTwoCover;
    private Button levelThreeButton;
    private VisualElement levelThreeCover;
    private Button levelFourButton;
    private VisualElement levelFourCover;
    private Button levelFiveButton;
    private VisualElement levelFiveCover;
    private Button levelSelectBackButton;

    [Header("Credits")]
    private VisualElement creditsMenu;
    private Button creditsBackButton;

    [Header("Tutorial")]
    private VisualElement tutorialScreen;
    private VisualElement tutorial11;
    private VisualElement tutorial12;
    private Button tutorial13;
    private VisualElement tutorial13Cover;
    private VisualElement tutorial21;
    private VisualElement tutorial22;
    private Button tutorial23;
    private VisualElement tutorial23Cover;
    private VisualElement tutorial31;
    private VisualElement tutorial32;
    private Button tutorial33;
    private VisualElement tutorial33Cover;
    private Button tutorialGo;

    [Header("Sounds")]
    [SerializeField] private AudioClip titleLoadClip;
    [SerializeField] private AudioClip mainMenuThemeClip;

    [Header("Wait Time")]
    private WaitForSeconds waitZeroFour = new WaitForSeconds(0.4f);


    void Awake()
    {
        SetUI();
        StartCoroutine(AnimateSequence());
    }


    // Initializing everything
    private void SetUI()
    {
        QueryAll();
        RegisterAll();
        SetDisplayAll();
    }

    private void QueryAll()
    {
        uiDocument = GetComponent<UIDocument>();

        // Main Menu
        titleElement = uiDocument.rootVisualElement.Q<Label>("Title");
        startButton = uiDocument.rootVisualElement.Q<Button>("StartGame");
        creditsButton = uiDocument.rootVisualElement.Q<Button>("Credits");
        levelSelectButton = uiDocument.rootVisualElement.Q<Button>("LevelSelect");
        exitButton = uiDocument.rootVisualElement.Q<Button>("ExitGame");

        // Level Select
        levelSelectMenu = uiDocument.rootVisualElement.Q<VisualElement>("LevelSelectMenu");
        unlockAllButton = uiDocument.rootVisualElement.Q<Button>("UnlockAll");
        levelOneButton = uiDocument.rootVisualElement.Q<Button>("1");
        levelOneCover = uiDocument.rootVisualElement.Q<VisualElement>("1cover");
        levelTwoButton = uiDocument.rootVisualElement.Q<Button>("2");
        levelTwoCover = uiDocument.rootVisualElement.Q<VisualElement>("2cover");
        levelThreeButton = uiDocument.rootVisualElement.Q<Button>("3");
        levelThreeCover = uiDocument.rootVisualElement.Q<VisualElement>("3cover");
        levelFourButton = uiDocument.rootVisualElement.Q<Button>("4");
        levelFourCover = uiDocument.rootVisualElement.Q<VisualElement>("4cover");
        levelFiveButton = uiDocument.rootVisualElement.Q<Button>("5");
        levelFiveCover = uiDocument.rootVisualElement.Q<VisualElement>("5cover");
        levelSelectBackButton = uiDocument.rootVisualElement.Q<Button>("LevelBack");

        // Credits
        creditsMenu = uiDocument.rootVisualElement.Q<VisualElement>("CreditsMenu");
        creditsBackButton = uiDocument.rootVisualElement.Q<Button>("CreditBackButton");

        // Tutorial menu
        tutorialScreen = uiDocument.rootVisualElement.Q<VisualElement>("TutorialScreen");
        tutorial11 = uiDocument.rootVisualElement.Q<VisualElement>("Box11");
        tutorial12 = uiDocument.rootVisualElement.Q<VisualElement>("Box12");
        tutorial13 = uiDocument.rootVisualElement.Q<Button>("Button13");
        tutorial13Cover = uiDocument.rootVisualElement.Q<VisualElement>("Button13Cover");
        tutorial21 = uiDocument.rootVisualElement.Q<VisualElement>("Box21");
        tutorial22 = uiDocument.rootVisualElement.Q<VisualElement>("Box22");
        tutorial23 = uiDocument.rootVisualElement.Q<Button>("Button23");
        tutorial23Cover = uiDocument.rootVisualElement.Q<VisualElement>("Button23Cover");
        tutorial31 = uiDocument.rootVisualElement.Q<VisualElement>("Box31");
        tutorial32 = uiDocument.rootVisualElement.Q<VisualElement>("Box32");
        tutorial33 = uiDocument.rootVisualElement.Q<Button>("Button33");
        tutorial33Cover = uiDocument.rootVisualElement.Q<VisualElement>("Button33Cover");
        tutorialGo = uiDocument.rootVisualElement.Q<Button>("Go");
    }

    private void RegisterAll()
    {
        // Main Menu
        startButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        creditsButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelSelectButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        exitButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        startButton.clicked += StartButtonClicked;
        levelSelectButton.clicked += LevelSelectButtonClicked;
        creditsButton.clicked += CreditsButtonClicked;
        exitButton.clicked += ExitButtonClicked;

        // Level Select
        levelOneButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelTwoButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelThreeButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelFourButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelFiveButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelSelectBackButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        unlockAllButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelOneButton.clicked += () => LevelSelectedClicked(1);
        levelTwoButton.clicked += () => LevelSelectedClicked(2);
        levelThreeButton.clicked += () => LevelSelectedClicked(3);
        levelFourButton.clicked += () => LevelSelectedClicked(4);
        levelFiveButton.clicked += () => LevelSelectedClicked(5);
        levelSelectBackButton.clicked += () => BackButtonClicked(0);
        unlockAllButton.clicked += UnlockAllButtonClicked;

        // Credits
        creditsBackButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        creditsBackButton.clicked += () => BackButtonClicked(1);

        // Tutorial
        tutorial13.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorial23.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorial33.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorialGo.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorial13.clicked += Tutorial13Clicked;
        tutorial23.clicked += Tutorial23Clicked;
        tutorial33.clicked += Tutorial33Clicked;
        tutorialGo.clicked += TutorialGoClicked;
        VisibleDisplayComps(Array.Empty<VisualElement>(), new VisualElement[] { tutorialGo });
        tutorialGo.SetEnabled(false);
    }

    private void SetDisplayAll()
    {
        // main menu
        titleText = titleElement.text;
        titleElement.text = "";
        VisibleOpacityComps(Array.Empty<VisualElement>(), new VisualElement[]{startButton, creditsButton, levelSelectButton, exitButton});

        // Invisible level select menu, + enable for interactions
        // Credits menu
        // Tutorial screen
        VisibleDisplayComps(Array.Empty<VisualElement>(), new VisualElement[] { levelSelectMenu, creditsMenu, tutorialScreen });

        VisibleOpacityComps(Array.Empty<VisualElement>(), new VisualElement[] {tutorial11, tutorial12, tutorial13, tutorial13Cover, tutorial21, tutorial22, tutorial23, tutorial23Cover, tutorial31, tutorial32, tutorial33, tutorial33Cover, tutorialGo});
    }

    private void SetEnabled(Button[] arrayEnabling, Button[] arrayDisabling)
    {
        for (int i = 0; i < arrayEnabling.Length; i++)
        {
            arrayEnabling[i].SetEnabled(true);
        }
        for (int i = 0; i < arrayDisabling.Length; i++)
        {
            arrayDisabling[i].SetEnabled(false);
        }
    }


    // Button Logic //
    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SoundManager.instance.PlayButtonHover();
    }

    private void LevelSelectedClicked(int levelNum)
    {
        IEnumerator WaitForSoundThenLoad()
        {
            SoundManager.instance.PlayButtonClick();
            yield return waitZeroFour;
            SceneManager.LoadScene(levelNum);
        }
        StartCoroutine(WaitForSoundThenLoad());
    }

    private void StartButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();

        VisibleDisplayComps(new VisualElement[] { tutorialScreen }, new VisualElement[] { titleElement, startButton, levelSelectButton, creditsButton, exitButton });

        StartCoroutine(MovePlayerAndHole(2f, new Vector3(1.25f, -1f, 0f)));

        StartCoroutine(WaitForSecondsWrapper(0.6f));
        StartCoroutine(TutorialSequence(new VisualElement[] { tutorial11, tutorial12, tutorial13 }));
    }

    private IEnumerator TutorialSequence(VisualElement[] listOfComps)
    {
        SoundManager.instance.PlayButtonClick();
        for (int i = 0; i < listOfComps.Length; i++)
        {
            listOfComps[i].style.opacity = 1f;
            yield return waitZeroFour;
        }
    }

    private void Tutorial13Clicked()
    {
        StartCoroutine(TutorialSequence(new VisualElement[] { tutorial13Cover, tutorial21, tutorial22, tutorial23 }));
    }

    private void Tutorial23Clicked()
    {
        StartCoroutine(TutorialSequence(new VisualElement[] { tutorial23Cover, tutorial31, tutorial32, tutorial33 }));
    }

    private void Tutorial33Clicked()
    {
        StartCoroutine(TutorialSequence(new VisualElement[] { tutorial33Cover, tutorialGo }));
        VisibleDisplayComps(new VisualElement[] { tutorialGo }, Array.Empty<VisualElement>());
        tutorialGo.SetEnabled(true);
    }

    private void TutorialGoClicked()
    {
        LevelSelectedClicked(1);
    }

    private void BackButtonClicked(int det)
    {
        if (det == 0) // coming from level select
        {
            SoundManager.instance.PlayButtonClick();
            VisibleDisplayComps(new VisualElement[] { titleElement, startButton, levelSelectButton, creditsButton, exitButton }, new VisualElement[] { levelSelectMenu });
            StartCoroutine(MovePlayerAndHole(2f, new Vector3(-1.25f, 1f, 0f)));

        }
        else if (det == 1) // coming from credits
        {
            SoundManager.instance.PlayButtonClick();
            VisibleDisplayComps(new VisualElement[] { titleElement, startButton, levelSelectButton, creditsButton, exitButton }, new VisualElement[] { creditsMenu });
        }
        else
        {
            Debug.Log("Dafuq");
        }
    }

    private void VisibleDisplayComps(VisualElement[] listUICompsForVisible, VisualElement[] listUICompsForInvisible)
    {
        for (int i = 0; i < listUICompsForVisible.Length; i++)
        {
            listUICompsForVisible[i].style.display = DisplayStyle.Flex;
        }
        for (int i = 0; i < listUICompsForInvisible.Length; i++)
        {
            listUICompsForInvisible[i].style.display = DisplayStyle.None;
        }
    }

    private void VisibleOpacityComps(VisualElement[] listUICompsForVisible, VisualElement[] listUICompsForInvisible)
    {
        for (int i = 0; i < listUICompsForVisible.Length; i++)
        {
            listUICompsForVisible[i].style.opacity = 1f;
        }
        for (int i = 0; i < listUICompsForInvisible.Length; i++)
        {
            listUICompsForInvisible[i].style.opacity = 0f;
        }
    }

    private void UnlockAllButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();
        VisibleOpacityComps(Array.Empty<VisualElement>(), new VisualElement[]{levelTwoCover, levelThreeCover, levelFourCover, levelFiveCover});

        SetEnabled(new Button[] { levelTwoButton, levelThreeButton, levelFourButton, levelFiveButton }, Array.Empty<Button>());
    }

    private void CreditsButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();
        VisibleDisplayComps(new VisualElement[] { creditsMenu }, new VisualElement[] { titleElement, startButton, levelSelectButton, creditsButton, exitButton });
    }

    private void LevelSelectButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();
        VisibleDisplayComps(new VisualElement[] { levelSelectMenu }, new VisualElement[] { titleElement, startButton, levelSelectButton, creditsButton, exitButton });
        VisibleOpacityComps(new VisualElement[] { levelTwoCover, levelThreeCover, levelFourCover, levelFiveCover }, new VisualElement[] { levelOneCover });

        SetEnabled(Array.Empty<Button>(), new Button[] { levelTwoButton, levelThreeButton, levelFourButton, levelFiveButton });

        StartCoroutine(MovePlayerAndHole(2f, new Vector3(1.25f, -1f, 0f)));
    }

    private void ExitButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();

        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying=false;
#endif
    }


    // Animations //
    IEnumerator AnimateSequence()
    {
        // player walk right and up, then shift background to left, sfx
        yield return StartCoroutine(AnimateWalkIn(4f, 9f, Vector2.right));

        yield return StartCoroutine(AnimateWalkIn(1.25f, 1f, Vector2.up));

        // enable buttons + text and music, sfx
        yield return StartCoroutine(AnimateTitle());

        yield return StartCoroutine(FadeButtons());
    }

    IEnumerator AnimateWalkIn(float duration, float destination, Vector2 unitVector)
    {
        Vector2 start = playerTransform.position;
        float elapsed = 0f;
        Vector2 target = start + unitVector * destination;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            playerTransform.position = Vector2.Lerp(start, target, progress); // Lerp smoothly goes from one position to another
            yield return null; // Wait one frame
        }
    }

    IEnumerator AnimateTitle()
    {
        yield return new WaitForSeconds(1.0f);

        SoundManager.instance.PlaySFXClip(titleLoadClip, 15f);

        for (int i = 0; i < titleText.Length; i++)
        {
            titleElement.text += titleText[i];
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator FadeButtons()
    {
        yield return new WaitForSeconds(1.0f);
        SoundManager.instance.PlayLoopMusic(mainMenuThemeClip, 0.1f);
        StartCoroutine(MovePlayerAndHole(2.5f, Vector3.left));
        float duration = 4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            float opacity = Mathf.Lerp(0f, 1f, percent);
            startButton.style.opacity = opacity;
            creditsButton.style.opacity = opacity;
            levelSelectButton.style.opacity = opacity;
            exitButton.style.opacity = opacity;
            yield return null;
        }
    }

    IEnumerator MovePlayerAndHole(float distance, Vector3 direction)
    {

        float duration = 1f;
        float elapsed = 0f;

        Vector3 playerStart = playerTransform.position;
        Vector3 tilemapStart = tilemapTransform.position;
        Vector3 playerTarget = playerStart + direction * distance;
        Vector3 tilemapTarget = tilemapStart + direction * distance;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // Clamp to prevent overshoot

            // Smooth skewed curve using a single mathematical function
            float curveValue = 1f - Mathf.Exp(-4f * t);

            playerTransform.position = Vector3.Lerp(playerStart, playerTarget, curveValue);
            tilemapTransform.position = Vector3.Lerp(tilemapStart, tilemapTarget, curveValue);

            yield return null;
        }
    }
    
    private IEnumerator WaitForSecondsWrapper(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

}