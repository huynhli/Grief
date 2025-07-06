using UnityEngine;
using System.Collections;

public class SkellyBoss : Enemy
{
    [Header("Boss")]
    public Player player;
    private Animator animator;
    [SerializeField] private bool isInvulnerable;

    [Header("Spawning")]
    public GameObject Tower1Prefab;
    public GameObject Tower2Prefab;
    public GameObject ClawPrefab;
    public GameObject CrystalPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        animator.SetBool("Dead", false);
        isInvulnerable = true;
        base.spriteRenderer.color = new Color(0.482f, 0.843f, 0.769f, 0.95f);

        StartCoroutine(AttackSequence());
        // dies automatically, just overrides
    }

    IEnumerator AttackSequence()
    {
        yield return StartCoroutine(AttackPhase(1, 1f));
        yield return StartCoroutine(AttackPhase(2, 1.25f));
    }

    protected override void Die()
    {
        animator.SetBool("Dead", true);
        animator.SetInteger("AttackType", 0);
        Invoke(nameof(DestroyBoss), 6f); 
    }

    private void DestroyBoss() 
    {
        base.Die();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (base.currentHealth < base.MaxHealth / 2)
        if (!isInvulnerable)
        {
            base.TakeDamage(damage);
        }
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
        yield return new WaitForSeconds(1.0f); // beginning, wait
        animator.SetFloat("Speed", animSpeed);
        if (phaseNum == 1)
        {
            while (base.currentHealth > base.MaxHealth / 2)
            {
                // Reset to idle and wait for idle state to finish once
                animator.SetInteger("AttackType", 0);
                int rand = UnityEngine.Random.Range(1, 3); // Choose attack before idle finishes
                yield return StartCoroutine(WaitForIdleToFinish(rand));
                
                yield return StartCoroutine(handleAttack(rand));

            }
        }
        else if (phaseNum == 2)
        {
            while (base.currentHealth > 0)
            {
                // Reset to idle and wait for idle state to finish once
                animator.SetInteger("AttackType", 0);
                int rand = UnityEngine.Random.Range(1, 4); // Choose attack before idle finishes
                yield return StartCoroutine(WaitForIdleToFinish(rand));
                
                yield return StartCoroutine(handleAttack(rand));     

            }
        }

    }

    IEnumerator WaitForIdleToFinish(int nextAttackType)
    {
        yield return null;

        // Wait until we're in the idle state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SkellyIdle"))
        {
            yield return null;
        }

        // Wait until the idle state is completely finished
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        
        // Set the attack type AFTER idle completes
        animator.SetInteger("AttackType", nextAttackType);
    }

    IEnumerator handleAttack(int atkType)
    {

        if (atkType == 1)
        {
            yield return new WaitForSeconds(3f);
            isInvulnerable = true;
            base.spriteRenderer.color = new Color(0.482f, 0.843f, 0.769f, 0.95f);
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
            base.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (atkType == 2)
        {
            // spawn two tower2 + 4 crystals
            yield return new WaitForSeconds(3f);
            isInvulnerable = true;
            base.spriteRenderer.color = new Color(0.482f, 0.843f, 0.769f, 0.95f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(15f, 2f), Quaternion.identity), 8f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(-15f, 2f), Quaternion.identity), 8f);
            float randomFloat = Random.Range(-10, 10);
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
            base.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (atkType == 3)
        {
            // spawn 2 tower1, 2 tower 2, hands 
            yield return new WaitForSeconds(2f);
            isInvulnerable = true;
            base.spriteRenderer.color = new Color(0.482f, 0.843f, 0.769f, 0.95f);
            Destroy(Instantiate(Tower1Prefab, new Vector2(21f, 3f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower1Prefab, new Vector2(-21f, 3f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(15f, 2f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower2Prefab, new Vector2(-15f, 2f), Quaternion.identity), 7f);
            yield return new WaitForSeconds(1f);
            ClawSpawn(0);
            yield return new WaitForSeconds(2f);
            isInvulnerable = false;
            base.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            ClawSpawn(1);
            yield return new WaitForSeconds(2f);
        }

        animator.SetInteger("AttackType", 0);
    }

    void ClawSpawn(int leftRight)
    {
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
