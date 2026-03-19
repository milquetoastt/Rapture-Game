using UnityEngine;
using System.Collections;
//TODO: implement the knockback/knockup things
public class WaterCut : SpellStats
{
    public Vector3 offset;
    private Rigidbody2D rb;
    public float force;
    public float sidewaysForce;
    public override void CastMantle(GameObject caster, Vector2 direction)
    {
        rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 90f);
        gameObject.transform.SetParent(PlayerController.instance.transform);
        
        gameObject.transform.localPosition = offset * direction.x;
        
        PlayerController.instance.canInputMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.right * direction * sidewaysForce, ForceMode2D.Impulse);
        StartCoroutine(UppercutMove(rb));
        
    }

    IEnumerator UppercutMove(Rigidbody2D rb)
    {
        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            rb.linearVelocityY = force;
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerController.instance.canInputMove = true;
        PlayerController.instance.isCasting = false;
        Destroy(gameObject);
    }
}
