using UnityEngine;
using System.Collections;
using System;

public class CrystalLaser : MonoBehaviour
{
    private CrystalBoss crystalGuy;
    public GameObject indicator;
    public GameObject laser;
    public Action OnFinished;

    [Header("angles")]
    public float startAngleDeg;
    public float targetAngleDeg;
    public float currentAngleDeg;

    [Header("movement")] 
    public bool charging = true;
    public float rotateSpeed = 90f;
    public bool returning = false;
    public bool waiting = false;
    private bool startedReturn = false;
    public float pauseTime = 1f;
    public int timesStopped = 0;


    public void Initialize(CrystalBoss owner, float startAngle, float targetAngle)
    {
        crystalGuy = owner;
        startAngleDeg = startAngle;
        targetAngleDeg = targetAngle;
        currentAngleDeg = startAngle;
        rotateSpeed = crystalGuy.laserBeamRotateSpeed;
        UpdateLaserTransform();
        indicator.SetActive(true);
        laser.SetActive(false);
        StartCoroutine(FiringMyLazor());
    }

    private IEnumerator FiringMyLazor()
    {
        yield return new WaitForSeconds(pauseTime);
        laser.SetActive(true);
        charging = false;
    }

    private void Update()
    {
        if(charging)
            return;
        if (!returning && !waiting)
        {
            currentAngleDeg = Mathf.MoveTowardsAngle(currentAngleDeg, targetAngleDeg, rotateSpeed * Time.deltaTime);
            UpdateLaserTransform();
        }
        
        
        else if (returning && !waiting)
        {
            Debug.Log("returning");
            currentAngleDeg = Mathf.MoveTowardsAngle(currentAngleDeg, startAngleDeg, rotateSpeed * Time.deltaTime);
            UpdateLaserTransform();
        }

        // done //TODO: this if will break. make sure it fits all scenarios where it reaches the end
        if ((Mathf.Abs(Mathf.DeltaAngle(currentAngleDeg, targetAngleDeg)) < 0.01f && !waiting && !startedReturn) ||
            Mathf.Abs(Mathf.DeltaAngle(currentAngleDeg, startAngleDeg)) < 0.01f && !waiting && startedReturn)
        {
            timesStopped++;
            if (timesStopped >= 3)
            {
                OnFinished?.Invoke();
                Destroy(gameObject,pauseTime);
            }
            else
                StartCoroutine(WaitForLaser());
        }
        
    }

    private IEnumerator WaitForLaser()
    {
        waiting = true;
        Debug.Log("waiting");
        yield return new WaitForSeconds(pauseTime);
        startedReturn = !startedReturn;
        Debug.Log("done waiting");
        waiting = false;
        returning = !returning;
    }
    
    private void UpdateLaserTransform()
    {
        float angleRad = currentAngleDeg * Mathf.Deg2Rad;

        Vector3 offset = crystalGuy.transform.position + new Vector3(
            Mathf.Cos(angleRad) * crystalGuy.ovalRadiusY,
            Mathf.Sin(angleRad) * crystalGuy.ovalRadiusY,
            0f
        );
        //Debug.Log(offset);
        transform.position = offset;

        Vector3 awayFromBoss = (offset - crystalGuy.transform.position).normalized;
        float faceAngle = Mathf.Atan2(awayFromBoss.y, awayFromBoss.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, faceAngle);
    }
}