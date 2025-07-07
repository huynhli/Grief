using UnityEngine;

public class BulletBlocker : MonoBehaviour
{
    [Header("Blocker Settings")]
    public int maxBlockedBullets = 1; // How many bullets it can block before breaking
    private int currentBlockedBullets = 0;
    public float bulletLife = 1f;  // Defines how long before the bullet is destroyed
    public float rotation = 0f;
    public float speed = 1f;
    private Vector2 spawnPoint;
    private float timer = 0f;
    
    [Header("Visual Effects")]
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
        
        // Make sure it has a collider set as trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void Update()
    {
        if(timer > bulletLife) Destroy(this.gameObject);
        timer += Time.deltaTime;
        transform.position = Movement(timer);
    }

    private Vector2 Movement(float timer)
    {
        // Moves right according to the bullet's rotation
        float x = timer * speed * transform.right.x;
        float y = timer * speed * transform.right.y;
        return new Vector2(x+spawnPoint.x, y+spawnPoint.y);
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
        
        // Destroy the bullet
        Destroy(bullet.gameObject);
        
        // Check if blocker should break
        if (currentBlockedBullets >= maxBlockedBullets)
        {
            BreakBlocker();
        }
        else
        {
            StartCoroutine(FlashBlocker());
        }
    }

    void BreakBlocker()
    {
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