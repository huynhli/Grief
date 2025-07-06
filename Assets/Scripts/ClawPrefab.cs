using UnityEngine;
using System.Collections;

public class ClawPrefab : MonoBehaviour
{
    private Rigidbody2D rb;
    public float initialV;
    public float accel;
    private bool shouldAccelerate = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // TODO play sfx
        StartCoroutine(MoveClaw());
    }

    void FixedUpdate()
    {
        if (shouldAccelerate)
        {
            Vector2 acceleration = new Vector2(accel * transform.localScale.x, 0f);
            rb.linearVelocity += acceleration * Time.fixedDeltaTime;
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
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator MoveClaw()
    {
        yield return new WaitForSeconds(1f); // Wait 1 second
        
        rb.linearVelocity = new Vector2(initialV * transform.localScale.x, 0f);
        
        shouldAccelerate = true;
    }
}
