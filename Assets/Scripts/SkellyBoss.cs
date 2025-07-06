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

    protected override void TakeDamage(int damage)
    {
        base.TakeDamage();
        if (base.currentHealth < base.maxHealth / 2)
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
            int rand = UnityEngine.Random.Range(1, phaseTwo ? 5 : 4);
            animator.SetInteger("AttackType", rand);
            StartCoroutine(handleAttack(rand));
            yield return new WaitForSeconds(6.5f); // duration of atk animations
        }
    }

    IEnumerator handleAttack(int atkType)
    {
        // spawn 4 of tower 1 for 10 seconds
        if (atkType == 1)
        {
            Destroy(Instantiate(Tower1Prefab, new Vector3(21f, 3f, 0f), Quaternion.identity), 10f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(-21f, 3f, 0f), Quaternion.identity), 10f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(21f, -2f, 0f), Quaternion.identity), 10f);
            Destroy(Instantiate(Tower1Prefab, new Vector3(-21f, -2f, 0f), Quaternion.identity), 10f);
            yield return new WaitForSeconds(10f);
        }
        // spawn 2 of tower 2 for 10 seconds + broken trees
        else if (atkType == 2)
        {
            Destroy(Instantiate(Tower2Prefab, new Vector3(15f, 2f, 0f), Quaternion.identity), 10f);
            Destroy(Instantiate(Tower2Prefab, new Vector3(-15f, 2f, 0f), Quaternion.identity), 10f);
            yield return new WaitForSeconds(10f);
        }
        // spawn 4 of each tower type + moving hands (waves)
        else
        {
            Destroy(Instantiate(Tower1Prefab, new Vector3(21f, 3f, 0f), Quaternion.identity), 10f);
            yield return new WaitForSeconds(10f);
        }
    }     
}
