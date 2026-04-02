using System;
using UnityEngine;
using System.Collections;

public class ElectroBall : MonoBehaviour
{
    
    public CrystalBoss crystalGuy;
    public Rigidbody2D rb;
    private float electroBallSpeed;
    
    
    public bool chargingUp = true;
    public float chargeTime;
    public float growSpeed;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        chargingUp = false;
        transform.parent = null;
        LaunchBall();
    }

    private void LaunchBall()
    {
        Vector3 direction = (PlayerController.instance.transform.position - crystalGuy.firePoint).normalized;
        rb.linearVelocity = direction * electroBallSpeed;
    }
}
