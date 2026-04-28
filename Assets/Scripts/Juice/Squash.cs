using UnityEngine;

public class VelocitySquashStretch : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform visual;

    public float stretchAmount = 0.3f;
    public float speedNeededForMax = 12f;
    public float returnSpeed = 14f;

    private Vector3 startScale;
    private Quaternion startVisualRotation;

    void Awake()
    {
        startScale = transform.localScale;
        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();
        if (visual != null)
            startVisualRotation = visual.rotation;
    }

    void LateUpdate()
    {
        Vector2 vel = rb.linearVelocity;
        //huge thanks to youtube tutorial for this
        if (vel.sqrMagnitude < 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.deltaTime * returnSpeed);
            visual.rotation = startVisualRotation;
            return;
        }

        float speed = Mathf.Clamp01(vel.magnitude / speedNeededForMax);
        float stretch = stretchAmount * speed;
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Vector3 targetScale = new Vector3(startScale.x * (1f + stretch), startScale.y * (1f - stretch), startScale.z);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * returnSpeed);
        visual.rotation = startVisualRotation;
    }
}