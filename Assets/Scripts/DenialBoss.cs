using UnityEngine;
using System.Collections;
using Unity.Collections;

public class DenialBoss : Enemy
{
    [Header("Boss Phases")]
    public Player player;
    [SerializeField] private Transform bossTransform;
    [HideInInspector]
    private int maxHealth = 10;
    public override int MaxHealth => maxHealth;
    private string bossTitle = "Denial";
    public override string BossTitle => bossTitle;

    private Animator animator;
    private bool isInvulnerable = true; // Start invulnerable during intro
    private bool battleStarted = false;
    private bool isVulnerablePhase = false; // Tracks if boss is in vulnerable phase during attack sequence

    [Header("Audio")]
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip battleMusic;
    [SerializeField] private AudioClip denyBullet;

    [Header("Intro Animation")]
    public float introAnimationDuration = 22.9f; // Adjust based on your animation length

    [Header("Attack Patterns")]
    public GameObject bullet;
    public GameObject bulletBlockerPrefab; // For the denial/blocking mechanic
    public int orbsPerWave = 16;
    private bool phaseTwo = false;
    public GameObject turretPrefab;
    public Transform[] turretSpawnPoints;
    public float turretLifetime = 15f;
    private GameObject[] spawnedTurrets;

    [Header("Vulnerability System")]
    public int[] vulnerableAttacks = { 2, 4 };

    protected override void Start()
    {
        base.Start();
        spawnedTurrets = new GameObject[3];
        animator = GetComponent<Animator>();

        // Start with the intro sequence
        StartCoroutine(IntroSequence());
    }

    public override void TakeDamage(int damage)
    {
        // Don't take damage during intro animation
        if (isInvulnerable)
            return;

        // Only take damage during vulnerable attack phases
        if (!isVulnerablePhase)
        {
            // Play denial effect - boss blocks the damage
            StartCoroutine(PlayDenialEffect());
            return;
        }

        base.TakeDamage(damage);

        // Check for phase two transition
        if (base.currentHealth < maxHealth / 2 && !phaseTwo)
        {
            phaseTwo = true;
            animator.SetBool("isPhaseTwo", true);
            // You can add phase transition effects here
        }
    }

    protected override void Die()
    {
        // Stop all coroutines and play death animation
        StopAllCoroutines();

        animator.SetBool("isDead", true);

        // Stop battle music
        // Implement stop music here

        Invoke(nameof(DestroyBoss), 3.5f);
    }

