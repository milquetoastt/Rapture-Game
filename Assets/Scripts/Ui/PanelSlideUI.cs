using UnityEngine;
using System.Collections;

public class PanelSlideUI : MonoBehaviour
{
    public RectTransform panel;
    public Vector2 hiddenPos;
    public Vector2 shownPos;
    public float duration = 0.4f;
    public AnimationCurve curve;

    private Coroutine slideRoutine;

    public void SlideIn()
    {
        if (slideRoutine != null) StopCoroutine(slideRoutine);
        Time.timeScale = 0;
        
        if(UIManager.instance != null) 
            UIManager.instance.gameIsPaused = true;
        if(ManaResource.instance != null) 
            ManaResource.instance.allowTimeControl = false;
        
        slideRoutine = StartCoroutine(Slide(hiddenPos, shownPos));
    }

    public void SlideOut()
    {
        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(Slide(shownPos, hiddenPos,true));
    }

    private IEnumerator Slide(Vector2 start, Vector2 end,bool slidingOut = false)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float curvedT = curve.Evaluate(t);
            panel.anchoredPosition = Vector2.Lerp(start, end, curvedT);
            yield return null;
        }
        
        Time.timeScale = 1;
        
        if (slidingOut && UIManager.instance != null)
        {
            UIManager.instance.gameIsPaused = false;
            ManaResource.instance.allowTimeControl = true;
        }
        panel.anchoredPosition = end;
    }
}
