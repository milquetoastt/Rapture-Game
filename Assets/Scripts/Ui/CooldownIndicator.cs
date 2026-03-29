using UnityEngine;
using UnityEngine.UI;

public class CooldownIndicatorUI : MonoBehaviour
{
    public RectTransform spinnerPivot;
    public GameObject spinnerHandObject;
    public Image dimOverlay;

    private float cooldownDuration;
    private float cooldownRemaining;
    private bool isCoolingDown;

    private void Start()
    {
        SetReadyVisual();
    }

    private void Update()
    {
        if (!isCoolingDown) return;

        cooldownRemaining -= Time.deltaTime;

        if (cooldownRemaining <= 0f)
        {
            cooldownRemaining = 0f;
            isCoolingDown = false;
            SetReadyVisual();
            return;
        }

        UpdateVisual();
    }

    public void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownRemaining = duration;
        isCoolingDown = true;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        float percent = 1f - (cooldownRemaining / cooldownDuration);

        float angle = Mathf.Lerp(0f, -360f, percent);

        if (spinnerPivot != null)
            spinnerPivot.localEulerAngles = new Vector3(0f, 0f, angle);

        if (spinnerHandObject != null)
            spinnerHandObject.SetActive(true);

        if (dimOverlay != null)
            dimOverlay.enabled = true;
    }

    private void SetReadyVisual()
    {
        if (spinnerPivot != null)
            spinnerPivot.localEulerAngles = Vector3.zero;

        if (spinnerHandObject != null)
            spinnerHandObject.SetActive(false);

        if (dimOverlay != null)
            dimOverlay.enabled = false;
    }
}