    private void DestroyBoss()
    {
        base.Die();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && battleStarted)
        {
            player.TakeDamage();
        }
    }

    IEnumerator IntroSequence()
    {
        // Play intro animation
        animator.SetBool("isIntro", true);

        // Play intro music
        StartCoroutine(playIntroAfterOneSecond());

        // Wait for intro animation to complete
        yield return new WaitForSeconds(introAnimationDuration);

        // End intro animation
        animator.SetBool("isIntro", false);

        // Start battle phase
        StartBattle();
    }

    IEnumerator playIntroAfterOneSecond()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.instance.PlaySFXClip(introMusic, bossTransform, 2f);
    }

    private void StartBattle()
    {
        isInvulnerable = false;
        battleStarted = true;
        animator.SetBool("isAttack", true);

        // Switch to battle music
        SoundManager.instance.PlayLoopMusic(battleMusic, bossTransform, 0.1f);

        // Start attack sequence
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(1f);

        while (base.currentHealth > 0)
        {
            // Choose attack pattern based on phase
            int attackCount = phaseTwo ? 4 : 3;
            int rand = UnityEngine.Random.Range(1, attackCount + 1);

            yield return StartCoroutine(HandleAttack(rand));

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator HandleAttack(int atkType)
    {
        // Check if attack makes boss vulnerable
        bool shouldBeVulnerable = System.Array.Exists(vulnerableAttacks, x => x == atkType);

        if (shouldBeVulnerable)
        {
            isVulnerablePhase = true;
            animator.SetBool("isVulnerable", true);
        }
        else
        {
            isVulnerablePhase = false;
            animator.SetBool("isVulnerable", false);
        }
        switch (atkType)
        {
            case 1:
                yield return StartCoroutine(SpawnerAttack());
                break;
            case 2:
                yield return StartCoroutine(WaveAttack());
                break;
            case 3:
                yield return StartCoroutine(OrbsAttack());
                break;
            case 4: // Phase 2 only
                // if (phaseTwo)
                //     yield return StartCoroutine(UltimateAttack());
                break;
        }
    }

    // Attack 1: Turrets
    IEnumerator SpawnerAttack()
    {
        Destroy(Instantiate(turretPrefab, new Vector2(-20f, 10f), Quaternion.identity), turretLifetime);
        Destroy(Instantiate(turretPrefab, new Vector2(20f, 10f), Quaternion.identity), turretLifetime);
        Destroy(Instantiate(turretPrefab, new Vector2(0f, -10f), Quaternion.identity), turretLifetime);
    
        // Wait for the attack duration
        yield return new WaitForSeconds(turretLifetime);
    }


    // Attack 2: Wave Attack - 4 bullet ripples with dodge gaps
    IEnumerator WaveAttack()
    {
        int totalRipples = 5;
        int bulletsPerRipple = 100; // More bullets for better circle coverage
        float rippleInterval = 2f; // Time between each ripple
        float gapAngle = 15f; // Angle of the gap in degrees (adjust for difficulty)
        
        // Calculate initial gap direction (toward player's current position)
        Vector3 playerDirection = (player.transform.position - bossTransform.position).normalized;
        float gapStartAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        
        for (int ripple = 0; ripple < totalRipples; ripple++)
        {
            // Slightly rotate the gap for each ripple to make it more challenging
            float randomOffset = UnityEngine.Random.Range(-30f, 30f); // Random rotation within ±30 degrees
            float currentGapAngle = gapStartAngle + randomOffset;
            
            // Create bullets in a circle, skipping the gap area
            for (int i = 0; i < bulletsPerRipple; i++)
            {
                float bulletAngle = (360f / bulletsPerRipple) * i;
                
                // Check if this bullet would be in the gap area
                float angleDifference = Mathf.DeltaAngle(bulletAngle, currentGapAngle);
                
                // Skip bullets that would be in the gap
                if (Mathf.Abs(angleDifference) < gapAngle / 2f)
                    continue;
                
                // Convert angle to radians for positioning
                float radians = bulletAngle * Mathf.Deg2Rad;
                
                // Spawn bullet at boss position
                Vector3 spawnPos = bossTransform.position;
                GameObject bulletObj = Instantiate(bullet, spawnPos, Quaternion.identity);
                
                // Calculate direction for bullet movement
                Vector3 bulletDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);
                
                // Set bullet velocity
                Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
                if (bulletRb == null)
                    bulletRb = bulletObj.AddComponent<Rigidbody2D>();
                
                float bulletSpeed = 5f + (ripple * 2f); // Increase speed for later ripples
                bulletRb.linearVelocity = bulletDirection * bulletSpeed;
                
                // Make bullets visually distinct for each ripple
                SpriteRenderer bulletRenderer = bulletObj.GetComponent<SpriteRenderer>();
                if (bulletRenderer != null)
                {
                    // Color bullets based on ripple number
                    Color[] rippleColors = { Color.red, Color.magenta, Color.cyan, Color.yellow };
                    bulletRenderer.color = rippleColors[ripple % rippleColors.Length];
                }
                
                // Destroy bullet after time
                Destroy(bulletObj, 10f);
            }
            
            // Wait before next ripple
            if (ripple < totalRipples - 1)
                yield return new WaitForSeconds(rippleInterval);
        }
        
        // Wait a bit after all ripples are fired
        yield return new WaitForSeconds(2f);
    }


    // Attack 3: Orbs that target player one by one with fast acceleration
    IEnumerator OrbsAttack()
    {
        int totalWaves = 4;
        float waveInterval = 1f;
        
        for (int wave = 0; wave < totalWaves; wave++)
        {
            // Spawn orbs in a circle around the boss
            GameObject[] orbs = new GameObject[orbsPerWave];
            
            for (int i = 0; i < orbsPerWave; i++)
            {
                float angle = (360f / orbsPerWave) * i;
                float radians = angle * Mathf.Deg2Rad;
                float spawnRadius = 5f;
                
                Vector3 spawnPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * spawnRadius,
                    Mathf.Sin(radians) * spawnRadius,
                    0f
                );
                
                GameObject orb = Instantiate(bullet, spawnPos, Quaternion.identity);
                orbs[i] = orb;
                
                // Make orb visually distinct (you can change this based on your bullet prefab)
                SpriteRenderer orbRenderer = orb.GetComponent<SpriteRenderer>();
                if (orbRenderer != null)
                {
                    orbRenderer.color = Color.yellow;
                    orb.transform.localScale = Vector3.one * 1.2f; // Make orbs slightly larger
                }
                
                // Destroy orb after time if it hasn't been launched
                Destroy(orb, 12f);
            }
            
            // Launch orbs one by one at the player
            for (int i = 0; i < orbsPerWave; i++)
            {
                if (orbs[i] != null)
                {
                    Vector3 playerPos = player.transform.position;
                    StartCoroutine(LaunchOrbAtPlayer(orbs[i], playerPos));
                }
                
                yield return new WaitForSeconds(0.15f); // Short delay between each orb launch
            }
            
            yield return new WaitForSeconds(waveInterval);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator LaunchOrbAtPlayer(GameObject orb, Vector3 targetPos)
    {
        if (orb == null) yield break;
        
        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb == null)
            orbRb = orb.AddComponent<Rigidbody2D>();
        
        // Calculate direction to target
        Vector3 direction = (targetPos - orb.transform.position).normalized;
        
        // Fast acceleration - start with moderate speed and rapidly increase
        float acceleration = 30f; // Much higher acceleration
        float maxSpeed = 45f; // Higher max speed
        float currentSpeed = 15f;
        
        while (orb != null && currentSpeed < maxSpeed)
        {
            orbRb.linearVelocity = direction * currentSpeed;
            currentSpeed += acceleration * Time.deltaTime;
            yield return null;
        }
        
        // Maintain max speed
        if (orb != null)
            orbRb.linearVelocity = direction * maxSpeed;
    }


    IEnumerator PlayDenialEffect()
    {
        // Flash the boss or play a special effect
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.blue;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); 
        }
        
        // Play denial sound
        SoundManager.instance.PlaySFXClip(denyBullet, bossTransform, 0.2f);
    }

}