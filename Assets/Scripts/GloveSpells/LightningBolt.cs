using UnityEngine;

public class LightningBolt : SpellStats
{
    public Rigidbody2D rb;

    
    public float duration = 1.0f;  // total travel time before destroy

    private Vector2 dir;

    

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    public override void Fire(Vector2 direction)
    {
        dir = direction.normalized;
        Destroy(gameObject, duration);
    }

    void FixedUpdate()
    {
        
    }
}
