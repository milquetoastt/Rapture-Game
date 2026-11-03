using UnityEngine;
using System.Collections;

public class JuiceManager : MonoBehaviour
{
    [Header("hit juice")]
    public float hitJuiceDuration = .2f;
    public float shakeDuration;
    public float shakeMagnitude;
    public GameObject damageTextPrefab; 
    private Coroutine pauseRoutine;
    
    [Header("rapture juice")]
    public GameObject raptureAnimationPrefab;
    
    public static JuiceManager instance;
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void PauseTime(float duration)
    {
        if(pauseRoutine != null)
            StopCoroutine(pauseRoutine);
        
        pauseRoutine = StartCoroutine(PauseTimeCoroutine(duration));
        
    }

    public IEnumerator PauseTimeCoroutine(float duration)
    {
        UIManager.instance.gameIsPaused = true;
        Time.timeScale = 0;
        ManaResource.instance.allowTimeControl = false;
        
        float endTime = Time.realtimeSinceStartup + duration;
        while (Time.realtimeSinceStartup < endTime)
        {
            yield return null;
        }
        
        Time.timeScale = 1f;
        UIManager.instance.gameIsPaused = false;
        ManaResource.instance.allowTimeControl = true;

        pauseRoutine = null;
    }
    
    

    private bool hitJuiceRunning = false;

    public void HitJuice(Vector2 location, int damage, float pauseLength = .5f)
    {
        // always show damage text
        GameObject textObj = Instantiate(damageTextPrefab, location, Quaternion.identity);
        DamageText damageText = textObj.GetComponent<DamageText>();
        damageText.Setup(damage);

        // always allow shake
        FindObjectOfType<CinemachineMouseOffset2DD>().ShakeCamera(shakeDuration, shakeMagnitude);

        // only one actual pause
        if (!hitJuiceRunning)
        {
            StartCoroutine(HitJuiceCoroutine(pauseLength));
        }
    }

    private IEnumerator HitJuiceCoroutine(float pauseLength)
    {
        hitJuiceRunning = true;
        yield return new WaitForEndOfFrame();
        PauseTime(pauseLength);
        hitJuiceRunning = false;
    }

    public void CastJuice()
    {
        Debug.Log("implement Cast Juice");
    }

    public void RaptureJuice(int element, GameObject target)
    {
        if (target == null)
            return;
        // darken everything else BUT affected enemies
        // 
        switch (element)
        {
            case 0 :
                Debug.Log("calling wind rapture");
                StartCoroutine(WindRapJuice(element, target));
                break;
        }

    }
    
    private IEnumerator WindRapJuice(int element, GameObject target)
    {
        
        GameObject animation = Instantiate(raptureAnimationPrefab,target.transform.position,Quaternion.identity);
        RaptureAnimation raptureAnimation = animation.GetComponent<RaptureAnimation>();
        raptureAnimation.StartUp(element, target);
        yield return new WaitForEndOfFrame();
    }
}
