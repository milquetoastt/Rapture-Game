using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class CrystalBoss : EnemyBase
{
    private int maxHP;

    [Header("tracking")]
    public Vector3 firePoint;
    public float ovalRadiusX;
    public float ovalRadiusY;
    public Transform tracker;

    [Header("laser beams")]
    public GameObject laserBeamPrefab;
    public float laserBeamRotateSpeed;

    [Header("pikachu! use electroball")]
    public GameObject electroBallPrefab;
    public float electroBallSpeed = 2f;
    public float electroBallChargeTime = 1f;
    public float electroBallGrowSpeed = 1f;

    [Header("behaviors")]
    public BossState currentState;
    public bool attackInProgress = false;
    public float timeBetweenAttacks = 2f;

    public int attackIndex;
    public int lastAttackIndex = -1;

    public enum BossState
    {
        Idle,
        ChoosingAttack,
        Attacking,
        Recovering,
        Dead
    }

    private void Awake()
    {
        base.Awake();
        canBeStunned = false;
        maxHP = hp;
    }

    private void Start()
    {
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (currentState != BossState.Dead)
        {
            currentState = BossState.Idle;
            Debug.Log("waiting");
            yield return new WaitForSeconds(3f);

            currentState = BossState.ChoosingAttack;
            attackIndex = ChooseAttack();
            //attackIndex = 5;
            currentState = BossState.Attacking;
            attackInProgress = true;

            switch (attackIndex)
            {
                case 0:
                {
                    yield return StartCoroutine(BallAttack(2));
                    break;
                }

                case 1:
                {
                    yield return StartCoroutine(LaserAttack(
                        new (float, float)[]
                        {
                            (45f, 135f), (135f, 225f), (225f, 315f), (315f, 45f)
                        }));
                    break;
                }

                case 2:
                {
                    StartCoroutine(BallAttack(2,2f));

                    yield return StartCoroutine(LaserAttack(
                        new (float, float)[]
                        {
                            (90f, 269.9f), (270, 89.9f)
                        }));

                    break;
                }

                case 3:
                {
                    StartCoroutine(BallAttack(4, 2f));
                    yield return StartCoroutine(LaserAttack(
                        new (float, float)[]
                        {
                            (45f, 135f),(225f, 315f)
                        }));
                    break;
                }

                case 4:
                {
                    StartCoroutine(BallAttack(6,2.5f));
                    yield return StartCoroutine(LaserAttack(
                        new (float, float)[]
                        {
                            (45f, 89.9f + 135f),(225f, 89.9f + 315f),(135f, 89.9f + 225f), (315f, 89.9f + 45f)
                        }));
                    break;
                }
                
                case 5:
                    StartCoroutine(BallAttack(3,2f));
                    yield return StartCoroutine(LaserAttack(
                        new (float, float)[]
                        {
                            (70,20),(290, 340),(110,160),(200,250)
                        }));
                    break;
                
            }

            attackInProgress = false;

            if (currentState != BossState.Dead)
            {
                currentState = BossState.Recovering;
                yield return new WaitForSeconds(timeBetweenAttacks);
            }
        }
    }

    private int GetPhase()
    {
        float hpPercent = (float)hp / maxHP;

        if (hpPercent > 0.66f)
            return 0;
        else if (hpPercent > 0.33f)
            return 1;

        return 2;
    }

    private int ChooseAttack()
    {
        int phase = GetPhase();
        int chosenAttack = -1;

        switch (phase)
        {
            case 0:
                Debug.Log("phase0");
                do
                {
                    chosenAttack = Random.Range(0, 2);
                }
                while (chosenAttack == lastAttackIndex);
                break;

            case 1:
                Debug.Log("phase1");
                do
                {
                    chosenAttack = Random.Range(2, 4);
                }
                while (chosenAttack == lastAttackIndex);
                break;

            case 2:
                Debug.Log("phase2");
                do
                {
                    chosenAttack = Random.Range(4, 6);
                }
                while (chosenAttack == lastAttackIndex);
                break;
        }

        lastAttackIndex = chosenAttack;
        return chosenAttack;
    }

    private IEnumerator BallAttack(int numShots, float delay = 1.5f)
    {
        for (int i = 0; i < numShots; i++)
        {
            bool hasLaunched = false;

            UseElectroBall(() => hasLaunched = true);

            yield return new WaitUntil(() => hasLaunched);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator LaserAttack((float start, float target)[] laserAngles)
    {
        int lasersFinished = 0;
        int totalLasers = laserAngles.Length;

        for (int i = 0; i < totalLasers; i++)
        {
            CrystalLaser laser = SpawnSweepingLaser(
                laserAngles[i].start,
                laserAngles[i].target
            );

            laser.OnFinished += () => lasersFinished++;
        }

        yield return new WaitUntil(() => lasersFinished >= totalLasers);
    }

    private void Update()
    {
        GetPointOnOval();
        tracker.transform.position = firePoint;

        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnSweepingLaser(70, 20);
            SpawnSweepingLaser(110f, 160f);
            SpawnSweepingLaser(250f, 200f);
            SpawnSweepingLaser(290f, 340f);
        }
    }

    public CrystalLaser SpawnSweepingLaser(float startAngleDeg, float targetAngleDeg)
    {
        GameObject laserObj = Instantiate(laserBeamPrefab, transform.position, Quaternion.identity);
        CrystalLaser laser = laserObj.GetComponent<CrystalLaser>();

        if (laser != null)
        {
            laser.Initialize(this, startAngleDeg, targetAngleDeg);
        }

        return laser;
    }

    public void GetPointOnOval()
    {
        Vector3 toPlayer = PlayerController.instance.transform.position - transform.position;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x);

        Vector3 pointOnOval = transform.position + new Vector3(
            Mathf.Cos(angle) * ovalRadiusX,
            Mathf.Sin(angle) * ovalRadiusY,
            0f
        );

        firePoint = pointOnOval;
    }

    public ElectroBall UseElectroBall(Action onLaunchedCallback)
    {
        GameObject electroBall = Instantiate(electroBallPrefab, firePoint, Quaternion.identity);
        ElectroBall ball = electroBall.GetComponent<ElectroBall>();

        ball.crystalGuy = this;
        ball.OnLaunched += onLaunchedCallback;

        return ball;
    }
}