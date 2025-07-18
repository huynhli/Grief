using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player")]
    private Animator animator;
    public int maxHealth = 5;
    public int currentHealth;
    private Transform playerTransform;
    public bool isDead = false; // Added isDead flag

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
    public float shootCooldown = 0.15f; // Time between shots in seconds
    private float lastShootTime = 0f;
    private bool isHoldingShooting = false; // Track if mouse is being held

    [Header("Damage Taken")]
    public float invincibilityDuration = 1.5f;
    public bool isInvincible;
    private SpriteRenderer spriteRenderer;

    [Header("SFX")]
    [SerializeField] private AudioClip dashSFX;
    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip playerHurtSFX;


    [Header("UI")]
    public LevelUIManager levelUIManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isInvincible = false;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || isDashing)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isHoldingShooting = true;
            if (Time.time - lastShootTime >= shootCooldown)
            {
                Shoot();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingShooting = false;
        }

        if (isHoldingShooting && Time.time - lastShootTime >= shootCooldown)
        {
            Shoot();
        }

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, verticalMovement * moveSpeed * 2 / 3);

        Flip();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isDead) return;
        
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
            Vector3 ls = playerTransform.localScale;
            ls.x *= -1f;
            playerTransform.localScale = ls;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (isDead) return;
        
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        isInvincible = true;
        trailRenderer.emitting = true;

        SoundManager.instance.PlaySFXClip(dashSFX, 4f);

        dashDirection = new Vector2(horizontalMovement, verticalMovement).normalized;

        rb.linearVelocity = new Vector2(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        trailRenderer.emitting = false;
        isInvincible = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    private void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 shootDirection = (mousePosition - playerTransform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, playerTransform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        SoundManager.instance.PlaySFXClip(fireSFX, 0.2f);

        lastShootTime = Time.time;
    }

    public void TakeDamage()
    {
        if (!isInvincible)
        {
            StartCoroutine(TakeDamageRoutine());
        }
    }

    public IEnumerator TakeDamageRoutine()
    {
        isInvincible = true;
        currentHealth -= 1;
        SoundManager.instance.PlaySFXClip(playerHurtSFX, 1f);
        StartCoroutine(FlashPlayer());

        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;

        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            isInvincible = true;
            rb.linearVelocity = Vector2.zero;

            SoundManager.instance.PlaySFXClip(deathSFX, 4f);
            levelUIManager.ShowDeathScreen();
            
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

    public void stopMoving()
    {
        playerTransform.position = Vector3.zero;
        isInvincible = true;
        rb.linearVelocity = Vector2.zero;
    }
}