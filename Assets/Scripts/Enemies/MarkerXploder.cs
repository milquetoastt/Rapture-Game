using UnityEngine;
public class MarkerXploder : EnemyBullet
{
    private Animator anim;
    public Collider2D explosionCollider;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
