using UnityEngine;
using System.Collections;

public class JuiceManager : MonoBehaviour
{
    [Header("hit juice")]
    public float hitJuiceDuration = .2f;
    public float shakeDuration;
    public float shakeMagnitude;
    public GameObject damageTextPrefab; 
    
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

    private bool hitJuiceRunning = false;

    public void HitJuice(Vector2 location, int damage)
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
            StartCoroutine(HitJuiceCoroutine());
        }
    }

    private IEnumerator HitJuiceCoroutine()
    {
        hitJuiceRunning = true;

        UIManager.instance.gameIsPaused = true;
        Time.timeScale = 0;
        ManaResource.instance.allowTimeControl = false;

        float pauseEndTime = Time.realtimeSinceStartup + hitJuiceDuration;

        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return null;
        }

        Time.timeScale = 1;
        UIManager.instance.gameIsPaused = false;
        ManaResource.instance.allowTimeControl = true;

        hitJuiceRunning = false;
    }

    public void CastJuice()
    {
        Debug.Log("implement Cast Juice");
    }
}
