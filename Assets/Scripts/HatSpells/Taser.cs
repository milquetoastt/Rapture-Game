using UnityEngine;

public class Taser : SpellStats
{
    public float lifeTime = 0.2f;
    public string enemyBulletTag = "EnemyBullet";

    public override void CastHat(Vector2 mouseWorld)
    {
        transform.position = mouseWorld;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyBulletTag))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
