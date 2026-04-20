using UnityEngine;
using System.Collections;

public class Archer : EnemyBase
{
    public PatrolEnemy patrolEnemy;
    public LayerMask playerLayer;
    public float lengthOfSight;
    public GameObject arrowPrefab;
    public WindupRing ring;

    [Header("Attack Stuff")]
    public float attackCooldown;
    public float abilityTimer;
    private Coroutine attackRoutine;
    public float attackTimer = 1.5f;
    public float windUp = 0.25f;
    private float nextAttackTime = 0f;
    private bool hasFiredAtCurrentSighting = false;
    
    private void Update()
    {
        patrolEnemy.stunned = stunned;

        if (stunned)
            return;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right * patrolEnemy.direction,
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
            patrolEnemy.doingSomethingElse = true;
            attackRoutine = StartCoroutine(FireAfterWindup());
        }
    }

    private IEnumerator FireAfterWindup()
    {
        try
        {
            ring.Begin(windUp);
            yield return new WaitForSeconds(windUp);

            GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            if (arrow != null)
                arrow.direction = patrolEnemy.direction;

            hasFiredAtCurrentSighting = true;
            nextAttackTime = Time.time + attackCooldown;
        }
        finally
        {
            ring.End();
            attackRoutine = null;
            patrolEnemy.doingSomethingElse = false;
        }
    }

    protected override void OnStunStart()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        ring.End();
        patrolEnemy.doingSomethingElse = false;
        patrolEnemy.stunned = true;
    }

    protected override void OnStunEnd()
    {
        patrolEnemy.stunned = false;
    }
}