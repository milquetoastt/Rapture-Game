using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Tooltip("0 = stays fixed in the world, 1 = moves with the camera.")]
    [SerializeField] private Vector2 parallaxEffect = new Vector2(0.5f, 0.5f);

    private Transform cam;
    private Vector3 startPosition;
    private Vector3 camStartPosition;

    private void Start()
    {
        cam = Camera.main.transform;
        startPosition = transform.position;
        camStartPosition = cam.position;
    }

    private void Update()
    {
        Vector3 camDelta = cam.position - camStartPosition;

        transform.position = new Vector3(
            startPosition.x + camDelta.x * parallaxEffect.x,
            startPosition.y + camDelta.y * parallaxEffect.y,
            startPosition.z
        );
    }
}