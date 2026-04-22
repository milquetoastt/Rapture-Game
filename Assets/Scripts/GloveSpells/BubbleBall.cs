using Unity.VisualScripting;
using UnityEngine;

public class BubbleBall : SpellStats
{
    public Rigidbody2D rb;
    
    public float duration = 10f;  // total travel time before destroy
    public float launchForce = 5f;
    private Vector2 dir;
    private float age;
    
    public GameObject dropletsPrefab;
    public int maxDroplets = 3;
    public float dropletRange = 30f;
    public float dropletLaunchForce = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Collider2D projectileCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = PlayerController.instance.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, playerCollider);
    }

    public override void Fire(Vector2 direction)
    {
        Debug.Log("BubbleBall Fire called");
        rb.AddForce(direction.normalized * launchForce, ForceMode2D.Impulse);
        Destroy(gameObject, duration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ledges"))
        {
            dir = collision.GetContact(0).normal;
            Debug.Log("BubbleBall Collided");
            Destroy(gameObject);
            for (int i = 0; i < maxDroplets; i++)
            {
                float angleOffset = Random.Range(-dropletRange, dropletRange);
                float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                float finalAngle = baseAngle + angleOffset;
                Vector2 newDir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                    Mathf.Sin(finalAngle * Mathf.Deg2Rad));
                GameObject droplet = Instantiate(dropletsPrefab, transform.position, Quaternion.identity);
                Rigidbody2D dropletRb = droplet.GetComponent<Rigidbody2D>();

                dropletRb.AddForce(newDir * dropletLaunchForce, ForceMode2D.Impulse);
            }
        }
    }
}
