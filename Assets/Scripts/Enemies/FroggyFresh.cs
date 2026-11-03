using UnityEngine;
using System.Collections;
public class FroggyFresh : EnemyBase
{
    [Header("frog things")]
    public float castCooldown = 3f;
    public float windup;
    public bool isCasting = false;
    private float timer;
    private Coroutine castRoutine;
    public float upForce = 20f;
    public float sideForce = 50f;
    public bool isGrounded = true;
    public int contactDamage = 15;
    public float knockbackForce = 6;
    public float stunTime = .9f;
    
    
    void Update()
    {
        if (stunned)
            return;
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        timer += Time.deltaTime;
        if (alerted && timer >= castCooldown && !isCasting && isGrounded)
        {
            castRoutine = StartCoroutine(CastSpell());
        }
        if(!alerted)
            CheckForPlayer();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Player"))
        {
            PlayerController.instance.TakeHit(contactDamage, knockbackForce, transform.position);
            PlayerController.instance.StunRoutineWrapper(stunTime);
        }
    }
    
    private IEnumerator CastSpell()
    {
        isCasting = true;
        //windup
        // windupAnimation
        yield return new WaitForSeconds(windup);
        //if to the left
        Vector2 direction;
        if (PlayerController.instance.transform.position.x < transform.position.x)
            direction = new Vector2(-sideForce, upForce);
        else
            direction = new Vector2(sideForce, upForce);
        rb.AddForce(direction,ForceMode2D.Impulse);
        //launch
        //landing

        timer = 0f;
        isCasting = false;
    }

    protected override void OnStunStart()
    {
        isCasting = false;
        if (castRoutine != null)
        {
            StopCoroutine(castRoutine);
            castRoutine = null;
        }
    }
}
