using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("stats")]
    public int hp = 30;
    public bool canBeStunned = true;
    public DungeonRoom myRoom;
    public bool stunned = false;
    public int experience = 1;

    protected Coroutine stunRoutine;
    public Rigidbody2D rb;

    protected virtual void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        SpellStats spell = collision.GetComponent<SpellStats>();

        if (spell != null)
        {
            TakeHit(spell.damage, spell.knockback, collision.transform.position);

            // if hit by rain or jetpack, ignore the hit juice
            CloudRain rain = collision.GetComponent<CloudRain>();
            Jetpack jetpack = collision.GetComponent<Jetpack>();
            Droplet droplet = collision.GetComponent<Droplet>();

            if (rain != null || jetpack != null || droplet != null)
            {
                Debug.Log("hit by rain");
            }
            else
            {
                if (JuiceManager.instance != null)
                {
                    Vector2 location = new Vector2(transform.position.x, transform.position.y);
                    JuiceManager.instance.HitJuice(location, spell.damage);
                    
                }
            }

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
        if (!canBeStunned)
            yield break;

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

        if (knockbackForce > 0f && rb != null)
        {
            Vector2 knockDir = ((Vector2)transform.position - hitSourcePos).normalized;
            knockDir.y = 0f;
            knockDir.Normalize();

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (hp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (myRoom != null)
        {
            myRoom.EnemyDied(this);
        }

        if (PlayerController.instance != null)
            PlayerController.instance.GainExperience(experience);

        Destroy(gameObject);
    }
    
    public virtual void ApplyDungeonScaling(float hpMult, float damageMult, float expMult)
    {
        hp = Mathf.RoundToInt(hp * hpMult);
        //TODO damage mult here
        DoDamageMult(damageMult);
        experience = Mathf.RoundToInt(experience * expMult);
    }

    public virtual void DoDamageMult(float damageMult)
    {
        Debug.Log("called base do damage mult");
    }
}