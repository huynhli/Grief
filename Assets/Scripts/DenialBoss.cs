using UnityEngine;
using System.Collections;
using Unity.Collections;

public class DenialBoss : Enemy
{
    [Header("Boss Phases")]
    public Player player;
    [SerializeField] private Transform bossTransform;
    public override int MaxHealth { get; set; } = 300;
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
        if (base.currentHealth < base.MaxHealth / 2 && !phaseTwo)
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

            animator.SetInteger("AttackType", rand);
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
                yield return StartCoroutine(BulletBlockAttack());
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
    IEnumerator BulletBlockAttack()
    {
        // Spawn bullet blockers in strategic positions
        GameObject blocker1 = Instantiate(bulletBlockerPrefab, new Vector3(5f, 2f, 0f), Quaternion.identity);
        GameObject blocker2 = Instantiate(bulletBlockerPrefab, new Vector3(-5f, 2f, 0f), Quaternion.identity);
        GameObject blocker3 = Instantiate(bulletBlockerPrefab, new Vector3(0f, -2f, 0f), Quaternion.identity);

        // Destroy blockers after duration
        Destroy(blocker1, 8f);
        Destroy(blocker2, 8f);
        Destroy(blocker3, 8f);

        yield return new WaitForSeconds(8f);
    }

    // Creates a wave of denial energy
    IEnumerator DenialWaveAttack()
    {
        // Implementation depends on your wave attack prefab
        // This is a placeholder for your specific attack pattern
        yield return new WaitForSeconds(3f);
    }

    // Bullet hell attack
    IEnumerator OrbsAttack()
    {
        GameObject Orb1 = Instantiate(bullet, new Vector3(bossTransform.position.x, bossTransform.position.y, 0f), Quaternion.identity);
        // Orbs accelerate towards player and attack
        // Implementation depends on your movement system
        yield return new WaitForSeconds(2f);
    }

    // Phase 2 ultimate attack - maximum denial
    IEnumerator UltimateBlockAttack()
    {
        // Create a field of bullet blockers
        for (int i = 0; i < 6; i++)
        {
            Vector3 spawnPos = new Vector3(
                UnityEngine.Random.Range(-10f, 10f),
                UnityEngine.Random.Range(-5f, 5f),
                0f
            );
            GameObject blocker = Instantiate(bulletBlockerPrefab, spawnPos, Quaternion.identity);
            Destroy(blocker, 10f);
        }

        yield return new WaitForSeconds(10f);
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