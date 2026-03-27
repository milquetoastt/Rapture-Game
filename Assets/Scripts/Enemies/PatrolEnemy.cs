using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int direction = 1;

    [Header("Checks")]
    public Transform wallCheck;
    public Transform groundCheck;
    public float wallCheckDistance = 0.1f;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool canMove = true;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        bool wallAhead = Physics2D.Raycast(
            transform.position,
            Vector2.right * direction,
            wallCheckDistance,
            groundLayer
        );

        bool groundAhead = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
        if (wallAhead)
        {
            Debug.Log("wall found!");
        }
        if (wallAhead || !groundAhead)
        {
            Debug.Log("wall: " + wallAhead + " | ground: " + groundAhead);
            TurnAround();
        }

        if (canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void TurnAround()
    {
        Debug.Log("turned around");
        direction *= -1;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = Vector3.right * direction * wallCheckDistance;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
