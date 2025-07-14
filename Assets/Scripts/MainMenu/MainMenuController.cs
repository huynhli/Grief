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


    void Awake()
    {
        SetUI();
        StartCoroutine(AnimateSequence());
    }

    private void SetUI()
    {
        uiDocument = GetComponent<UIDocument>();

        // Set title
        titleElement = uiDocument.rootVisualElement.Q<Label>("Title");
        titleText = titleElement.text;
        titleElement.text = "";

        // Find main menu buttons, set invisible, enable hovering + clicks 
        startButton = uiDocument.rootVisualElement.Q<Button>("StartGame");
        creditsButton = uiDocument.rootVisualElement.Q<Button>("Credits");
        levelSelectButton = uiDocument.rootVisualElement.Q<Button>("LevelSelect");
        exitButton = uiDocument.rootVisualElement.Q<Button>("ExitGame");
        startButton.style.opacity = 0f;
        creditsButton.style.opacity = 0f;
        levelSelectButton.style.opacity = 0f;
        exitButton.style.opacity = 0f;
        startButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        creditsButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        levelSelectButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        exitButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        startButton.clicked += StartButtonClicked;
        levelSelectButton.clicked += LevelSelectButtonClicked;
        creditsButton.clicked += CreditsButtonClicked;
        exitButton.clicked += ExitButtonClicked;

        // Invisible level select menu, + enable for interactions
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
        levelSelectMenu.style.display = DisplayStyle.None;
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

        // Credits menu
        creditsMenu = uiDocument.rootVisualElement.Q<VisualElement>("CreditsMenu");
        creditsBackButton = uiDocument.rootVisualElement.Q<Button>("CreditBackButton");
        creditsMenu.style.display = DisplayStyle.None;
        creditsBackButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        creditsBackButton.clicked += () => BackButtonClicked(1);

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

        tutorialScreen.style.display = DisplayStyle.None;
        tutorial11.style.opacity = 0f;
        tutorial12.style.opacity = 0f;
        tutorial13.style.opacity = 0f;
        tutorial13Cover.style.opacity = 0f;
        tutorial21.style.opacity = 0f;
        tutorial22.style.opacity = 0f;
        tutorial23.style.opacity = 0f;
        tutorial23Cover.style.opacity = 0f;
        tutorial31.style.opacity = 0f;
        tutorial32.style.opacity = 0f;
        tutorial33.style.opacity = 0f;
        tutorial33Cover.style.opacity = 0f;
        tutorialGo.style.opacity = 0f;


        tutorial13.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorial13.clicked += Tutorial13Clicked;
        tutorial23.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorial23.clicked += Tutorial23Clicked;
        tutorial33.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorial33.clicked += Tutorial33Clicked;
        tutorialGo.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        tutorialGo.clicked += TutorialGoClicked;

        tutorial13.pickingMode = PickingMode.Ignore; 
        tutorial23.pickingMode = PickingMode.Ignore; 
        tutorial33.pickingMode = PickingMode.Ignore; 
        tutorialGo.pickingMode = PickingMode.Ignore; 
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
            yield return new WaitForSeconds(0.4f);
            SceneManager.LoadScene(levelNum);
        }
        StartCoroutine(WaitForSoundThenLoad());
    }

    private void StartButtonClicked() {
        IEnumerator TutorialSequence1()
        {
            titleElement.style.display = DisplayStyle.None;
            startButton.style.display = DisplayStyle.None;
            levelSelectButton.style.display = DisplayStyle.None;
            creditsButton.style.display = DisplayStyle.None;
            exitButton.style.display = DisplayStyle.None;
            tutorialScreen.style.display = DisplayStyle.Flex;

            StartCoroutine(MoveStuff(2f, new Vector3(1.25f, -1f, 0f)));

            yield return new WaitForSeconds(1f);
            tutorial11.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial12.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial13.pickingMode = PickingMode.Position; 
            tutorial13.style.opacity = 1f;
        }
        StartCoroutine(TutorialSequence1());
    }

    private void Tutorial13Clicked()
    {
        IEnumerator TutorialSequence2()
        {
            tutorial13.pickingMode = PickingMode.Ignore;
            // tutorial13.SetEnabled(false);
            // tutorial13.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            tutorial13Cover.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial21.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial22.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial23.pickingMode = PickingMode.Position; 
            tutorial23.style.opacity = 1f;
        }
        StartCoroutine(TutorialSequence2());
    }

    private void Tutorial23Clicked()
    {
        IEnumerator TutorialSequence3()
        {
            tutorial23.pickingMode = PickingMode.Ignore; 
            tutorial23.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            tutorial23Cover.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial31.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial32.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorial33.pickingMode = PickingMode.Position; 
            tutorial33.style.opacity = 1f;
        }
        StartCoroutine(TutorialSequence3());
    }

    private void Tutorial33Clicked()
    {
        IEnumerator TutorialSequence4()
        {
            tutorial33.pickingMode = PickingMode.Ignore; 
            tutorial33.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            tutorial33Cover.style.opacity = 1f;
            yield return new WaitForSeconds(0.4f);
            tutorialGo.pickingMode = PickingMode.Position; 
            tutorialGo.style.opacity = 1f;
        }
        StartCoroutine(TutorialSequence4());
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
            levelSelectMenu.style.display = DisplayStyle.None;
            titleElement.style.display = DisplayStyle.Flex;
            startButton.style.display = DisplayStyle.Flex;
            levelSelectButton.style.display = DisplayStyle.Flex;
            creditsButton.style.display = DisplayStyle.Flex;
            exitButton.style.display = DisplayStyle.Flex;
            StartCoroutine(MoveStuff(2f, new Vector3(-1.25f, 1f, 0f)));

        }
        else if (det == 1) // coming from credits
        {
            SoundManager.instance.PlayButtonClick();
            creditsMenu.style.display = DisplayStyle.None;
            titleElement.style.display = DisplayStyle.Flex;
            startButton.style.display = DisplayStyle.Flex;
            levelSelectButton.style.display = DisplayStyle.Flex;
            creditsButton.style.display = DisplayStyle.Flex;
            exitButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.Log("Dafuq");
        }
    }

    private void UnlockAllButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();
        levelTwoCover.style.opacity = 0f;
        levelThreeCover.style.opacity = 0f;
        levelFourCover.style.opacity = 0f;
        levelFiveCover.style.opacity = 0f;
        levelTwoButton.SetEnabled(true);
        levelThreeButton.SetEnabled(true);
        levelFourButton.SetEnabled(true);
        levelFiveButton.SetEnabled(true);
    }

    private void CreditsButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();
        creditsMenu.style.display = DisplayStyle.Flex;
        titleElement.style.display = DisplayStyle.None;
        startButton.style.display = DisplayStyle.None;
        levelSelectButton.style.display = DisplayStyle.None;
        creditsButton.style.display = DisplayStyle.None;
        exitButton.style.display = DisplayStyle.None;
    }

    private void LevelSelectButtonClicked()
    {
        SoundManager.instance.PlayButtonClick();
        levelSelectMenu.style.display = DisplayStyle.Flex;
        titleElement.style.display = DisplayStyle.None;
        startButton.style.display = DisplayStyle.None;
        levelSelectButton.style.display = DisplayStyle.None;
        creditsButton.style.display = DisplayStyle.None;
        exitButton.style.display = DisplayStyle.None;

        // disable levels locked
        levelOneCover.style.opacity = 0f;
        levelTwoCover.style.opacity = 1f;
        levelThreeCover.style.opacity = 1f;
        levelFourCover.style.opacity = 1f;
        levelFiveCover.style.opacity = 1f;

        levelTwoButton.SetEnabled(false);
        levelThreeButton.SetEnabled(false);
        levelFourButton.SetEnabled(false);
        levelFiveButton.SetEnabled(false);

        // animation to move
        StartCoroutine(MoveStuff(2f, new Vector3(1.25f, -1f, 0f)));
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

        SoundManager.instance.PlaySFXClip(titleLoadClip, playerTransform, 15f);

        for (int i = 0; i < titleText.Length; i++)
        {
            titleElement.text += titleText[i];
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator FadeButtons()
    {
        yield return new WaitForSeconds(1.0f);
        SoundManager.instance.PlayLoopMusic(mainMenuThemeClip, playerTransform, 0.1f);
        StartCoroutine(MoveStuff(2.5f, Vector3.left));
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

    IEnumerator MoveStuff(float distance, Vector3 direction)
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

}