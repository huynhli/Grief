using UnityEngine;
using System.Collections;

public class Tower2Prefab : MonoBehaviour
{
    [SerializeField] private float attackDelay;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float bulletVelocity;
    [SerializeField] private int iterationsBeforeSwitch = 3;
    [SerializeField] private float randomAngleRange = 5f; // Max random angle variation in degrees
    [SerializeField] private float health = 1f;
    
    private bool shootingLeft = true;
    private int currentIteration = 0;
    
    void Start()
    {
        StartCoroutine(AttackSequence());
    }
    
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
    
    IEnumerator AttackSequence() 
    {
        while (true)
        {
            yield return new WaitForSeconds(attackDelay);
            
            // Shoot bullets in fan direction, alternating sides
            ShootFanDirection();
            
            currentIteration++;
            if (currentIteration >= iterationsBeforeSwitch)
            {
                shootingLeft = !shootingLeft;
                currentIteration = 0;
            }
        }
    }
    
    void ShootFanDirection()
    {
        // 8 bullets in a fan pattern (90 degrees total spread)
        int bulletCount = 8;
        float fanAngle = 90f;
        float baseAngle = shootingLeft ? 180f : 0f; // Left = 180°, Right = 0°
        
        for (int i = 0; i < bulletCount; i++)
        {
            float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);
            float angle = baseAngle + (i - (bulletCount - 1) / 2f) * (fanAngle / (bulletCount - 1)) + randomAngle;
            angle *= Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().linearVelocity = dir * bulletVelocity;
        }
    }
}