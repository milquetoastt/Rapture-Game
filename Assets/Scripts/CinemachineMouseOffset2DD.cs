using UnityEngine;
using Cinemachine;

public class CinemachineMouseOffset2DD : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCam;

    [Header("Feel")]
    public float maxOffset = 2.5f;      // world units
    public float mouseInfluence = 0.35f; // 0..1-ish, how much mouse affects camera
    public float smoothTime = 0.12f;

    CinemachineVirtualCamera vcam;
    CinemachineFramingTransposer framing;
    Vector3 offsetVel;

    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        framing = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (!mainCam) mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (!player || !mainCam || framing == null) return;

        // mouse position in world
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = player.position.z;

        // direction from player to mouse
        Vector3 toMouse = mouseWorld - player.position;

        // scale influence and clamp
        Vector3 targetOffset = Vector3.ClampMagnitude(toMouse * mouseInfluence, maxOffset);

        // smooth
        Vector3 smoothed = Vector3.SmoothDamp(framing.m_TrackedObjectOffset, targetOffset, ref offsetVel, smoothTime);
        framing.m_TrackedObjectOffset = smoothed;
    }
}