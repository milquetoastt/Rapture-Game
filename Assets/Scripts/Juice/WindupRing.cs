using UnityEngine;
using System.Collections;

[System.Serializable]
public class WindupRingVisual
{
    public Transform ringVisual;

    public Vector3 startScale = Vector3.one * 2f;
    public Vector3 endScale = Vector3.one * 0.5f;

    public Vector3 startRotation;
    public Vector3 endRotation;
}

[System.Serializable]
public class FinishRingVisual
{
    public SpriteRenderer finishRing;

    public Vector3 startScale = Vector3.one * 0.5f;
    public Vector3 endScale = Vector3.one * 2f;

    public Vector3 startRotation;
    public Vector3 endRotation;
}

public class WindupRing : MonoBehaviour
{
    public WindupRingVisual[] ringVisuals;

    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public Animator anim;

    [Header("finish ring")]
    public FinishRingVisual[] finishRings;

    public float lifetime = 1f;
    public float timer;

    private Coroutine routine;
    private Coroutine finishRoutine;

    public void Begin(float duration)
    {
        if (ringVisuals == null || ringVisuals.Length == 0)
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(BeginRoutine(duration));
    }

    public void End()
    {
        if (routine != null)
            StopCoroutine(routine);

        if (ringVisuals != null)
        {
            foreach (WindupRingVisual ring in ringVisuals)
            {
                if (ring == null || ring.ringVisual == null) continue;
                ring.ringVisual.gameObject.SetActive(false);
            }
        }

        if (finishRings != null && finishRings.Length > 0)
        {
            if (finishRoutine != null)
                StopCoroutine(finishRoutine);

            finishRoutine = StartCoroutine(FinishRingRoutine());
        }
    }

    private IEnumerator BeginRoutine(float duration)
    {
        foreach (WindupRingVisual ring in ringVisuals)
        {
            if (ring == null || ring.ringVisual == null) continue;

            ring.ringVisual.gameObject.SetActive(true);
            ring.ringVisual.localScale = ring.startScale;
            ring.ringVisual.localEulerAngles = ring.startRotation;
        }
        anim.SetTrigger("Cast");
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float curvedT = scaleCurve.Evaluate(t);

            foreach (WindupRingVisual ring in ringVisuals)
            {
                if (ring == null || ring.ringVisual == null) continue;

                ring.ringVisual.localScale = Vector3.LerpUnclamped(
                    ring.startScale,
                    ring.endScale,
                    curvedT
                );

                ring.ringVisual.localEulerAngles = Vector3.LerpUnclamped(
                    ring.startRotation,
                    ring.endRotation,
                    curvedT
                );
            }

            yield return null;
        }

        foreach (WindupRingVisual ring in ringVisuals)
        {
            if (ring == null || ring.ringVisual == null) continue;

            ring.ringVisual.localScale = ring.endScale;
            ring.ringVisual.localEulerAngles = ring.endRotation;
        }
    }

    private IEnumerator FinishRingRoutine()
    {
        foreach (FinishRingVisual ring in finishRings)
        {
            if (ring == null || ring.finishRing == null) continue;

            ring.finishRing.gameObject.SetActive(true);
            ring.finishRing.transform.localScale = ring.startScale;
            ring.finishRing.transform.localEulerAngles = ring.startRotation;

            Color c = ring.finishRing.color;
            c.a = 1f;
            ring.finishRing.color = c;
        }

        timer = lifetime;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            float t = Mathf.Clamp01(1f - timer / lifetime);
            float curvedT = scaleCurve.Evaluate(t);

            foreach (FinishRingVisual ring in finishRings)
            {
                if (ring == null || ring.finishRing == null) continue;

                ring.finishRing.transform.localScale = Vector3.LerpUnclamped(
                    ring.startScale,
                    ring.endScale,
                    curvedT
                );

                ring.finishRing.transform.localEulerAngles = Vector3.LerpUnclamped(
                    ring.startRotation,
                    ring.endRotation,
                    curvedT
                );

                Color c = ring.finishRing.color;
                c.a = 1f - curvedT;
                ring.finishRing.color = c;
            }

            yield return null;
        }

        foreach (FinishRingVisual ring in finishRings)
        {
            if (ring == null || ring.finishRing == null) continue;

            Color c = ring.finishRing.color;
            c.a = 0f;
            ring.finishRing.color = c;

            ring.finishRing.gameObject.SetActive(false);
        }
    }
}