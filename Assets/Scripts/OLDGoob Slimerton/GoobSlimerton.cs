using System.Collections;
using UnityEngine;

public class GoobSlimerton : MonoBehaviour
{
    public float moveSpeed = 4f;
    public int health = 30;
    public GameObject bulletPrefab;
    private Transform player;
    public Transform firePoint;
    public float bulletSpeed = 3f;
    public float bulletCooldown = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        while (true)
        {
            GameObject projectile = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.position - firePoint.position).normalized;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.linearVelocity = direction * bulletSpeed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            yield return new WaitForSeconds(bulletCooldown);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        SpellStats spellStats = other.GetComponent<SpellStats>();
        if (spellStats != null)
        {
            Debug.Log("hit by spell");
            health -= spellStats.damage;
        }
    }
}
