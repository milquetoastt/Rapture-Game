using UnityEngine;

public class AirGrapple : SpellStats
{
    public DistanceJoint2D joint;
    public GameObject grapplePrefab;

    public float minDistance = 2f;
    public float maxDistance = 10f;

    private GameObject grappleInstance;
    private bool grappling = false;

    private Vector2 anchorPoint;
    private float grappleDistance;

    public override void CastHat(Vector2 mouseWorld)
    {
        if (joint == null)
            joint = PlayerController.instance.GetComponent<DistanceJoint2D>();

        Vector2 playerPos = PlayerController.instance.transform.position;
        Vector2 toMouse = mouseWorld - playerPos;

        float distanceToMouse = toMouse.magnitude;
        Vector2 dir = distanceToMouse > 0.0001f ? toMouse / distanceToMouse : Vector2.right;

        grappleDistance = Mathf.Clamp(distanceToMouse, minDistance, maxDistance);
        anchorPoint = playerPos + dir * grappleDistance;

        grappling = true;
        HoldGrapple();
    }

    void Update()
    {
        if (grappling)
        {
            HoldGrapple();
        }

        if (Input.GetMouseButtonUp(1))
        {
            joint.enabled = false;

            if (grappleInstance != null)
                Destroy(grappleInstance);

            grappling = false;
            Destroy(gameObject);
        }
    }

    void HoldGrapple()
    {
        Vector2 playerPos = PlayerController.instance.transform.position;

        joint.connectedAnchor = anchorPoint;
        joint.distance = grappleDistance;
        joint.enabled = true;

        if (grappleInstance == null)
        {
            grappleInstance = Instantiate(grapplePrefab, anchorPoint, Quaternion.identity);
        }
        else
        {
            grappleInstance.transform.position = anchorPoint;
        }

        Vector2 dirToPlayer = playerPos - anchorPoint;
        float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        grappleInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = anchorPoint;
    }
}