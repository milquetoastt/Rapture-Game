using UnityEngine;

public class Downburst : SpellStats
{
    public Rigidbody2D rb;
    private Animator anim;
    public Collider2D downBurstCollider;
    public float duration = 1.0f;
    public float speed = .5f;
    private Vector2 dir;
    private float age;
    
    public float upForce = 50f;

    public override void CastMantle(GameObject caster, Vector2 direction)
    {
        Rigidbody2D rbP = PlayerController.instance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rbP.linearVelocityY = 0;
            rbP.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
            PlayerController.instance.isCasting = false;
        }
        
    }
    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, duration);
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.down * speed;
    }
    
    public void TurnOnCollider()
    {
        downBurstCollider.enabled = true;
    }
    
    public void DestroyMarker()
    {
        Destroy(gameObject);
    }
}
