using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public int health = 100;
    public bool showSpeed = false;
    public bool isCasting = false;
    [Header("Move Settings")]
    public float moveSpeed = 5f;
    public float groundFriction = 20f;
    public float maxInputSpeed = 8f;
    private float horizontalInput;
    private Rigidbody2D rb;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public bool isGrounded;

    public float jumpForce = 10f;
    public bool canInputMove = true;

    public static PlayerController instance;

    [Header("Camera Things")]
    public CinemachineVirtualCamera vcam;
    public float normalSize = 15f;
    public float zoomedSize = 13f;
    public float zoomInSpeed = 8f;
    public float zoomOutSpeed = 5f;

    public GloveSpells gloveSpells;
    public ManaResource manaResource;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        instance = this;

        if (gloveSpells == null)
            gloveSpells = GetComponent<GloveSpells>();

        if (manaResource == null)
            manaResource = GetComponent<ManaResource>();
    }
    void Update()
    {
        //Debug.Log(heldSeconds);
        // -1 = left, +1 = right
        horizontalInput = Input.GetAxisRaw("Horizontal");

        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (showSpeed)
            Debug.Log(horizontalSpeed);

        if (Input.GetKeyDown("space") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (gloveSpells != null)
            gloveSpells.HandleInput();
        {
            bool isHolding = false;
            if (manaResource != null)
                isHolding = manaResource.isHolding;

            float targetSize = isHolding ? zoomedSize : normalSize;

            // choose speed based on direction (returning vs zooming in)
            float speed = (targetSize > vcam.m_Lens.OrthographicSize) ? zoomOutSpeed : zoomInSpeed;

            vcam.m_Lens.OrthographicSize = Mathf.MoveTowards(
                vcam.m_Lens.OrthographicSize,
                targetSize,
                Time.unscaledDeltaTime * speed
            );
        }
    }
    private void FixedUpdate()
    {
        float vx = rb.linearVelocity.x;

        if (horizontalInput != 0)
        {
            // Only apply force if we're trying to accelerate in a direction
            // and we're not already at/over the input speed cap in that direction.
            bool pushingRightAllowed = horizontalInput > 0f && vx < maxInputSpeed;
            bool pushingLeftAllowed = horizontalInput < 0f && vx > -maxInputSpeed;

            if ((pushingRightAllowed || pushingLeftAllowed) && canInputMove)
            {
                rb.AddForce(Vector2.right * horizontalInput * moveSpeed, ForceMode2D.Force);
            }
            // If not allowed, we simply don't add more force.
            // We do NOT clamp velocity, so outside forces can keep vx above the cap.
        }
        else if (isGrounded)
        {
            // Manual friction (when no input)
            Vector2 velocity = rb.linearVelocity;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                groundFriction * Time.fixedDeltaTime
            );
            rb.linearVelocity = velocity;
        }

        isGrounded = Physics2D.OverlapCircle(
        groundCheck.position,
        groundCheckRadius,
        groundLayer);

    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void Hurt(int damage)
    {
        health -= damage;
        Debug.Log("Health: " + health);
    }

    void DamageBoost()
    {

    }
}