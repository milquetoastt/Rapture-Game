    using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Meteor : SpellStats
{
    public float upForce = 50f;
    public float fallingHorizontalSpeed = 1f;
    public float fallingVerticalSpeed = 1f;
    public float dashSpeed = 20f;
    private bool wasGrounded= true;

    public override void CastMantle(GameObject caster, Vector2 direction)
    {
        Rigidbody2D rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        gameObject.transform.SetParent(PlayerController.instance.transform);
        if (rb != null)
        {
            Debug.Log(direction);
            if (PlayerController.instance.isGrounded)
            {
                //sideways flying
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
                rb.AddForce(Vector2.right *direction * dashSpeed, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("downward meteor");
                //downward strike\
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.right *direction * fallingHorizontalSpeed, ForceMode2D.Impulse);
                rb.AddForce(Vector2.down * fallingVerticalSpeed, ForceMode2D.Impulse);
            }
            //GameObject meteorBox = Instantiate(meteorPrefab, caster.transform);
            //meteorBox.transform.localPosition = offset;
        }
    }

    private void Update()
    {
        if (wasGrounded && !PlayerController.instance.isGrounded)
        {
            Debug.Log("meteor takeoff");
            
        }
        
        if (PlayerController.instance.isGrounded && !wasGrounded)
        {
            Debug.Log("meteor touched da ground");
            PlayerController.instance.isCasting = false;
            Destroy(gameObject,.2f);
        }
        
        wasGrounded = PlayerController.instance.isGrounded;
    }
}
