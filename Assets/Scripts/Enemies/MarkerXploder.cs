using UnityEngine;
public class MarkerXploder : EnemyBullet
{
    private Animator anim;
    public Collider2D explosionCollider;
    public float lifetime = 5f;
    private float timer;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timer > lifetime)
        {
            anim.SetTrigger("FizzleOut");
        }
    }
    
    public void Detonate()
    {
        Debug.Log("detonating mark");
        anim.SetTrigger("Detonate");
    }

    public void TurnOnCollider()
    {
        explosionCollider.enabled = true;
    }
    
    public void DestroyMarker()
    {
        Destroy(gameObject);
    }
}
