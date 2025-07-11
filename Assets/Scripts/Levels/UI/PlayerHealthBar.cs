using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealthBar : MonoBehaviour
{
    private ProgressBar progressBar;
    public Player player;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        progressBar = root.Q<ProgressBar>("PlayerBar");
        progressBar.lowValue = 0f;
        progressBar.highValue = (float) player.maxHealth;
        
        progressBar.value = (float) player.currentHealth;
    }

    public void Update()
    {
        progressBar.value = (float) player.currentHealth;
        if (progressBar.value <= 0)
        {
            Destroy(gameObject);
        }
    }
}