using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ladder Settings")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float maxClimbTime = 3f;
    [SerializeField] private LayerMask ladderLayer;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float horizontalInput;
    private float verticalInput;

    private bool isGrounded;
    private bool jumpRequested;

    private bool isClimbing;
    private bool isNearLadder;
    private float climbTimer;

    private bool isCrouching;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int ClimbHash = Animator.StringToHash("Climb");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int CrouchHash = Animator.StringToHash("Crouch");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;

        climbTimer = maxClimbTime;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput > 0f)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isClimbing)
        {
            isCrouching = !isCrouching;
            UpdateColliderDimensions();
        }

        HandleLadderClimb();

        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing && !isCrouching)
        {
            jumpRequested = true;
            animator.SetTrigger(JumpHash);
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(horizontalInput * moveSpeed * 0.5f, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

            if (jumpRequested)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpRequested = false;
                isGrounded = false;
            }
        }
    }

    private void UpdateAnimations()
    {
        bool isMovingHorizontally = Mathf.Abs(horizontalInput) > 0.01f;

        animator.SetBool(RunHash, isMovingHorizontally && !isClimbing && !isCrouching);
        animator.SetBool(ClimbHash, isClimbing);
        animator.SetBool(CrouchHash, isCrouching);

        if (isClimbing)
        {
            animator.speed = Mathf.Abs(verticalInput) > 0.01f ? 1f : 0f;
        }
        else
        {
            animator.speed = 1f;
        }
    }

    private void UpdateColliderDimensions()
    {
        if (isCrouching)
        {
            float targetHeight = originalColliderSize.y * (2f / 3f);
            float heightDifference = originalColliderSize.y - targetHeight;

            capsuleCollider.size = new Vector2(originalColliderSize.x, targetHeight);
            capsuleCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (heightDifference / 2f));
        }
        else
        {
            capsuleCollider.size = originalColliderSize;
            capsuleCollider.offset = originalColliderOffset;
        }
    }

    private void HandleLadderClimb()
    {
        if (isGrounded)
        {
            climbTimer = maxClimbTime;
        }

        if (isNearLadder && (Mathf.Abs(verticalInput) > 0.1f) && climbTimer > 0f && !isCrouching)
        {
            isClimbing = true;
        }

        if (isClimbing)
        {
            climbTimer -= Time.deltaTime;

            if (!isNearLadder || climbTimer <= 0f)
            {
                isClimbing = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isNearLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isNearLadder = false;
            isClimbing = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) == 0) return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.6f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }
}