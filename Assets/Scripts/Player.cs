using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed = 15f;
    float horizontalMovement;
    float verticalMovement;

    [Header("Dashing")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Animator")]
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (isDashing)
        // {
        //     return;
        // }
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, verticalMovement * moveSpeed * 2 / 3);
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

    // public void Dash(InputAction.CallbackContext context)
    // {
    //     if (context.performed && canDash)
    //     {
    //         StartCoroutine(DashCoroutine());
    //     }
    // }

    // private IEnumerator DashCoroutine()
    // {
    //     canDash = false;
    //     isDashing = true;

    //     trailRenderer.emitting = true;
    //     float dashDirection = ;
    // }
}
