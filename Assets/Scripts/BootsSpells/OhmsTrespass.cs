using UnityEngine;

public class OhmmTresspass : SpellStats
{
    private Rigidbody2D rb;
    private float charge = 0f;

    public float maxCharge = 2f;
    public float chargeRate = 1f;
    public float teleportDistance = 10f;
    public float moveSlowMultiplier = 0.3f;

    [Header("Indicator")]
    public GameObject teleportIndicatorPrefab;
    private GameObject teleportIndicatorInstance;

    private Vector2 lastValidDirection = Vector2.right;

    private bool wasGrounded;
    private bool slowMovement;
    
    [Header("hitbox")]
    public GameObject hitboxPrefab;
    public float trailWidth = 1f;
    public float trailLength = 1f;

    private void Start()
    {
        if (PlayerController.instance != null)
            rb = PlayerController.instance.GetComponent<Rigidbody2D>();
    }

    public override void CastBoot(Vector2 direction)
    {
        if (rb == null && PlayerController.instance != null)
            rb = PlayerController.instance.GetComponent<Rigidbody2D>();

        charge = 0f;

        slowMovement = PlayerController.instance.isGrounded;
        wasGrounded = PlayerController.instance.isGrounded;

        if (direction.sqrMagnitude > 0.01f)
            lastValidDirection = direction.normalized;

        CreateIndicator();
        UpdateIndicatorPosition(direction);
    }

    public override void HoldBoot(Vector2 direction)
    {
        if (rb == null) return;

        charge += Time.deltaTime * chargeRate;
        charge = Mathf.Clamp(charge, 0f, maxCharge);

        if (PlayerController.instance.isGrounded)
        {
            slowMovement = true;
        }

        if (slowMovement)
        { 
            Vector2 vel = rb.linearVelocity;
            float maxSpeed = PlayerController.instance.moveSpeed * moveSlowMultiplier;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            rb.linearVelocity = vel;
        }

        wasGrounded = PlayerController.instance.isGrounded;

        if (direction.sqrMagnitude > 0.01f)
            lastValidDirection = direction.normalized;

        UpdateIndicatorPosition(direction);
    }

    public override void EndBoot(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
            lastValidDirection = direction.normalized;

        float chargePercent = charge / maxCharge;
        float distance = teleportDistance * chargePercent;
        Vector2 dir = lastValidDirection;
        Vector2 startPos = PlayerController.instance.transform.position;
        PlayerController.instance.transform.position += (Vector3)(dir * distance);
        Vector2 endPos = PlayerController.instance.transform.position;
        SpawnHitbox(startPos, endPos);
        DestroyIndicator();

        Debug.Log("trespassed with : " + chargePercent + "%");

        charge = 0f;
        slowMovement = false;
        wasGrounded = false;

        Destroy(gameObject, 0.1f);
    }

    private void SpawnHitbox(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;
        float trailLength = delta.magnitude;
        Vector2 midpoint = (start + end) / 2f;
        GameObject hitbox = Instantiate(hitboxPrefab, midpoint, Quaternion.identity);
        hitbox.transform.localScale = new Vector3(trailLength, trailWidth, 1f);
        Destroy(hitbox,.1f);
    }
    private void CreateIndicator()
    {
        if (teleportIndicatorPrefab == null) return;

        if (teleportIndicatorInstance == null)
        {
            teleportIndicatorInstance = Instantiate(teleportIndicatorPrefab);
        }

        teleportIndicatorInstance.SetActive(true);
    }

    private void UpdateIndicatorPosition(Vector2 direction)
    { 
        Vector2 dir = direction.sqrMagnitude > 0.01f ? direction.normalized : lastValidDirection;
        float chargePercent = charge / maxCharge;
        float distance = teleportDistance * chargePercent;

        Vector3 targetPos = PlayerController.instance.transform.position + (Vector3)(dir * distance);
        teleportIndicatorInstance.transform.position = targetPos;
    }

    private void DestroyIndicator()
    {
        Destroy(teleportIndicatorInstance);
        teleportIndicatorInstance = null;
    }
}