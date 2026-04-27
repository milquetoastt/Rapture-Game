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

    public void HitJuice(Vector2 location, int damage)
    {
        if (!UIManager.instance.gameIsPaused)
        {
            UIManager.instance.gameIsPaused = true;
            StartCoroutine(HitJuiceCoroutine(location, damage));
        }
    }

    private IEnumerator HitJuiceCoroutine(Vector2 location, int damage)
    {
        Time.timeScale = 0;
        ManaResource.instance.allowTimeControl = false;
        
        float pauseEndTime = Time.realtimeSinceStartup + hitJuiceDuration;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        FindObjectOfType<CinemachineMouseOffset2DD>().ShakeCamera(shakeDuration, shakeMagnitude);

        //damage text
        GameObject textObj = Instantiate(damageTextPrefab, location, Quaternion.identity);
        DamageText damageText = textObj.GetComponent<DamageText>();
        damageText.Setup(damage);
        

        Time.timeScale = 1;
        UIManager.instance.gameIsPaused = false;
        ManaResource.instance.allowTimeControl = true;
        
    }

    public void CastJuice()
    {
        Debug.Log("implement Cast Juice");
    }
}
