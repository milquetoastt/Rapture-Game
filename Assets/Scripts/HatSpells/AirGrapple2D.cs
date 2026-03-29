using UnityEngine;

public class AirGrapple2D : MonoBehaviour
{
    public Camera cam;
    public DistanceJoint2D joint;

    public float minDistance = 2f;
    public float maxDistance = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseScreen = Input.mousePosition;

            // Convert mouse to world point (important in 2D)
            Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
            HoldGrapple(mouseWorld);
        }

        if (Input.GetMouseButtonUp(1))
        {
            joint.enabled = false;
        }
    }

    void HoldGrapple(Vector2 mouseWorld)
    {
        Vector2 playerPos = PlayerController.instance.transform.position;
        Vector2 toMouse = mouseWorld - playerPos;

        float distanceToMouse = toMouse.magnitude;
        Debug.Log(distanceToMouse);

        // Avoid issues if mouse is exactly on player
        Vector2 dir = (distanceToMouse > 0.0001f) ? (toMouse / distanceToMouse) : Vector2.right;

        // Clamp the length
        float clampedDist = Mathf.Clamp(distanceToMouse, minDistance, maxDistance);

        // Place anchor at the clamped spot
        joint.connectedAnchor = playerPos + dir * clampedDist;

        // Make joint length match
        joint.distance = clampedDist;

        joint.enabled = true;

        
    }
}
