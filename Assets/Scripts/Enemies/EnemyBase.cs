using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int hp = 30;
    public float stunResist = 0f;
    public DungeonRoom myRoom;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpellStats spell = collision.gameObject.GetComponent<SpellStats>();
        if (spell != null)
        {
            TakeDamage(spell.damage);
            if (spell.knockback < 0f)
                ApplyKnockback();
            if (spell.stunTarget)
                ApplyStun();
        }
    }

    public virtual void ApplyKnockback()
    {
        Debug.Log("Called EnemyBase ApplyKnockback()");
    }

    public virtual void ApplyStun()
    {
        Debug.Log("Called EnemyBase ApplyStun()");
    }
    private void TakeDamage(int damage)
    {
        hp -= damage;

    }

    private void Die()
    {
        myRoom.EnemyDied(this);
        Destroy(gameObject);
    }
}
