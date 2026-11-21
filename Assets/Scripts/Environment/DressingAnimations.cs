using UnityEngine;

public class DressingAnimations : MonoBehaviour
{
    public Animator anim;
    [Header("candle settings")]
    public bool frame2Start = false;

    [Header("f settings")]
    public bool floating = false;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector3 offset = new Vector3(0, 1f, 0);
    public float duration = 2f;
    public bool pingPong = true;
    public bool useLocalSpace = true;

    [Header("rotation wobble settings")]
    public bool wobble = false;
    [Tooltip("Max rotation in degrees either side of the starting rotation")]
    public float wobbleAngle = 10f;
    [Tooltip("How fast the noise changes (higher = jittery, lower = lazy sway)")]
    public float wobbleSpeed = .2f;
    [Tooltip("Adds a second, faster noise layer for a less uniform motion")]
    [Range(0f, 1f)] public float noiseDetail = 0.3f;
    public bool randomizeSeed = true;

    float elapsed;
    Vector3 lastApplied;

    float wobbleTime;
    float noiseSeed;
    float lastWobbleAngle;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (frame2Start)
        {
            anim.SetBool("2ndglow", true);
        }

        // Random seed so multiple objects don't wobble in sync
        noiseSeed = randomizeSeed ? Random.Range(0f, 1000f) : 0f;
    }

    void Update()
    {
        if (floating)
        {
            elapsed += Time.deltaTime;

            float time01 = pingPong
                ? Mathf.PingPong(elapsed, duration) / duration
                : Mathf.Repeat(elapsed, duration) / duration;

            float t = curve.Evaluate(time01);

            Vector3 currentOffset = offset * t;
            Vector3 delta = currentOffset - lastApplied;
            lastApplied = currentOffset;

            if (useLocalSpace)
                transform.localPosition += delta;
            else
                transform.position += delta;
        }

        if (wobble)
        {
            wobbleTime += Time.deltaTime * wobbleSpeed;
            float n1 = Mathf.PerlinNoise(noiseSeed, wobbleTime) * 2f - 1f;
            float n2 = Mathf.PerlinNoise(noiseSeed + 50f, wobbleTime * 2.7f) * 2f - 1f;
            float noise = Mathf.Lerp(n1, n2, noiseDetail);

            float targetAngle = noise * wobbleAngle;

            float deltaAngle = targetAngle - lastWobbleAngle;
            lastWobbleAngle = targetAngle;

            transform.Rotate(0f, 0f, deltaAngle, useLocalSpace ? Space.Self : Space.World);
        }
    }
}