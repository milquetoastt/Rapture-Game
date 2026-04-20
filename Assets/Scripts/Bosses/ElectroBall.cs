using System;
using UnityEngine;
using System.Collections;

public class ElectroBall : EnemyBullet
{
    
    public CrystalBoss crystalGuy;
    public CrystalController crystalController;
    public Rigidbody2D rb;
    private float electroBallSpeed;
    
    
    public bool chargingUp = true;
    public float chargeTime;
    public float growSpeed;
    public bool hasLaunched = false;

    public Collider2D col;
    
    //https://particularreality.substack.com/p/coding-conventions-and-unity-best
    public Action OnLaunched;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col.enabled = false;
        transform.parent = crystalGuy.tracker;
        growSpeed = crystalGuy.electroBallGrowSpeed;
        electroBallSpeed = crystalGuy.electroBallSpeed;
        chargeTime = crystalGuy.electroBallChargeTime;

        StartCoroutine(Charge());
    }

    private void Update()
    {
        if (chargingUp)
        {
            //transform.position = crystalGuy.firePoint;
            transform.localScale += new Vector3(growSpeed * Time.deltaTime, growSpeed * Time.deltaTime, 0f);
        }
    }

    public IEnumerator Charge()
    {
        chargingUp = true;
        yield return new WaitForSeconds(chargeTime);
        col.enabled = true;
        chargingUp = false;
        transform.parent = null;
        LaunchBall();
    }

    private void LaunchBall()
    {
        //tell crystalController that it can fire the next shot
        OnLaunched?.Invoke();
        Vector3 direction = (PlayerController.instance.transform.position - crystalGuy.firePoint).normalized;
        rb.linearVelocity = direction * electroBallSpeed;
        
    }
}
