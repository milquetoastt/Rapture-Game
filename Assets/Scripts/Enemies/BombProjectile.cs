using System;
using System.Collections;
using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float explodeDelay = 2f;
    private float timer;
    public float killExplosionDelay;
    public Rigidbody2D rb;
    public bool exploded = false;
    
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= explodeDelay && !exploded)
        {
            exploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        rb.linearVelocity = Vector3.zero;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, killExplosionDelay);
        Destroy(gameObject, killExplosionDelay);
    }
}