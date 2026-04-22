using UnityEngine;
using System.Collections;

public class Archer : EnemyBase
{
    public PatrolEnemy patrolEnemy;
    public LayerMask playerLayer;
    public float lengthOfSight;
    public GameObject arrowPrefab;
    public WindupRing ring;

    [Header("attack stuff")]
    public float attackCooldown;
    public float abilityTimer;
    private Coroutine attackRoutine;
    public float attackTimer = 1.5f;
    public float windUp = 0.25f;
    private float nextAttackTime = 0f;
    private bool hasFiredAtCurrentSighting = false;

    protected override void Awake()
    {
        base.Awake();

        if (patrolEnemy == null)
            patrolEnemy = GetComponent<PatrolEnemy>();
    }

    private void Update()
    {
        if (patrolEnemy != null)
            patrolEnemy.stunned = stunned;

        if (stunned)
            return;

        int facingDirection = GetFacingDirection();

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right * facingDirection,
            lengthOfSight,
            playerLayer
        );

        bool playerAhead = hit.collider != null && hit.collider.CompareTag("Player");

        if (!playerAhead)
        {
            hasFiredAtCurrentSighting = false;
        }

        if (playerAhead && !hasFiredAtCurrentSighting && Time.time >= nextAttackTime && attackRoutine == null)
        {
            if (patrolEnemy != null)
                patrolEnemy.doingSomethingElse = true;

            attackRoutine = StartCoroutine(FireAfterWindup(facingDirection));
        }
    }

    private IEnumerator FireAfterWindup(int fireDirection)
    {
        try
        {
            if (ring != null)
                ring.Begin(windUp);

            yield return new WaitForSeconds(windUp);

            GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            

            if (arrow != null)
                arrow.direction = fireDirection;

            hasFiredAtCurrentSighting = true;
            nextAttackTime = Time.time + attackCooldown;
        }
        finally
        {
            if (ring != null)
                ring.End();

            attackRoutine = null;

            if (patrolEnemy != null)
                patrolEnemy.doingSomethingElse = false;
        }
    }

    private int GetFacingDirection()
    {
        if (patrolEnemy != null)
            return patrolEnemy.direction;

        return transform.localScale.x >= 0 ? 1 : -1;
    }

    protected override void OnStunStart()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (ring != null)
            ring.End();

        if (patrolEnemy != null)
        {
            patrolEnemy.doingSomethingElse = false;
            patrolEnemy.stunned = true;
        }
    }

    protected override void OnStunEnd()
    {
        if (patrolEnemy != null)
            patrolEnemy.stunned = false;
    }

    public override void DoDamageMult(float mult)
    {
        Arrow arrow = arrowPrefab.GetComponent<Arrow>();
        arrow.damage = Mathf.RoundToInt(arrow.damage * mult);
    }
}