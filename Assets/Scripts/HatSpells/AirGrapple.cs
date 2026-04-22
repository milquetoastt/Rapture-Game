using UnityEngine;

public class AirGrapple : SpellStats
{
    public DistanceJoint2D joint;
    public GameObject grapplePrefab;

    public float minDistance = 2f;
    public float maxDistance = 10f;

    public override void CastHat(Vector2 mouseWorld)
    {
        if (joint == null)
            joint = PlayerController.instance.GetComponent<DistanceJoint2D>();

        HoldGrapple(mouseWorld);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            joint.enabled = false;
            Destroy(gameObject);
        }
    }

    void HoldGrapple(Vector2 mouseWorld)
    {
        
        Vector2 playerPos = PlayerController.instance.transform.position;
        Vector2 toMouse = mouseWorld - playerPos;

        float distanceToMouse = toMouse.magnitude;
        Vector2 dir = distanceToMouse > 0.0001f ? toMouse / distanceToMouse : Vector2.right;

        float clampedDist = Mathf.Clamp(distanceToMouse, minDistance, maxDistance);
        Vector2 anchorPoint = playerPos + dir * clampedDist;

        joint.connectedAnchor = anchorPoint;
        joint.distance = clampedDist;
        joint.enabled = true;

        transform.position = anchorPoint;
    }
}