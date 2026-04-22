using UnityEngine;

public class Goob : EnemyBase
{

    public int damage = 1;
    public float knockbackForce = 6;
    public float stunTime = .8f;
    public bool tutorial = false;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Player"))
        {
            PlayerController.instance.TakeHit(damage, knockbackForce, transform.position);
            PlayerController.instance.StunRoutineWrapper(stunTime);
        }
    }
    protected override void Die()
    {
        if (myRoom != null)
        {
            myRoom.EnemyDied(this);
        }

        if (PlayerController.instance != null)
            PlayerController.instance.Level();

        Destroy(gameObject);
    }
}
