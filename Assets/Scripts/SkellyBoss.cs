using UnityEngine;
using System.Collections;

public class SkellyBoss : Enemy
{
    [Header("Boss")]
    public Player player;
    private Animator animator;
    private bool phaseTwo;

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

        StartCoroutine(AttackSequence());
    }

    protected override void Die()
    {
        animator.SetBool("Dead", true);
        Invoke(nameof(DestroyBoss), 3.5f); // Changed this line
    }

    private void DestroyBoss() // Add this method
    {
        base.Die();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (base.currentHealth < base.MaxHealth / 2)
        {
            phaseTwo = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage();
        }
    }

    IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(1.5f);
        while (base.currentHealth > 0)
        {
            // Reset to idle and wait for idle state to finish
            animator.SetInteger("AttackType", 0);
            yield return StartCoroutine(WaitForIdleToFinish());

            int rand = UnityEngine.Random.Range(1, phaseTwo ? 3 : 2);
            yield return StartCoroutine(handleAttack(rand)); // Wait for attack to complete
            
        }
    }

    IEnumerator WaitForIdleToFinish()
    {
        // Wait one frame to ensure transition starts
        yield return null;
        
        // Wait until we're in the idle state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SkellyIdle"))
        {
            yield return null;
        }
        
        // Wait until the idle state finishes (normalized time >= 1.0)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }

    IEnumerator handleAttack(int atkType)
    {
        animator.SetInteger("AttackType", atkType);
        
        if (atkType == 1)
        {
            yield return new WaitForSeconds(3f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(21f, 3f, 0f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(-21f, 3f, 0f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(21f, -2f, 0f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(-21f, -2f, 0f), Quaternion.identity), 7f);
            yield return new WaitForSeconds(4f); // Wait for rest of animation to be done
        }
        else if (atkType == 2)
        {
            yield return new WaitForSeconds(3f);
            Destroy(Instantiate(Tower2Prefab, new Vector3(15f, 2f, 0f), Quaternion.identity), 7f);
            Destroy(Instantiate(Tower2Prefab, new Vector3(-15f, 2f, 0f), Quaternion.identity), 7f);
            yield return new WaitForSeconds(4f);
        }
        else if (atkType == 3)
        {
            yield return new WaitForSeconds(3f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(21f, 3f, 0f), Quaternion.identity), 7f);
            yield return new WaitForSeconds(4f);
        }
        
        animator.SetInteger("AttackType", 0);
    }  
}
