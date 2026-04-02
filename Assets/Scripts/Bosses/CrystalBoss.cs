using UnityEngine;

public class CrystalBoss : EnemyBase 
{
    
    [Header("tracking")]
    public Vector3 firePoint;
    public float ovalRadiusX;
    public float ovalRadiusY;
    public Transform tracker;
    
    [Header("pikachu! use electroball")]
    public GameObject electroBallPrefab;
    public float electroBallSpeed = 2f;
    public float electroBallChargeTime = 1f;
    public float electroBallGrowSpeed = 1f;
    
    
    
    protected override void Awake()
    {
        base.Awake();
        canBeStunned = false;
        // keep uncommented for testing. will need to change instantiate ball in ElectroBall() 
        // v
        
    }

    private void Update()
    {
        GetPointOnOval();
        tracker.transform.position = firePoint;
    }
    public void GetPointOnOval()
    {
        Vector3 toPlayer = PlayerController.instance.transform.position - transform.position;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x);

        Vector3 pointOnOval = transform.position + new Vector3(
            Mathf.Cos(angle) * ovalRadiusX,
            Mathf.Sin(angle) * ovalRadiusY, 0
        );
        firePoint = pointOnOval;
        
    }

    public void UseElectroBall()
    {
        GameObject electroBall = Instantiate(electroBallPrefab, firePoint, Quaternion.identity);
        ElectroBall ball = electroBall.GetComponent<ElectroBall>();
        ball.crystalGuy = this;
    }
    
    
    
    
}
