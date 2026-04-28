using UnityEngine;

public class SpriteParticle : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;

    public float lifetime = 0.4f;

    private float timer;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        timer = lifetime;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Clamp01(timer / lifetime);
            sr.color = c;
        }

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

}