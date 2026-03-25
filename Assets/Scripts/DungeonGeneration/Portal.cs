using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("stats")]
    public bool unlocked = true;

    [Header("where to?")]
    public bool down;
    public bool up;
    public bool left;
    public bool right;

    [Header("distance to teleport?")]
    public float xDistance;
    public float yDistance;

    private Rigidbody2D rb;
    public void DoTeleport()
    {
        rb = PlayerController.instance.GetComponent<Rigidbody2D>();
        PlayerController.instance.canInputMove = false;
        rb.linearVelocity = Vector2.zero;
        Vector2 playerLocation = PlayerController.instance.transform.position;
        

        if (down)
            PlayerController.instance.transform.position = playerLocation - new Vector2(0, -yDistance);
        else if (up)
            PlayerController.instance.transform.position = playerLocation - new Vector2(0, yDistance);
        else if (left)
            PlayerController.instance.transform.position = playerLocation - new Vector2(-xDistance, 0);
        else
            PlayerController.instance.transform.position = playerLocation - new Vector2(xDistance, 0);

        PlayerController.instance.canInputMove = true;

    }

    public void TryTeleport()
    {
        if (unlocked)
            DoTeleport();
    }

    public void Lock()
    {
        unlocked = false;
    }

    public void Unlock()
    {
        unlocked = true;
    }
}
