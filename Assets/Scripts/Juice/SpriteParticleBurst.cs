using UnityEngine;

public class SpriteParticleBurst : MonoBehaviour
{
    public GameObject particlePrefab;
    public Sprite[] possibleSprites;

    [Header("burst")]
    public float forceMin = 2f;
    public float forceMax = 5f;
    public float spreadAngle = 90f;
    public float lifetime = 0.4f;

    public void Burst(Vector2 origin, Vector2 mainDirection, Sprite[] sprites, int spawnAmount)
    {
        if (particlePrefab == null) return;
        if (sprites == null || sprites.Length == 0) return;

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject obj = Instantiate(particlePrefab, origin, Quaternion.identity);
            obj.GetComponent<SpriteParticle>().lifetime = lifetime; 
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            if (sr != null)
            {
                sr.sprite = sprites[Random.Range(0, sprites.Length)];
            }

            Vector2 dir = RandomDirectionAround(mainDirection.normalized, spreadAngle);
            float force = Random.Range(forceMin, forceMax);

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(dir * force, ForceMode2D.Impulse);
            }

            Destroy(obj, lifetime);
        }
    }

    private Vector2 RandomDirectionAround(Vector2 direction, float angle)
    {
        float randomAngle = Random.Range(-angle * 0.5f, angle * 0.5f); 
        Vector2 rotated = Quaternion.Euler(0f, 0f, randomAngle) * direction;
        return rotated.normalized;
    }
}