using UnityEngine;
using System.Collections;

public class BomberGuy : EnemyBase
{

    public PatrolEnemy patrol;
    public WindupRing ring;
    [Header("attack")]
    public GameObject bomb;
    public Transform throwPoint;
    public float attackCooldown = 2f;
    public float windUp = 0.5f;
    public float killExplosionDelay = .3f;

    [Header("throw settings")]
    public float arcHeight = 3f;
    public float bombGravityScale = 1f;

    private Coroutine attackRoutine;
    private float nextAttackTime;

    private void Awake()
    {
        windUp = Random.Range(windUp-.25f, windUp+.25f);
    }
    private void Update()
    {
        if (PlayerController.instance == null)
            return;

        if (stunned)
        {
            Debug.Log("stunned");
            CancelAttack();
            patrol.stunned = true;
            patrol.doingSomethingElse = false;
            
            return;
        }

        patrol.stunned = false;

        //FacePlayer();

        if (attackRoutine == null && Time.time >= nextAttackTime)
            attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    { 
        patrol.doingSomethingElse = true;

        //FacePlayer();
        ring.Begin(windUp);
        yield return new WaitForSeconds(windUp);

        if (stunned)
        {
            attackRoutine = null;
            patrol.doingSomethingElse = false;
            ring.End();
            yield break;
            
        }

        //FacePlayer();
        ThrowBombAtPlayer();

        nextAttackTime = Time.time + attackCooldown;
        
        attackRoutine = null;
        
        yield return new WaitForSeconds(.5f);
        patrol.doingSomethingElse = false;
    }

    private void ThrowBombAtPlayer()
    {
        Vector2 startPos = throwPoint.position;
        Vector2 targetPos = PlayerController.instance.transform.position;

        GameObject spawnedBomb = Instantiate(bomb, startPos, Quaternion.identity);
        BombProjectile bomba = spawnedBomb.GetComponent<BombProjectile>();
        bomba.killExplosionDelay = killExplosionDelay;
        
        Collider2D bombCol = spawnedBomb.GetComponent<Collider2D>();
        Collider2D playerCol = PlayerController.instance.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(bombCol, playerCol);
        
        ring.End();
        
        Rigidbody2D rb = spawnedBomb.GetComponent<Rigidbody2D>();
        rb.gravityScale = bombGravityScale;
        rb.linearVelocity = CalculateLaunchVelocity(startPos, targetPos, arcHeight, rb.gravityScale);
    }

    private Vector2 CalculateLaunchVelocity(Vector2 start, Vector2 target, float extraHeight, float gravityScale)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * gravityScale);
        float apexY = Mathf.Max(start.y, target.y) + extraHeight;
        float velocityY = Mathf.Sqrt(2f * gravity * (apexY - start.y));
        float timeUp = velocityY / gravity;
        float timeDown = Mathf.Sqrt(2f * (apexY - target.y) / gravity);
        float totalTime = timeUp + timeDown;
        float velocityX = (target.x - start.x) / totalTime;
        return new Vector2(velocityX, velocityY);
    }

    private void FacePlayer()
    {
        float playerX = PlayerController.instance.transform.position.x;
        float myX = transform.position.x;

        if (playerX < myX)
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        else if (playerX > myX)
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);


        patrol.direction = transform.localScale.x < 0 ? -1 : 1;
    }

    private void CancelAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
            nextAttackTime = Time.time + attackCooldown;
            ring.End();
        }
    }
}