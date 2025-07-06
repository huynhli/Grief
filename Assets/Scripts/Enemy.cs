using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Boss")]
    private Rigidbody2D rb;
    public virtual int MaxHealth { get; set; } = 200;
    public int currentHealth;

    [Header("Damage Taken")]
    public SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashYellow());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashYellow()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f); 
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

