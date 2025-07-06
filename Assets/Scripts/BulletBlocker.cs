using UnityEngine;

public class BulletBlocker : MonoBehaviour
{
    [Header("Blocker Settings")]
    public int maxBlockedBullets = 5; // How many bullets it can block before breaking
    private int currentBlockedBullets = 0;
    
    [Header("Visual Effects")]
    // public GameObject blockEffectPrefab; // Particle effect when blocking
    // public AudioClip blockSound;
    // private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    
    // [Header("Destruction")]
    // public GameObject destructionEffectPrefab;
    // public AudioClip destructionSound;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Make sure it has a collider set as trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's a bullet
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            BlockBullet(bullet);
        }
    }

    void BlockBullet(Bullet bullet)
    {
        currentBlockedBullets++;
        
        // Play block effect
        // if (blockEffectPrefab)
        //     Instantiate(blockEffectPrefab, bullet.transform.position, Quaternion.identity);
        
        // // Play block sound
        // if (audioSource && blockSound)
        //     audioSource.PlayOneShot(blockSound);
        
        // Destroy the bullet
        Destroy(bullet.gameObject);
        
        // Check if blocker should break
        if (currentBlockedBullets >= maxBlockedBullets)
        {
            BreakBlocker();
        }
        else
        {
            // Visual feedback - flash or change color slightly
            StartCoroutine(FlashBlocker());
        }
    }

    void BreakBlocker()
    {
        // Play destruction effect
        // if (destructionEffectPrefab)
        //     Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        
        // // Play destruction sound
        // if (audioSource && destructionSound)
        //     audioSource.PlayOneShot(destructionSound);
        
        // Destroy the blocker
        Destroy(gameObject);
    }

    System.Collections.IEnumerator FlashBlocker()
    {
        if (spriteRenderer)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
}