using UnityEngine;

public class WilloWisp : SpellStats
{
    public Rigidbody2D rb;
    public float speed = 8f;
    public float lifeTime = 2f;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    public override void CastHat(Vector2 mouseWorld)
    {
        Vector2 startPos = transform.position;
        Vector2 direction = (mouseWorld - startPos).normalized;

        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }
}