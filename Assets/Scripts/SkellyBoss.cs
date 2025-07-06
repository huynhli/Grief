using UnityEngine;
using System.Collections;

public class SkellyBoss : Enemy
{
    [Header("Boss")]
    public Player player;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
            int rand = UnityEngine.Random.Range(1, 4);
            animator.SetInteger("AttackType", rand);
            yield return new WaitForSeconds(6.5f); // duration of atk animations
        }
    }
}
