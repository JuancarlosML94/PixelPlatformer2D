using UnityEngine;

/// <summary>
/// Controlador de movimiento para un juego 2D de scroll lateral.
/// Incluye: movimiento con aceleración, salto con altura variable,
/// coyote time, jump buffer y giro del sprite.
///
/// Requisitos en el GameObject: Rigidbody2D + Collider2D.
/// Crea un hijo vacío "GroundCheck" en los pies del personaje
/// y asigna la capa del suelo en el inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento horizontal")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 70f;
    [SerializeField, Range(0f, 1f)] private float airControl = 0.7f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Gravedad")]
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fallGravityMultiplier = 1.6f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Detección de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        ReadInput();
        CheckGround();
        HandleTimers();
        HandleJump();
        HandleFlip();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyBetterGravity();
    }

    private void ReadInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            coyoteTimer = 0f;
        }
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleTimers()
    {
        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void ApplyHorizontalMovement()
    {
        float targetSpeed = moveInput * maxSpeed;
        float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        if (!isGrounded)
        {
            rate *= airControl;
        }

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    private void ApplyBetterGravity()
    {
        rb.gravityScale = rb.linearVelocity.y < 0f ? gravityScale * fallGravityMultiplier : gravityScale;

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    private void HandleFlip()
    {
        if (moveInput > 0f && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0f && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}