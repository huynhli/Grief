using UnityEngine;
using UnityEngine.UIElements;

public class BossHealthBar : MonoBehaviour
{
    private ProgressBar progressBar;
    public Enemy boss;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        progressBar = root.Q<ProgressBar>("ProgressBar");
        progressBar.lowValue = 0f;
        progressBar.highValue = (float) boss.maxHealth;
        
        progressBar.value = (float) boss.currentHealth;
    }

    public void Update()
    {
        progressBar.value = (float) boss.currentHealth;
    }
}