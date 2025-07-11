using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Blocker Settings")]
    public int maxBlockedBullets = 1; // How many bullets it can block before breaking
    private int currentBlockedBullets = 0;

    [Header("Visual Effects")]
    private SpriteRenderer spriteRenderer;
    private DenialBoss denialBoss;
    private bool blockingEnabled = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the Denial Boss in the scene
        denialBoss = FindFirstObjectByType<DenialBoss>();

        // Make sure it has a collider set as trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void Update()
    {
        // Check if denial boss is in phase two
        if (denialBoss != null)
        {
            // Access the phase two status (you'll need to make this public in DenialBoss)
            blockingEnabled = denialBoss.isPhaseTwo;

            // Optional: Visual indicator when blocking is enabled
            if (blockingEnabled)
            {
                spriteRenderer.color = Color.red;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.TakeDamage();
            Destroy(gameObject);
        }

        if (blockingEnabled)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                BlockBullet(bullet);
                return; // Don't destroy on wall hit if we blocked a bullet
            }
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    
    void BlockBullet(Bullet bullet)
    {
        currentBlockedBullets++;
        
        // Destroy the player bullet
        Destroy(bullet.gameObject);
        
        // Check if this enemy bullet should break
        if (currentBlockedBullets >= maxBlockedBullets)
        {
            BreakBlocker();
        }
        else
        {
            // Visual feedback - flash when blocking
            StartCoroutine(FlashBlocker());
        }
    }

    void BreakBlocker()
    {
        // Optional: Add destruction effect here
        Destroy(gameObject);
    }

    System.Collections.IEnumerator FlashBlocker()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.red;
        }
    }
}
