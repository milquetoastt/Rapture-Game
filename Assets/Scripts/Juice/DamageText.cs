using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private Rigidbody2D rb;

    public TMP_Text text;
    public int damage;
    public float horizontalForce = 10f;
    public float upForce = 10f;
    public float lifeTime = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(int damageAmount)
    {
        damage = damageAmount;
        Debug.Log("damageText received: " + damageAmount);
        if (text == null)
            text = GetComponent<TMP_Text>();

        text.text = damage.ToString();

        DoForce();
        Destroy(gameObject, lifeTime);
    }

    public void DoForce()
    {
        Vector2 dir = Vector2.zero;

        if (PlayerController.instance.transform.position.x > transform.position.x)
            dir.x = Random.Range(-horizontalForce - 8f, -horizontalForce);
        else
            dir.x = Random.Range(horizontalForce, horizontalForce + 8f);

        dir.y = upForce;
        rb.AddForce(dir, ForceMode2D.Impulse);
    }
}