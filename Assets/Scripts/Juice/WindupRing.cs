using UnityEngine;
using System.Collections;

public class WindupRing : MonoBehaviour
{
    public Transform ringVisual;
    public Vector3 startScale = Vector3.one * 2f;
    public Vector3 endScale = Vector3.one * 0.5f;
    
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine routine;

    public void Begin(float duration)
    {
        if (ringVisual == null)
        {
            return;
        }

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(BeginRoutine(duration));
    }

    public void End()
    {
        if (routine != null)
            StopCoroutine(routine);

        if (ringVisual != null)
            ringVisual.gameObject.SetActive(false);
    }

    private IEnumerator BeginRoutine(float duration)
    {
        ringVisual.gameObject.SetActive(true);
        ringVisual.localScale = startScale;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float curvedT = scaleCurve.Evaluate(t);

            ringVisual.localScale = Vector3.LerpUnclamped(startScale, endScale, curvedT);

            yield return null;
        }

        ringVisual.localScale = endScale;
    }
}