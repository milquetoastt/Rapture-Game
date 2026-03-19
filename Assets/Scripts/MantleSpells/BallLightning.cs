using UnityEngine;

public class BallLightning : SpellStats
{
    public float moveSpeed = 12f;
    public float moveDuration = 0.35f;

    private Rigidbody2D rb;
    private Vector2 lockedDirection;
    private float moveTimer;
    private bool isLockedMovement;

    private float originalGravity;

    public override void CastMantle(GameObject caster, Vector2 direction)
    {
        rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        transform.SetParent(PlayerController.instance.transform);

        Vector2 currentVelocity = rb.linearVelocity;

        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            if (currentVelocity.y < 0f)
            {
                float horizontal = Mathf.Sign(currentVelocity.x);

                if (Mathf.Abs(currentVelocity.x) < 0.01f)
                    horizontal = Mathf.Sign(direction.x);

                lockedDirection = new Vector2(horizontal, 0f);
            }
            else
            {
                lockedDirection = currentVelocity.normalized;
            }
        }
        else
        {
            lockedDirection = direction.normalized;
        }
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        PlayerController.instance.isCasting = false;
        moveTimer = moveDuration;
        isLockedMovement = true;
        PlayerController.instance.canInputMove = false;
    }

    private void FixedUpdate()
    {
        if (!isLockedMovement || rb == null)
            return;

        rb.linearVelocity = lockedDirection * moveSpeed;

        moveTimer -= Time.fixedDeltaTime;

        if (moveTimer <= 0f)
            EndLockedMovement();
    }

    private void EndLockedMovement()
    {
        isLockedMovement = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;

        PlayerController.instance.canInputMove = true;
        Destroy(gameObject, 0.1f);
    }
}