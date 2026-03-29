using System.Collections;
using UnityEngine;

public class BomberGuy : EnemyBase
{
    public PatrolEnemy patrol;

    [Header("Attack")]
    public float attackTimer = 2.5f;
    public GameObject bomb;
    public Transform throwPoint;

    public float bombGravityScale = 1f;
    public float arcHeight = 3f;
    public float flightTime = 2f;

    public void Start()
    {
        StartCoroutine(Attack());
    }
    private IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackTimer);
            ThrowBomb();
        }

    }

    private void ThrowBomb()
    {

    }

}
