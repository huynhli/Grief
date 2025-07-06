using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Boss")]
    private Rigidbody2D rb;
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Damage Taken")]
    public SpriteRenderer spriteRenderer;
    private Color ogColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        ogColor = spriteRenderer.color;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashWhite());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = ogColor;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

