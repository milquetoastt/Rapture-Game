using UnityEngine;
using System.Collections;

public class Archer : EnemyBase
{
    public PatrolEnemy patrolEnemy;
    public LayerMask playerLayer;
    public float lengthOfSight;
    public GameObject arrowPrefab;

    [Header("Attack Stuff")]
    public float attackCooldown;
    public float abilityTimer;
    private Coroutine attackRoutine;
    public float attackTimer = 1.5f;
    public float windUp = 0.25f;
    private float nextAttackTime = 0f;

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

        if (playerAhead && Time.time >= nextAttackTime && attackRoutine == null)
        {
            attackRoutine = StartCoroutine(FireAfterWindup());
            patrolEnemy.doingSomethingElse = true;
        }
    }

    private IEnumerator FireAfterWindup()
    {
        yield return new WaitForSeconds(windUp);

        GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.direction = patrolEnemy.direction;

        attackRoutine = null;
        patrolEnemy.doingSomethingElse = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    protected override void OnStunStart()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        patrolEnemy.doingSomethingElse = false;
        patrolEnemy.stunned = true;
    }

    protected override void OnStunEnd()
    {
        patrolEnemy.stunned = false;
    }
}