using UnityEngine;
using System.Collections;

public class CrystalPrefab : MonoBehaviour
{
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float bulletVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AttackSequence());
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    IEnumerator AttackSequence() {
        yield return new WaitForSeconds(6.5f);
        for (int i = 0; i < 24; i++)
        {
            float angle = i * 15f * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity)
                .GetComponent<Rigidbody2D>().linearVelocity = dir * bulletVelocity;
        }
    }
}
