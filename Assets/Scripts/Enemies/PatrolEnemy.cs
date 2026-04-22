using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int direction = 1;

    public Transform wallCheck;
    public Transform groundCheck;
    public float wallCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public bool stunned = false;
    public bool doingSomethingElse = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (doingSomethingElse || stunned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (wallCheck == null || groundCheck == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        bool wallAhead = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            wallCheckDistance,
            groundLayer
        );

        bool groundAhead = Physics2D.OverlapCircle(
            groundCheck.position,
            0.1f,
            groundLayer
        );

        if (wallAhead || !groundAhead)
            TurnAround();

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    public void TurnAround()
    {
        direction *= -1;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
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
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}