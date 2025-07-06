using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TakeDamage(bulletDamage);

            Destroy(gameObject);
        }
    }
}
