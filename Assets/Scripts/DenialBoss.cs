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

    // Signature denial attack - creates bullet-blocking barriers
 IEnumerator BulletBlockerVortexAttack()
    {
        float attackDuration = 10f;
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
                
                // Destroy after a short time
                Destroy(blocker, 3f);
            }
            
            elapsedTime += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator MoveBulletBlockerInVortex(GameObject blocker, Vector3 startPos, float startAngle)
    {
        if (blocker == null) yield break;
        
        float moveTime = 2f;
        float elapsedTime = 0f;
        
        while (blocker != null && elapsedTime < moveTime)
        {
            float progress = elapsedTime / moveTime;
            float currentAngle = startAngle + (progress * 180f); // Half rotation during movement
            float currentRadius = Mathf.Lerp(3f, 8f, progress); // Move outward
            
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

    // Creates a wave of denial energy
    IEnumerator DenialWaveAttack()
    {
        int totalWaves = 6;
        float waveInterval = 1f;
        
        for (int wave = 0; wave < totalWaves; wave++)
        {
            // Determine wave direction (alternating)
            bool leftToRight = (wave % 2 == 0);
            float waveAngle = leftToRight ? 0f : 180f;
            Vector3 waveStartPos = bossTransform.position + new Vector3(
                leftToRight ? -15f : 15f,
                UnityEngine.Random.Range(-3f, 3f),
                0f
            );
            
            // Create bullets with gaps
            for (int i = 0; i < bulletsPerWave; i++)
            {
                // Create gaps at specific intervals
                bool isGap = (i % 5 == 2 || i % 5 == 3); // 2 bullet gap every 5 bullets
                if (isGap) continue;
                
                Vector3 bulletPos = waveStartPos + new Vector3(0f, (i - bulletsPerWave/2) * 0.8f, 0f);
                GameObject waveBullet = Instantiate(bullet, bulletPos, Quaternion.identity);
                
                // Give bullet velocity
                Rigidbody2D bulletRb = waveBullet.GetComponent<Rigidbody2D>();
                if (bulletRb == null)
                    bulletRb = waveBullet.AddComponent<Rigidbody2D>();
                
                Vector3 direction = leftToRight ? Vector3.right : Vector3.left;
                bulletRb.linearVelocity = direction * 6f;
                
                // Destroy bullet after time
                Destroy(waveBullet, 5f);
            }
            
            yield return new WaitForSeconds(waveInterval);
        }
        
        yield return new WaitForSeconds(1f);
    }


    // Bullet hell attack
    IEnumerator OrbsAttack()
    {
        int totalWaves = 4;
        float waveInterval = 2f;
        
        for (int wave = 0; wave < totalWaves; wave++)
        {
            // Store player's current position as target
            Vector3 playerPos = player.transform.position;
            
            // Spawn orbs in a circle around the boss
            for (int i = 0; i < orbsPerWave; i++)
            {
                float angle = (360f / orbsPerWave) * i;
                float radians = angle * Mathf.Deg2Rad;
                float spawnRadius = 4f;
                
                Vector3 spawnPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * spawnRadius,
                    Mathf.Sin(radians) * spawnRadius,
                    0f
                );
                
                GameObject orb = Instantiate(bullet, spawnPos, Quaternion.identity);
                
                // Start the orb movement coroutine
                StartCoroutine(MoveOrbToTarget(orb, playerPos));
                
                // Destroy orb after time if it hasn't hit anything
                Destroy(orb, 8f);
            }
            
            yield return new WaitForSeconds(waveInterval);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator MoveOrbToTarget(GameObject orb, Vector3 targetPos)
    {
        if (orb == null) yield break;
        
        // Wait a moment before starting movement
        yield return new WaitForSeconds(0.5f);
        
        if (orb == null) yield break;
        
        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        if (orbRb == null)
            orbRb = orb.AddComponent<Rigidbody2D>();
        
        // Calculate direction to target
        Vector3 direction = (targetPos - orb.transform.position).normalized;
        
        // Start with slow movement, then accelerate
        float acceleration = 2f;
        float maxSpeed = 12f;
        float currentSpeed = 3f;
        
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

    // Phase 2 ultimate attack - maximum denial
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
                StartCoroutine(MoveBulletBlockerInVortex(blocker, spawnPos, currentAngle));
                Destroy(blocker, 2.5f);
            }
            
            elapsedTime += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator UltimateWaveComponent()
    {
        yield return new WaitForSeconds(2f); // Offset start time
        
        for (int wave = 0; wave < 8; wave++) // More waves
        {
            bool leftToRight = (wave % 2 == 0);
            Vector3 waveStartPos = bossTransform.position + new Vector3(
                leftToRight ? -12f : 12f,
                UnityEngine.Random.Range(-4f, 4f),
                0f
            );
            
            for (int i = 0; i < 10; i++) // Fewer bullets per wave but more frequent
            {
                bool isGap = (i % 4 == 2); // Smaller gaps
                if (isGap) continue;
                
                Vector3 bulletPos = waveStartPos + new Vector3(0f, (i - 5) * 0.9f, 0f);
                GameObject waveBullet = Instantiate(bullet, bulletPos, Quaternion.identity);
                
                Rigidbody2D bulletRb = waveBullet.GetComponent<Rigidbody2D>();
                if (bulletRb == null)
                    bulletRb = waveBullet.AddComponent<Rigidbody2D>();
                
                Vector3 direction = leftToRight ? Vector3.right : Vector3.left;
                bulletRb.linearVelocity = direction * 7f; // Faster bullets
                
                Destroy(waveBullet, 4f);
            }
            
            yield return new WaitForSeconds(1.2f); // Faster waves
        }
    }

    IEnumerator UltimateOrbComponent()
    {
        yield return new WaitForSeconds(4f); // Offset start time
        
        for (int wave = 0; wave < 3; wave++) // Fewer but more intense waves
        {
            Vector3 playerPos = player.transform.position;
            
            // Spawn more orbs
            for (int i = 0; i < 12; i++)
            {
                float angle = (360f / 12) * i;
                float radians = angle * Mathf.Deg2Rad;
                float spawnRadius = 5f;
                
                Vector3 spawnPos = bossTransform.position + new Vector3(
                    Mathf.Cos(radians) * spawnRadius,
                    Mathf.Sin(radians) * spawnRadius,
                    0f
                );
                
                GameObject orb = Instantiate(bullet, spawnPos, Quaternion.identity);
                StartCoroutine(MoveOrbToTarget(orb, playerPos));
                Destroy(orb, 6f);
            }
            
            yield return new WaitForSeconds(3f);
        }
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