using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBase : MonoBehaviour
{
    [Header("stats")]
    public int hp = 30;
    public bool canBeStunned = true;
    public DungeonRoom myRoom;
    public bool stunned = false;
    public int experience = 1;
    public int element;
	public Animator anim;

    protected Coroutine stunRoutine;
    public Rigidbody2D rb;
    private HashSet<SpellStats> spellsThatHitMe = new HashSet<SpellStats>();
    [Header("detection")]
    public float detectionRadius = 6f;
    public bool alerted = false;
    public List<(int elementType, int castType)> hitElementList = new List<(int element, int castType)>();
    private SpriteRenderer spr;

    protected virtual void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }
    
    public bool spriteFacesRight = true;

    protected void FaceDirection(float dirX)
    {
        if (spr == null || Mathf.Approximately(dirX, 0f)) return;

        bool wantRight = dirX > 0f;
        spr.flipX = (wantRight != spriteFacesRight);
    }
    protected virtual void CheckForPlayer()
    {

        float dist = Vector2.Distance(transform.position, PlayerController.instance.transform.position);

        if (dist <= detectionRadius)
        {
            alerted = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        SpellStats spell = collision.GetComponent<SpellStats>();

        if (spell != null)
        {
            if (spellsThatHitMe.Contains(spell))
            {
                return;
            }
            TakeHit(spell.damage, spell.knockback, collision.transform.position);
            spellsThatHitMe.Add(spell);
            
            if(hitElementList.Count == 0)
                hitElementList.Add((spell.element, spell.castType));
            else if (hitElementList[^1].castType != spell.castType)
                hitElementList.Add((spell.element, spell.castType));
            
            // if hit by rain or jetpack, ignore the hit juice
            CloudRain rain = collision.GetComponent<CloudRain>();
            Jetpack jetpack = collision.GetComponent<Jetpack>();
            Droplet droplet = collision.GetComponent<Droplet>();

            

            if (spell.stunTime > 0f)
            {
                if (stunRoutine != null)
                    StopCoroutine(stunRoutine);

                stunRoutine = StartCoroutine(StunCoroutine(spell.stunTime));
            }
            
            if (hitElementList.Count == 4)
            {
                RaptureManager.instance.DoRapture(hitElementList[^1].elementType, this);
                hitElementList.Clear();
                
            }
        }
    }

    private void EnemyBaseDoHitJuice(int damage)
    {
        Vector2 location = new Vector2(transform.position.x, transform.position.y);
        JuiceManager.instance.HitJuice(location, damage);

    }

    public IEnumerator StunCoroutine(float stunTime)
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
        EnemyBaseDoHitJuice(damage);
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
        
        anim.SetTrigger("Die");
        
        //Destroy(gameObject);
    }
    
    public virtual void ApplyDungeonScaling(float hpMult, float damageMult, float expMult)
    {
        hp = Mathf.RoundToInt(hp * hpMult);
        DoDamageMult(damageMult);
        experience = Mathf.RoundToInt(experience * expMult);
    }

    public virtual void DoDamageMult(float damageMult)
    {
        Debug.Log("called base do damage mult");
    }
}