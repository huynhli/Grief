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

    [Header("Damage Boss")]
    public bool enemy;

    [Header("Damage Taken")]
    public float invincibilityDuration = 1.5f;
    public bool isInvincible;
    private SpriteRenderer spriteRenderer;

    [Header("SFX")]
    [SerializeField] private AudioClip dashSFX;
    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip playerHurtSFX;
    [SerializeField] private AudioClip buttonHoverClip;


    [Header("UI")]
    public UIDocument uiDocument;
    public Label youDied;
    public Button mainMenuButton;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        youDied = uiDocument.rootVisualElement.Q<Label>("Dead");
        youDied.style.display = DisplayStyle.None;
        mainMenuButton = uiDocument.rootVisualElement.Q<Button>("BackButton");
        mainMenuButton.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        mainMenuButton.clicked += MainMenuButtonClicked;
        mainMenuButton.style.display = DisplayStyle.None;
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isInvincible = false;

        currentHealth = maxHealth;
    }

    void MainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        SoundManager.instance.PlaySFXClip(buttonHoverClip, playerTransform, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        // Check for mouse button down (start holding)
        if (Input.GetMouseButtonDown(0))
        {
            isHoldingShooting = true;
            // Only fire immediately if cooldown allows
            if (Time.time - lastShootTime >= shootCooldown)
            {
                Shoot();
            }
        }

        // Check for mouse button up (stop holding)
        if (Input.GetMouseButtonUp(0))
        {
            isHoldingShooting = false;
        }

        // Continue shooting while holding and cooldown allows
        if (isHoldingShooting && Time.time - lastShootTime >= shootCooldown)
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
            Vector3 ls = playerTransform.localScale;
            ls.x *= -1f;
            playerTransform.localScale = ls;
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
        isInvincible = true;
        trailRenderer.emitting = true;

        SoundManager.instance.PlaySFXClip(dashSFX, playerTransform, 4f);

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
        SoundManager.instance.PlaySFXClip(fireSFX, playerTransform, 10f);

        // Update the last shoot time
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
        SoundManager.instance.PlaySFXClip(playerHurtSFX, transform, 1f);
        StartCoroutine(FlashPlayer());

        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;

        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            isInvincible = true;
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(2f);
            SoundManager.instance.PlaySFXClip(deathSFX, playerTransform, 4f);
            // player dead -- call game over, anmimation, etc.
            yield return new WaitForSeconds(2f);

            float duration = 1.5f;
            float elapsed = 0f;
            float mainMenuStartTime = duration * 0.5f; // Start at 50% through
            float mainMenuElapsed = 0f;
            float mainMenuDuration = 0.75f; // Duration for main menu fade

            youDied.style.display = DisplayStyle.Flex;
            mainMenuButton.style.display = DisplayStyle.Flex;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / duration;
                float opacity = Mathf.Lerp(0f, 1f, percent);
                youDied.style.opacity = opacity;

                if (elapsed >= mainMenuStartTime)
                {
                    mainMenuElapsed += Time.deltaTime;
                    float mainMenuPercent = Mathf.Clamp01(mainMenuElapsed / mainMenuDuration);
                    float mainMenuOpacity = Mathf.Lerp(0f, 1f, mainMenuPercent);
                    mainMenuButton.style.opacity = mainMenuOpacity;

                }

                yield return null;
            }
            
            
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