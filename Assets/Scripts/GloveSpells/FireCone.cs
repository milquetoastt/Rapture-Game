using UnityEngine;

public class FireCone : SpellStats
{
    public Rigidbody2D rb;
    
    public float duration = 1.0f;  // total travel time before destroy
    public float speed = .5f;
    private Vector2 dir;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    public override void Fire(Vector2 direction)
    {
        dir = direction.normalized;
        Debug.Log("fired in fire cone");
        Destroy(gameObject, duration);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = dir * speed;
    }
}
