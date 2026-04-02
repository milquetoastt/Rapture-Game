using UnityEngine;

public class CrystalBoss : EnemyBase 
{
    protected override void Awake()
    {
        base.Awake();
        canBeStunned = false;
    }
    
}
