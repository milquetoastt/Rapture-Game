using UnityEngine;

public class Arrow : EnemyBullet
{
    public int direction;
    public float moveSpeed = 3f;
    public Rigidbody2D rb;
    public float upForce = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocityX = moveSpeed * direction;
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector2(0, upForce));
    }
}
