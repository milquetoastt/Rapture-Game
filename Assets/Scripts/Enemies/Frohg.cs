using UnityEngine;

public class Frohg : EnemyBase
{
    public float castCooldown = 3f;
    public bool isJumping = false;
    public float UpForce = 8f;
    public float SideForce = 5f;
    public float minAirTime = 0.1f; // ignores ground contact right at takeoff

    private float timer;
    private bool airborne = false;
    private float airTimer;
    private float dirToPlayer;

    void Update()
    {
        CheckForPlayer();

        if (airborne)
        {
            airTimer += Time.deltaTime;
            return;
        }

        timer += Time.deltaTime;

        if (alerted && !stunned && timer >= castCooldown && !isJumping)
        {
            dirToPlayer = PlayerController.instance.transform.position.x - transform.position.x;
            FaceDirection(dirToPlayer);
            isJumping = true;
            timer = 0f;
            anim.SetTrigger("JumpWindup");
        }
    }

    public void Jump()
    {
        anim.SetTrigger("Jump");
        isJumping = true;
        airborne = true;
        airTimer = 0f;

        float dir = Mathf.Sign(PlayerController.instance.transform.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.AddForce(new Vector2(dir * SideForce, UpForce), ForceMode2D.Impulse);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!airborne || airTimer < minAirTime) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                Land();
                return;
            }
        }
    }

    private void Land()
    {
        airborne = false;
        isJumping = false;
        timer = 0f;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // flip now to face the player
        float dirToPlayer = PlayerController.instance.transform.position.x - transform.position.x;
        FaceDirection(dirToPlayer);

        anim.SetTrigger("Landed");
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}