using UnityEngine;

public class WindBurst : SpellStats
{
    public Rigidbody2D rb;
    
    public float duration = 1.0f;  // total travel time before destroy
    public float startSpeed = 28f;
    public float endSpeed = 2f;
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector2 dir;
    private float age;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    public override void Fire(Vector2 direction)
    {
        dir = direction.normalized;
        age = 0f;
        Destroy(gameObject, duration);
    }

    void FixedUpdate()
    {
        age += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(age / duration);
        float k = speedCurve.Evaluate(t);
        float speed = Mathf.Lerp(endSpeed, startSpeed, k);

        rb.linearVelocity = dir * speed;
    }
}
