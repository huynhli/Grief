using UnityEngine;
using UnityEngine.UIElements;

public class BossHealthBar : MonoBehaviour
{
    private ProgressBar progressBar;
    private Label titleLabel;
    public Enemy boss;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        titleLabel = root.Q<Label>("BossTitle");
        titleLabel.text = "";
        titleLabel.text = boss.BossTitle;

        progressBar = root.Q<ProgressBar>("ProgressBar");
        progressBar.lowValue = 0f;
        progressBar.highValue = (float) boss.MaxHealth;
        
        progressBar.value = (float) boss.currentHealth;
    }

    public void Update()
    {
        progressBar.value = (float)boss.currentHealth;
        if (progressBar.value <= 0)
        {
            Destroy(gameObject);
        }
    }
}