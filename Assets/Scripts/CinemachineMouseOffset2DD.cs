using UnityEngine;
using Cinemachine;

public class CinemachineMouseOffset2DD : MonoBehaviour
{
    public Transform player;
    public Camera mainCam;

    [Header("feel")]
    public float maxOffset = 2.5f;
    public float mouseInfluence = 0.35f;
    public float smoothTime = 0.12f;

    [Header("screen shake")]
    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.2f;
    public float shakeFrequency = 25f;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer framing;
    private Vector3 offsetVel;

    private float currentShakeTime;
    private float currentShakeMagnitude;

    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        framing = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (mainCam == null)
        {
            Debug.Log("finding camera");
            mainCam = Camera.main;
        }

        if (player == null && PlayerController.instance != null)
        {
            Debug.Log("finding player");
            player = PlayerController.instance.transform;
        }

        if (!mainCam) mainCam = Camera.main;
    }
    
    void Start()
    {
        if (vcam == null)
            vcam = GetComponent<CinemachineVirtualCamera>();

        if (vcam.Follow == null && PlayerController.instance != null)
        {
            vcam.Follow = PlayerController.instance.transform;
        }
    }

    void LateUpdate()
    {
        if (!player || !mainCam || framing == null) return;

        // mouse position in world
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = player.position.z;

        // direction from player to mouse
        Vector3 toMouse = mouseWorld - player.position;

        // base camera offset from mouse
        Vector3 targetOffset = Vector3.ClampMagnitude(toMouse * mouseInfluence, maxOffset);

        // smooth toward mouse offset
        Vector3 smoothed = Vector3.SmoothDamp(
            framing.m_TrackedObjectOffset,
            targetOffset,
            ref offsetVel,
            smoothTime
        );

        // add shake
        Vector3 shakeOffset = Vector3.zero;

        if (currentShakeTime > 0)
        {
            float damper = currentShakeTime / shakeDuration;

            float shakeX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f;
            float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * 2f;

            shakeOffset = new Vector3(shakeX, shakeY, 0f) * currentShakeMagnitude * damper;

            currentShakeTime -= Time.deltaTime;
        }

        framing.m_TrackedObjectOffset = smoothed + shakeOffset;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        currentShakeTime = duration;
        currentShakeMagnitude = magnitude;
    }
}