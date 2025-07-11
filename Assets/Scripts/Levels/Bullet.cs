using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage = 1;

    void Start()
    {
        Destroy(gameObject, 2f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage);
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Tower"))
        {
            Tower1Prefab tower1 = other.GetComponent<Tower1Prefab>();
            if (tower1 != null)
            {
                tower1.TakeDamage();
                Destroy(gameObject);
                return;
            }
            Tower2Prefab tower2 = other.GetComponent<Tower2Prefab>();
            if (tower2 != null)
            {
                tower2.TakeDamage();
                Destroy(gameObject);
                return;
            }
            CrystalPrefab crystal = other.GetComponent<CrystalPrefab>();
            if (crystal != null)
            {
                crystal.TakeDamage();
                Destroy(gameObject);
                return;
            }
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
