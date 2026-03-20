using UnityEngine;

public class CloudCart : SpellStats
{
    private Rigidbody2D rb;
    private float originalGravity;
    public float yDampSpeed = 2f;
    public float cloudOffset = -1f;

    public GameObject rainPrefab;
    public float rainWidth = 2f;
    public float spawnInterval = .3f;
    public float offset = 1f;
    
    private float spawnTimer;
    
    public override void CastBoot(Vector2 direction)
    {
        
        rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        rb.linearVelocityY /= 2;
        gameObject.transform.SetParent(PlayerController.instance.transform);
        gameObject.transform.localPosition = new Vector3(0f, cloudOffset, 0f);
        rb.gravityScale = 0;
    }

    public override void HoldBoot(Vector2 direction)
    {
        Vector2 vel = rb.linearVelocity;

        vel.y = Mathf.Lerp(vel.y, 0f, yDampSpeed * Time.fixedDeltaTime);

        rb.linearVelocity = vel;
        
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnRaindrop();
            spawnTimer = 0f;
        }
    }
    
    public override void EndBoot(Vector2 direction)
    {
        rb.gravityScale = originalGravity;
        Destroy(gameObject, .1f);
    }
    
    private void SpawnRaindrop()
    {
        Vector3 playerPos = PlayerController.instance.transform.position;
        //float randomX = Random.Range(-rainWidth / 2f, rainWidth / 2f);

        //Vector3 spawnPos = new Vector3( playerPos.x + randomX, playerPos.y - offset,0);
        Vector3 spawnPos = new Vector3(playerPos.x, playerPos.y + offset, 0);
        Instantiate(rainPrefab, spawnPos, Quaternion.identity);
    }
}
