using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

public class NotImplemented : MonoBehaviour
{
    [Header("UXML")]
    private UIDocument uiDocument;
    private VisualElement screen;
    private Button main;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        screen = uiDocument.rootVisualElement.Q<VisualElement>("NotImplemented");
        main = uiDocument.rootVisualElement.Q<Button>("Main");

        main.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        main.clicked += MainClicked;
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SoundManager.instance.PlayButtonHover();
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
}
