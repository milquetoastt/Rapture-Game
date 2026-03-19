using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Jetpack : SpellStats
{
    private Rigidbody2D rb;
    public float initialForce = 10f;
    public float force = 10f;
    public Vector2 offset;
    public float maxUpSpeed = 10f;
    public float tiltAngle = 30f;
    public float rotationSpeed = 10f;
    private void Awake()
    {
        rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        gameObject.transform.SetParent(PlayerController.instance.transform);
        gameObject.transform.localPosition = offset;
    }

    public override void CastBoot(Vector2 direction)
    {
        Debug.Log("Cast Jetpack");
        rb.linearVelocityY /= 2;
        rb.AddForce(Vector2.up*initialForce, ForceMode2D.Impulse);
    }

    public override void HoldBoot(Vector2 direction)
    {
        //Debug.Log("holding in jetpack");
        rb.AddForce(Vector2.up * force, ForceMode2D.Force);
    }

    public override void EndBoot(Vector2 direction)
    {
        Destroy(gameObject,.1f);
    }

    private void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float targetZ = 0f;

        if (inputX > 0.1f)
        {
            targetZ = -tiltAngle;
        }
        else if (inputX < -0.1f)
        {
            targetZ = tiltAngle;
        }

        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}
