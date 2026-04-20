using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyBase : MonoBehaviour
{
    [Header("stats")]
    public int hp = 30;
    public bool canBeStunned=true;
    public DungeonRoom myRoom;
    public bool stunned = false;
    public int experience = 1;
    

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
            //if hit by rain or jetpack, ignore the hit juice
            CloudRain rain = collision.GetComponent<CloudRain>();
            Jetpack jetpack = collision.GetComponent<Jetpack>();
            if (rain != null|| jetpack != null)
            {
                Debug.Log("hit by rain");
            }
            else
                UIManager.instance.HitJuice();

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

        if (knockbackForce > 0f)
        {
            Vector2 knockDir = ((Vector2)transform.position - hitSourcePos).normalized;
            knockDir.y = 0f;
            knockDir.Normalize();
        }

        if (hp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        // keep commented during testing
        if (myRoom != null)
        {
            myRoom.EnemyDied(this);
        }
        PlayerController.instance.GainExperience(experience);

        Destroy(gameObject);
    }
}