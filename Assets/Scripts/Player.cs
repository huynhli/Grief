using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player")]
    private Animator animator;
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed = 15f;
    float horizontalMovement;
    float verticalMovement;
    private bool isFacingRight = true;

    [Header("Dashing")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private Vector2 dashDirection;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    [Header("Damage Boss")]
    public bool enemy;

    [Header("Damage Taken")]
    public float invincibilityDuration = 1.5f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    // [Header("Camera Shake")]
    // public CinemachineImpulseSource impulseSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Shoot();
        }

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, verticalMovement * moveSpeed * 2 / 3);

        Flip();
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", horizontalMovement);
            animator.SetFloat("LastInputY", verticalMovement);
        }

        horizontalMovement = context.ReadValue<Vector2>().x;
        animator.SetFloat("InputX", horizontalMovement);

        verticalMovement = context.ReadValue<Vector2>().y;
        animator.SetFloat("InputY", verticalMovement);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;

        dashDirection = new Vector2(horizontalMovement, verticalMovement).normalized;

        rb.linearVelocity = new Vector2(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    private void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        Destroy(bullet, 2f);
    }

    // Contact damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            StartCoroutine(TakeDamage());
        }
    }

    public IEnumerator TakeDamage()
    {
        isInvincible = true;
        currentHealth -= 1;
        // animator.SetTrigger("isHurt");

        StartCoroutine(FlashPlayer());
        // if (impulseSource != null)
        // {
        //     Debug.Log("Shake!");
        //     impulseSource.GenerateImpulse();
        // }

        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;

        if (currentHealth <= 0)
        {
            // player dead -- call game over, anmimation, etc.
        }
    }

    private IEnumerator FlashPlayer()
    {
        float elapsed = 0;
        float flashInterval = 0.1f;

        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        spriteRenderer.enabled = true;
    }

}
