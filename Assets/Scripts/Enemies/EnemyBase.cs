using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public int hp = 30;
    public float stunResist = 0f;
    public DungeonRoom myRoom;
    public bool stunned = false;

    private Coroutine stunRoutine;
    public Rigidbody2D rb;

    protected virtual void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpellStats spell = collision.GetComponent<SpellStats>();
        if (spell != null)
        {

            TakeHit(spell.damage, spell.knockback, collision.transform.position);

            if (spell.stunTime > 0f)
            {
                if (stunRoutine != null)
                    StopCoroutine(stunRoutine);

                stunRoutine = StartCoroutine(StunCoroutine(spell.stunTime));
            }
        }
    }

    private IEnumerator StunCoroutine(float stunTime)
    {
        stunned = true;
        OnStunStart();

        yield return new WaitForSeconds(stunTime);

        stunned = false;
        OnStunEnd();
        stunRoutine = null;
    }

    protected virtual void OnStunStart()
    {
    }

    protected virtual void OnStunEnd()
    {
    }

    public virtual void TakeHit(int damage, float knockbackForce, Vector2 hitSourcePos)
    {
        hp -= damage;

        if (knockbackForce > 0f)
        {
            Vector2 knockDir = ((Vector2)transform.position - hitSourcePos).normalized;
            knockDir.y = 0f;
            knockDir.Normalize();

            ApplyKnockback(knockDir, knockbackForce);
        }

        if (hp <= 0)
            Die();
    }

    public virtual void ApplyKnockback(Vector2 direction, float force)
    {
        if (rb == null) return;

        //rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        //rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    protected virtual void Die()
    {
        // keep commented during testing
        if (myRoom != null)
        {
            myRoom.EnemyDied(this);
        }

        Destroy(gameObject);
    }
}