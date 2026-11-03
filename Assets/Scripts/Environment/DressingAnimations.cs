using UnityEngine;

public class DressingAnimations : MonoBehaviour
{
    public Animator anim;
    [Header("candle settings")]
    public bool frame2Start = false;

    public bool floating = false;
    [Header("f settings")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector3 offset = new Vector3(0, 1f, 0);
    public float duration = 2f;
    public bool pingPong = true;
    public bool useLocalSpace = true;

    float elapsed;
    Vector3 lastApplied;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (frame2Start)
        {
            anim.SetBool("2ndglow", true);
        }
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
    }
}
