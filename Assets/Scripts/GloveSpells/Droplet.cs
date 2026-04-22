using UnityEngine;

public class Droplet : MonoBehaviour
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb == null) return;

        Vector2 vel = rb.linearVelocity;

        if (vel.sqrMagnitude > 0.01f) // avoid jitter when almost stopped
        {
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Spell")
        {
            Debug.Log("Droplet collided with spell");
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
