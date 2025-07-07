using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class SkellyBoss : Enemy
{
    [Header("Boss")]
    public Player player;
    private Animator animator;
    [SerializeField] private bool isInvulnerable;
    private int maxHealth = 150;
    public override int MaxHealth => maxHealth;
    private string bossTitle = "BARGAIN";
    public override string BossTitle => bossTitle;
    private CompositeCollider2D collisionToToggle;
    
    // Add phase tracking
    private int currentPhase = 1;
    private Coroutine currentAttackCoroutine;

    [Header("Spawning")]
    public GameObject Tower1Prefab;
    public GameObject Tower2Prefab;
    public GameObject ClawPrefab;
    public GameObject CrystalPrefab;
    public GameObject enemyBulletPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip bossHurtSFX;
    [SerializeField] private AudioClip spawnClawSFX;
    [SerializeField] private AudioClip spawnTowerSFX;
    [SerializeField] private AudioClip spawnCrystalSFX;
    [SerializeField] private AudioClip bossThemeMusic;

    [Header("UI")]
    public WinScreen winScreen;
    public UIDocument uiDocument;
    public VisualElement bossHealthBar;

    protected override void Start()
    {
        base.Start();
        bossHealthBar = uiDocument.rootVisualElement.Q<VisualElement>("BossBar");
        bossHealthBar.style.opacity = 0f;
        collisionToToggle = GetComponent<CompositeCollider2D>();
        collisionToToggle.enabled = false;
        animator = GetComponent<Animator>();
        animator.SetBool("Dead", false);
        isInvulnerable = true;
        base.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        StartCoroutine(AttackSequence());
        // dies automatically, just overrides
    }

    IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(6f);
        bossHealthBar.style.opacity = 1f;
        StartCoroutine(PlayMusic());
        yield return StartCoroutine(AttackPhase(1, 1.25f));
    }

    IEnumerator PlayMusic()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlayLoopMusic(bossThemeMusic, player.transform, 0.2f);
    }

    public override void TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            SoundManager.instance.PlaySFXClip(bossHurtSFX, player.transform, 0.4f);
            base.TakeDamage(damage);
            StartCoroutine(flashRed());

            if (currentPhase == 1 && base.currentHealth <= maxHealth / 2)
            {
                isInvulnerable = true;
                StartCoroutine(TransitionToPhaseTwo());
            }
        }
        else
        {
            StartCoroutine(flashGreen());
        }
    }

    IEnumerator TransitionToPhaseTwo()
    {
        currentPhase = 2;
        
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
        }
        
        animator.SetInteger("AttackType", 0);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AttackPhase(2, 2f));
    }

    protected override void Die()
    {
        animator.SetBool("Dead", true);
        animator.SetInteger("AttackType", 0);
        player.enabled = false;
        player.stopMoving();
        SoundManager.instance.FadeOutLoopingMusic(5f);
        Invoke(nameof(DestroyBoss), 6f); 
    }

    private void DestroyBoss()
    {
        base.Die();
        winScreen.FadeIn();
    }

    IEnumerator flashGreen()
    {
        base.spriteRenderer.color = new Color(0.482f, 0.843f, 0.769f, 0.95f);
        yield return new WaitForSeconds(0.1f);
        base.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
    IEnumerator flashRed()
    {
        base.spriteRenderer.color = new Color(1.0f, 0.714f, 0.714f, 1.0f);
        yield return new WaitForSeconds(0.1f);
        base.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage();
        }
    }

    IEnumerator AttackPhase(int phaseNum, float animSpeed)
    {
        collisionToToggle.enabled = false;
        yield return new WaitForSeconds(1.0f); // beginning, wait
        collisionToToggle.enabled = true;
        animator.SetFloat("Speed", animSpeed);

        if (phaseNum == 1)
        {
            while (base.currentHealth > maxHealth / 2 && currentPhase == 1)
            {
                // Reset to idle and wait for idle state to finish once
                animator.SetInteger("AttackType", 0);
                int rand = UnityEngine.Random.Range(1, 3); // Choose attack before idle finishes
                currentAttackCoroutine = StartCoroutine(WaitForIdleToFinish(rand));
                yield return currentAttackCoroutine;

                // Check if we're still in phase 1 before continuing
                if (currentPhase == 1)
                {
                    currentAttackCoroutine = StartCoroutine(handleAttack(rand));
                    yield return currentAttackCoroutine;
                }
            }
        }
        else if (phaseNum == 2)
        {
            animator.SetFloat("Speed", 1f);
            animator.SetInteger("AttackType", 3);
            CameraShake.Instance.Shake(10f, 7f);

            yield return new WaitForSeconds(0.1f);

            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SkellyAttack3"))
            {
                yield return null;
            }

            // Wait for the animation to complete
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            // Now reset everything and start phase 2 proper
            animator.SetInteger("AttackType", 0);
            animator.SetFloat("Speed", 2f);
            Invoke("DisableInvulnerability", 5f);

            while (base.currentHealth > 0)
            {
                // Reset to idle and wait for idle state to finish once
                animator.SetInteger("AttackType", 0);
                int rand = UnityEngine.Random.Range(2, 4); // Choose attack before idle finishes
                currentAttackCoroutine = StartCoroutine(WaitForIdleToFinish(rand));
                yield return currentAttackCoroutine;

                currentAttackCoroutine = StartCoroutine(handleAttack(rand));
                yield return currentAttackCoroutine;
            }
            
            
        }
    }

    void DisableInvulnerability()
    {
        isInvulnerable = false;
    }

    IEnumerator WaitForIdleToFinish(int nextAttackType)
    {
        yield return null;

        // Wait until we're in the idle state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SkellyIdle"))
        {
            yield return null;
        }

        
        if (currentPhase == 2)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(RandomProjAttack());
        }
        
        // Wait until the idle state is completely finished
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        
        // Set the attack type AFTER idle completes
        animator.SetInteger("AttackType", nextAttackType);
    }

    IEnumerator RandomProjAttack()
    {
        int randomInt = Random.Range(0, 3);
        switch (randomInt)
        {
            case 0:
                // Attack 1: Semi-circle pattern at bottom of (0, 5.5)
                yield return StartCoroutine(SemiCircleAttack());
                break;
            case 1:
                // Attack 2: Two vertical lines shooting towards player
                yield return StartCoroutine(VerticalLineAttack());
                break;
            case 2:
                // Attack 3: Rectangle barrier of non-moving bullets
                yield return StartCoroutine(RectangleBarrierAttack());
                break;
            default:
                yield return null;
                break;
        }
    }

    IEnumerator SemiCircleAttack()
    {
        Vector2 center = new Vector2(0, 5.5f);
        float radius = 2f;
        int bulletCount = 16;
        
        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate angle for semi-circle (180 degrees spread)
            float angle = Mathf.PI * i / (bulletCount - 1); // 0 to PI radians
            
            // Calculate spawn position
            Vector2 spawnPos = center + new Vector2(
                Mathf.Cos(angle) * radius,
                -Mathf.Sin(angle) * radius // Negative because we want bottom semi-circle
            );
            
            // Calculate direction (outward from center)
            Vector2 direction = (spawnPos - center).normalized;
            
            // Spawn bullet
            GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
            
            // Assuming your bullet has a Rigidbody2D component
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 8f; // Adjust speed as needed
            }
            
            // Destroy bullet after some time
            Destroy(bullet, 5f);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator VerticalLineAttack()
{
    Vector2 leftStart = new Vector2(-20f, -14f);
    Vector2 rightStart = new Vector2(20f, -14f);
    int bulletsPerLine = 5;
    float verticalSpacing = 2f;
    
    GameObject[] bullets = new GameObject[10];
    int bulletIndex = 0;
    
    for (int i = 0; i < bulletsPerLine; i++)
    {
        Vector2 spawnPosL = leftStart + new Vector2(0, i * verticalSpacing);
        Vector2 spawnPosR = rightStart + new Vector2(0, i * verticalSpacing);
        
        bullets[bulletIndex] = Instantiate(enemyBulletPrefab, spawnPosL, Quaternion.identity);
        bulletIndex++;
        bullets[bulletIndex] = Instantiate(enemyBulletPrefab, spawnPosR, Quaternion.identity);
        bulletIndex++;
        
        yield return new WaitForSeconds(0.2f); // Delay between spawns
    }
    
    for (int i = 0; i < bullets.Length; i++)
    {
        if (bullets[i] != null) // Check if bullet still exists
        {
            Vector2 direction = (player.transform.position - bullets[i].transform.position).normalized;
            Rigidbody2D rb = bullets[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 12f; 
            }
            
            // Destroy bullet after some time
            Destroy(bullets[i], 4f);
        }
        yield return new WaitForSeconds(0.3f);
    }
}

    IEnumerator RectangleBarrierAttack()
    {
        float minY = -15f;
        float maxY = 0f;
        float minX = -21f;
        float maxX = 21f;
        float spacing = 2f; // Adjust spacing between bullets
        
        // Create top edge (y = maxY)
        for (float x = minX; x <= maxX; x += spacing)
        {
            Vector2 spawnPos = new Vector2(x, maxY);
            GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
            
            // Make sure the bullet doesn't move
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            
            // Destroy after some time
            Destroy(bullet, 2f);
        }
        
        // Create left edge (x = minX, excluding corners to avoid duplicates)
        for (float y = minY; y < maxY; y += spacing)
        {
            Vector2 spawnPos = new Vector2(minX, y);
            GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
            
            // Make sure the bullet doesn't move
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            
            // Destroy after some time
            Destroy(bullet, 2f);
        }
        
        // Create right edge (x = maxX, excluding corners to avoid duplicates)
        for (float y = minY; y < maxY; y += spacing)
        {
            Vector2 spawnPos = new Vector2(maxX, y);
            GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
            
            // Make sure the bullet doesn't move
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            
            // Destroy after some time
            Destroy(bullet, 2f);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator handleAttack(int atkType)
    {
        if (atkType == 1)
        {
            yield return new WaitForSeconds(3f);
            isInvulnerable = true;
            SoundManager.instance.PlaySFXClip(spawnTowerSFX, player.transform, 4f);
            float randomFloat = Random.Range(-3, 4);
            Destroy(Instantiate(Tower1Prefab, new Vector2(21f + randomFloat, 3f + randomFloat), Quaternion.identity), 10f);
            randomFloat = Random.Range(-3, 4);
            Destroy(Instantiate(Tower1Prefab, new Vector2(-21f + randomFloat, 3f + randomFloat), Quaternion.identity), 10f);
            randomFloat = Random.Range(-3, 4);
            Destroy(Instantiate(Tower1Prefab, new Vector2(21f + randomFloat, -10f + randomFloat), Quaternion.identity), 10f);
            randomFloat = Random.Range(-3, 4);
            Destroy(Instantiate(Tower1Prefab, new Vector2(-21f + randomFloat, -10f + randomFloat), Quaternion.identity), 10f);
            yield return new WaitForSeconds(4f); // Wait for rest of animation to be done
            isInvulnerable = false;
        }
        else if (atkType == 2)
        {
            // spawn two tower2 + 4 crystals
            yield return new WaitForSeconds(3f);
            isInvulnerable = true;
            SoundManager.instance.PlaySFXClip(spawnTowerSFX, player.transform, 4f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(15f, 2f), Quaternion.identity), 8f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(-15f, 2f), Quaternion.identity), 8f);
            float randomFloat = Random.Range(-10, 10);
            SoundManager.instance.PlaySFXClip(spawnCrystalSFX, player.transform, 2f);
            Destroy(Instantiate(CrystalPrefab, new Vector2(-20f + randomFloat, 4f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(-20f + randomFloat, 10f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(-20f + randomFloat, -2f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(-20f + randomFloat, -9f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(20f + randomFloat, 4f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(20f + randomFloat, 10f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(20f + randomFloat, -2f + randomFloat/2), Quaternion.identity), 7f);
            randomFloat = Random.Range(-10, 10);
            Destroy(Instantiate(CrystalPrefab, new Vector2(20f + randomFloat, -9f + randomFloat/2), Quaternion.identity), 7f);
            yield return new WaitForSeconds(4f);
            isInvulnerable = false;
        }
        else if (atkType == 3)
        {
            // spawn 2 tower1, 2 tower 2, hands 
            yield return new WaitForSeconds(2f);
            isInvulnerable = true;
            SoundManager.instance.PlaySFXClip(spawnTowerSFX, player.transform, 4f);
            Destroy(Instantiate(Tower1Prefab, new Vector2(21f, 3f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower1Prefab, new Vector2(-21f, 3f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(15f, 2f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(-15f, 2f), Quaternion.identity), 7f);
            yield return new WaitForSeconds(1f);
            ClawSpawn(0);
            yield return new WaitForSeconds(2f);
            isInvulnerable = false;
            ClawSpawn(1);
            yield return new WaitForSeconds(2f);
        }

        animator.SetInteger("AttackType", 0);
    }

    void ClawSpawn(int leftRight)
    {
        SoundManager.instance.PlaySFXClip(spawnClawSFX, player.transform, 4f);
        float leftSpawn = -38f;
        float rightSpawn = 38f;
        if (leftRight == 0)
        {
            GameObject tempClaw1 = Instantiate(ClawPrefab, new Vector2(leftSpawn, 16f), Quaternion.identity);
            GameObject tempClaw2 = Instantiate(ClawPrefab, new Vector2(leftSpawn, 10.5f), Quaternion.identity);
            GameObject tempClaw3 = Instantiate(ClawPrefab, new Vector2(leftSpawn, 4f), Quaternion.identity);
            GameObject tempClaw4 = Instantiate(ClawPrefab, new Vector2(leftSpawn, -2f), Quaternion.identity);
            GameObject tempClaw5 = Instantiate(ClawPrefab, new Vector2(leftSpawn, -8.5f), Quaternion.identity);
            GameObject tempClaw6 = Instantiate(ClawPrefab, new Vector2(leftSpawn, -14f), Quaternion.identity);
        }
        else
        {
            GameObject tempClaw1 = Instantiate(ClawPrefab, new Vector2(rightSpawn, 13.5f), Quaternion.identity);
            GameObject tempClaw2 = Instantiate(ClawPrefab, new Vector2(rightSpawn, 7f), Quaternion.identity);
            GameObject tempClaw3 = Instantiate(ClawPrefab, new Vector2(rightSpawn, 1.5f), Quaternion.identity);
            GameObject tempClaw4 = Instantiate(ClawPrefab, new Vector2(rightSpawn, -5f), Quaternion.identity);
            GameObject tempClaw5 = Instantiate(ClawPrefab, new Vector2(rightSpawn, -10.5f), Quaternion.identity);
            tempClaw1.transform.localScale = new Vector3(-1f, 1f, 1f);
            tempClaw2.transform.localScale = new Vector3(-1f, 1f, 1f);
            tempClaw3.transform.localScale = new Vector3(-1f, 1f, 1f);
            tempClaw4.transform.localScale = new Vector3(-1f, 1f, 1f);
            tempClaw5.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}