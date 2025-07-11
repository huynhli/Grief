using UnityEngine;
using System.Collections;

public class Tower1Prefab : MonoBehaviour
{
    [SerializeField] private float attackDelay;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float bulletVelocity;
    [SerializeField] private float rotationOffset = 22.5f; // How much to rotate (half of 45°)
    [SerializeField] private float randomAngleRange = 5f; // Max random angle variation in degrees
    [SerializeField] private float health = 1f;
    
    private bool alternatePattern = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AttackSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage()
    {
        health--;
    }


    IEnumerator AttackSequence() {
        while (true)
        {
            yield return new WaitForSeconds(attackDelay);
            
            float currentRotation = alternatePattern ? rotationOffset : 0f;
            
            for (int i = 0; i < 8; i++)
            {
                float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);
                float angle = (i * 45f + currentRotation + randomAngle) * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity)
                    .GetComponent<Rigidbody2D>().linearVelocity = dir * bulletVelocity;
            }
            
            // Toggle the pattern for next shot
            alternatePattern = !alternatePattern;
        }
    }
}