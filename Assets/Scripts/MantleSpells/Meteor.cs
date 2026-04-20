    using System;
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

    private bool hasRebounded = false;

    public float reboundForce = 10f;
    public float reboundHorizontalForce = 10f;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasRebounded) return;

        Rigidbody2D rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy != null && rb != null)
        {
            hasRebounded = true;

            rb.linearVelocity = Vector2.zero;

            float horizontalDir = Mathf.Sign(PlayerController.instance.transform.position.x - enemy.transform.position.x);

            Vector2 reboundDir = new Vector2(horizontalDir * reboundHorizontalForce, reboundForce);
            rb.AddForce(reboundDir, ForceMode2D.Impulse);
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
