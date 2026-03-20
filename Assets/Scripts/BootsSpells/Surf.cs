using UnityEngine;

public class Surf : SpellStats
{
    private Rigidbody2D playerRb;
    private Rigidbody2D rb;

    private float originalMoveSpeed;
    private float originalMaxInputSpeed;
    private float originalGroundFriction;

    private float surfDirection;

    [Header("Movement Boost")]
    public float speedBoost = 5f;
    public float maxSpeedBoost = 5f;
    public float frictionDuringSurf = 5f;

    [Header("Attach")]
    public Vector2 attachOffset = new Vector2(1.5f, -0.5f);

    [Header("Detach")]
    public float launchSpeed = 12f;
    public float detachedLifetime = 0.4f;

    [Header("Hover")]
    public float hoverHeight = 0.75f;
    public float rayLength = 2f;
    public float hoverForce = 20f;
    public float hoverDamping = 8f;
    public LayerMask groundLayer;

    public override void CastBoot(Vector2 direction)
    {
        playerRb = PlayerController.instance.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();

        originalMoveSpeed = PlayerController.instance.moveSpeed;
        originalMaxInputSpeed = PlayerController.instance.maxInputSpeed;
        originalGroundFriction = PlayerController.instance.groundFriction;

        PlayerController.instance.moveSpeed += speedBoost;
        PlayerController.instance.maxInputSpeed += maxSpeedBoost;
        PlayerController.instance.groundFriction = frictionDuringSurf;

        surfDirection = Mathf.Sign(playerRb.linearVelocity.x);
        if (surfDirection == 0f)
            surfDirection = Mathf.Sign(direction.x);
        if (surfDirection == 0f)
            surfDirection = 1f;

        transform.SetParent(PlayerController.instance.transform);
        transform.localPosition = new Vector3(attachOffset.x * surfDirection, attachOffset.y, 0f);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * surfDirection;
        transform.localScale = scale;

        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
    }

    public override void HoldBoot(Vector2 direction)
    {
        Vector2 origin = playerRb.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);

        if (hit.collider == null) return;

        float currentHeight = hit.distance;
        float heightError = hoverHeight - currentHeight;
        float lift = (heightError * hoverForce) - (playerRb.linearVelocity.y * hoverDamping);

        playerRb.AddForce(Vector2.up * lift, ForceMode2D.Force);
    }

    public override void EndBoot(Vector2 direction)
    {
        PlayerController.instance.moveSpeed = originalMoveSpeed;
        PlayerController.instance.maxInputSpeed = originalMaxInputSpeed;
        PlayerController.instance.groundFriction = originalGroundFriction;

        transform.SetParent(null);

        rb.simulated = true;
        rb.linearVelocity = new Vector2(surfDirection * launchSpeed, 0f);

        Destroy(gameObject, detachedLifetime);
    }
}