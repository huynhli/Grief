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
    public int bulletsPerWave = 12;
    public int orbsPerWave = 16;
    private bool phaseTwo = false;

    [Header("Vulnerability System")]
    public int[] vulnerableAttacks = { 2, 4 };

    protected override void Start()
    {
        base.Start();
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

            StartCoroutine(HandleAttack(rand));

            yield return new WaitForSeconds(6.5f);
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
                yield return StartCoroutine(BulletBlockerVortexAttack());
                break;
            case 2:
                yield return StartCoroutine(DenialWaveAttack());
                break;
            case 3:
                yield return StartCoroutine(OrbsAttack());
                break;
            case 4: // Phase 2 only
                if (phaseTwo)
                    yield return StartCoroutine(UltimateBlockAttack());
                break;
        }
    }

    // Attack 1: Bullet blocker vortex - unavoidable, forces player to break bullets
    IEnumerator BulletBlockerVortexAttack()
    {
        float attackDuration = 5f;
        float spawnInterval = 0.3f;
        float elapsedTime = 0f;
        
        while (elapsedTime < attackDuration)
        {
            // Calculate vortex parameters
            float angle = (elapsedTime * 360f) % 360f; // Rotating angle
            float radius = 3f + Mathf.Sin(elapsedTime * 2f) * 1.5f; // Pulsing radius
            
            // Spawn bullet blockers in vortex pattern
            for (int i = 0; i < 6; i++)
            {
                float currentAngle = angle + (i * 60f); // 6 arms of the vortex
                float radians = currentAngle * Mathf.Deg2Rad;
                
                Vector3 spawnPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * radius,
                    Mathf.Sin(radians) * radius,
                    0f
                );
                
                GameObject blocker = Instantiate(bulletBlockerPrefab, spawnPos, Quaternion.identity);
                
                // Give the blocker movement toward the center, then outward
                StartCoroutine(MoveBulletBlockerInVortex(blocker, spawnPos, currentAngle));
                
                // Destroy after much longer time for extended travel
                Destroy(blocker, 10f);
            }
            
            elapsedTime += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator MoveBulletBlockerInVortex(GameObject blocker, Vector3 startPos, float startAngle)
    {
        if (blocker == null) yield break;
        
        float moveTime = 2f; // Much longer travel time
        float elapsedTime = 0f;
        
        while (blocker != null && elapsedTime < moveTime)
        {
            float progress = elapsedTime / moveTime;
            float currentAngle = startAngle + (progress * 180f); // Half rotation during movement
            float currentRadius = Mathf.Lerp(3f, 20f, progress); // Move much further outward
            
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 newPos = bossTransform.position + new Vector3(
                Mathf.Cos(radians) * currentRadius,
                Mathf.Sin(radians) * currentRadius,
                0f
            );
            
            blocker.transform.position = newPos;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Attack 2: Ripple waves originating from boss with gaps
    IEnumerator DenialWaveAttack()
    {
        int totalWaves = 6;
        float waveInterval = 4f;
        
        for (int wave = 0; wave < totalWaves; wave++)
        {
            // Create ripple wave emanating from boss
            StartCoroutine(CreateRippleWave(wave));
            
            yield return new WaitForSeconds(waveInterval);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator CreateRippleWave(int waveNumber)
    {
        float maxRadius = 30f;
        float waveSpeed = 20f;
        float currentRadius = 2f;
        
        // Create gap angle - this will be the safe spot in the ripple
        float gapAngle = UnityEngine.Random.Range(0f, 360f);
        float gapSize = 45f; // Size of the gap in degrees
        
        while (currentRadius < maxRadius)
        {
            // Calculate number of bullets based on circumference
            int bulletsInRing = Mathf.RoundToInt(currentRadius * 2f);
            bulletsInRing = Mathf.Max(8, bulletsInRing); // Minimum 8 bullets
            
            float angleStep = 360f / bulletsInRing;
            
            for (int i = 0; i < bulletsInRing; i++)
            {
                float currentAngle = i * angleStep;
                
                // Check if this bullet is in the gap
                float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, gapAngle));
                if (angleDifference < gapSize / 2f)
                    continue; // Skip bullets in the gap
                
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 bulletPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * currentRadius,
                    Mathf.Sin(radians) * currentRadius,
                    0f
                );
                
                GameObject rippleBullet = Instantiate(bullet, bulletPos, Quaternion.identity);
                
                // Give bullets outward velocity
                Rigidbody2D bulletRb = rippleBullet.GetComponent<Rigidbody2D>();
                if (bulletRb == null)
                    bulletRb = rippleBullet.AddComponent<Rigidbody2D>();
                
                Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);
                bulletRb.linearVelocity = direction * 4f;
                
                // Destroy bullet after time
                Destroy(rippleBullet, 6f);
            }
            
            currentRadius += waveSpeed * Time.deltaTime;
            yield return null;
        }
    }


    // Attack 3: Orbs that target player one by one with fast acceleration
    IEnumerator OrbsAttack()
    {
        int totalWaves = 4;
        float waveInterval = 3f;
        
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
        float acceleration = 25f; // Much higher acceleration
        float maxSpeed = 20f; // Higher max speed
        float currentSpeed = 5f;
        
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

    // Attack 4: Ultimate attack - combines all previous attacks
    IEnumerator UltimateBlockAttack()
    {
        float attackDuration = 15f;
        
        // Start all attack patterns simultaneously
        StartCoroutine(UltimateVortexComponent());
        StartCoroutine(UltimateWaveComponent());
        StartCoroutine(UltimateOrbComponent());
        
        yield return new WaitForSeconds(attackDuration);
    }


    IEnumerator UltimateVortexComponent()
    {
        float duration = 12f;
        float elapsedTime = 0f;
        float spawnInterval = 0.4f;
        
        while (elapsedTime < duration)
        {
            // Faster, more chaotic vortex
            float angle = (elapsedTime * 540f) % 360f; // Faster rotation
            float radius = 2f + Mathf.Sin(elapsedTime * 3f) * 1f;
            
            for (int i = 0; i < 4; i++) // Fewer arms but more frequent
            {
                float currentAngle = angle + (i * 90f);
                float radians = currentAngle * Mathf.Deg2Rad;
                
                Vector3 spawnPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * radius,
                    Mathf.Sin(radians) * radius,
                    0f
                );
                
                GameObject blocker = Instantiate(bulletBlockerPrefab, spawnPos, Quaternion.identity);
                StartCoroutine(UltimateMoveBlockerInVortex(blocker, spawnPos, currentAngle));
                Destroy(blocker, 6f); // Longer travel time
            }
            
            elapsedTime += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator UltimateMoveBlockerInVortex(GameObject blocker, Vector3 startPos, float startAngle)
    {
        if (blocker == null) yield break;
        
        float moveTime = 4f; // Faster movement for ultimate
        float elapsedTime = 0f;
        
        while (blocker != null && elapsedTime < moveTime)
        {
            float progress = elapsedTime / moveTime;
            float currentAngle = startAngle + (progress * 180f);
            float currentRadius = Mathf.Lerp(2f, 18f, progress); // Travel far and fast
            
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 newPos = bossTransform.position + new Vector3(
                Mathf.Cos(radians) * currentRadius,
                Mathf.Sin(radians) * currentRadius,
                0f
            );
            
            blocker.transform.position = newPos;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator UltimateWaveComponent()
    {
        yield return new WaitForSeconds(2f); // Offset start time
        
        for (int wave = 0; wave < 4; wave++) // More frequent ripples
        {
            StartCoroutine(CreateUltimateRippleWave(wave));
            yield return new WaitForSeconds(2f); // Faster ripple waves
        }
    }

    IEnumerator CreateUltimateRippleWave(int waveNumber)
    {
        float maxRadius = 12f;
        float waveSpeed = 10f; // Faster ripple expansion
        float currentRadius = 1.5f;
        
        // Smaller gap for ultimate difficulty
        float gapAngle = UnityEngine.Random.Range(0f, 360f);
        float gapSize = 35f; // Smaller gap
        
        while (currentRadius < maxRadius)
        {
            int bulletsInRing = Mathf.RoundToInt(currentRadius * 2.5f);
            bulletsInRing = Mathf.Max(6, bulletsInRing);
            
            float angleStep = 360f / bulletsInRing;
            
            for (int i = 0; i < bulletsInRing; i++)
            {
                float currentAngle = i * angleStep;
                
                float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, gapAngle));
                if (angleDifference < gapSize / 2f)
                    continue;
                
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 bulletPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * currentRadius,
                    Mathf.Sin(radians) * currentRadius,
                    0f
                );
                
                GameObject rippleBullet = Instantiate(bullet, bulletPos, Quaternion.identity);
                
                Rigidbody2D bulletRb = rippleBullet.GetComponent<Rigidbody2D>();
                if (bulletRb == null)
                    bulletRb = rippleBullet.AddComponent<Rigidbody2D>();
                
                Vector3 direction = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f);
                bulletRb.linearVelocity = direction * 5f; // Faster bullets
                
                Destroy(rippleBullet, 4f);
            }
            
            currentRadius += waveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator UltimateOrbComponent()
    {
        yield return new WaitForSeconds(4f); // Offset start time
        
        for (int wave = 0; wave < 3; wave++)
        {
            // Spawn orbs in circle
            GameObject[] orbs = new GameObject[10]; // Fewer orbs but more aggressive
            
            for (int i = 0; i < 10; i++)
            {
                float angle = (360f / 10) * i;
                float radians = angle * Mathf.Deg2Rad;
                float spawnRadius = 6f;
                
                Vector3 spawnPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * spawnRadius,
                    Mathf.Sin(radians) * spawnRadius,
                    0f
                );
                
                GameObject orb = Instantiate(bullet, spawnPos, Quaternion.identity);
                orbs[i] = orb;
                
                SpriteRenderer orbRenderer = orb.GetComponent<SpriteRenderer>();
                if (orbRenderer != null)
                {
                    orbRenderer.color = Color.red; // Red for ultimate attack
                    orb.transform.localScale = Vector3.one * 1.3f;
                }
                
                Destroy(orb, 8f);
            }
            
            // Launch orbs rapidly one by one
            for (int i = 0; i < 10; i++)
            {
                if (orbs[i] != null)
                {
                    Vector3 playerPos = player.transform.position;
                    StartCoroutine(UltimateLaunchOrbAtPlayer(orbs[i], playerPos));
                }
                
                yield return new WaitForSeconds(0.1f); // Very fast succession
            }
            
            yield return new WaitForSeconds(2.5f);
        }
    }

    IEnumerator UltimateLaunchOrbAtPlayer(GameObject orb, Vector3 targetPos)
    {
        if (orb == null) yield break;
        
        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb == null)
            orbRb = orb.AddComponent<Rigidbody2D>();
        
        Vector3 direction = (targetPos - orb.transform.position).normalized;
        
        // Even faster acceleration for ultimate
        float acceleration = 35f;
        float maxSpeed = 25f;
        float currentSpeed = 8f;
        
        while (orb != null && currentSpeed < maxSpeed)
        {
            orbRb.linearVelocity = direction * currentSpeed;
            currentSpeed += acceleration * Time.deltaTime;
            yield return null;
        }
        
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