using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public RectTransform frameRect;
    public RectTransform fillMaskRect;
    public RectTransform fillRect;

    [Header("sizing")]
    public float baseWidth = 64f;
    public float widthPerHealth = 4f;
    public float height = 16f;

    [Header("fill")]
    public float leftInset = 2f;
    public float rightInset = 2f;

    private PlayerController player;

    private void Start()
    {
        player = PlayerController.instance;
        UpdateBarSize();
        UpdateFill();
    }

    private void Update()
    {
        UpdateBarSize();
        UpdateFill();
    }

    float GetTotalFrameWidth()
    {
        return baseWidth + (player.maxHealth * widthPerHealth);
    }

    float GetInnerFillWidth()
    {
        return GetTotalFrameWidth() - leftInset - rightInset;
    }

    void UpdateBarSize()
    {
        float frameWidth = GetTotalFrameWidth();
        frameRect.sizeDelta = new Vector2(frameWidth, height);

        float innerWidth = GetInnerFillWidth();
        fillMaskRect.sizeDelta = new Vector2(innerWidth, height);

        fillMaskRect.anchoredPosition = new Vector2(leftInset, 0f);

        fillRect.sizeDelta = new Vector2(innerWidth, height);
        fillRect.anchoredPosition = Vector2.zero;
    }

    void UpdateFill()
    {

        float percent = (float)player.health / player.maxHealth;
        float innerWidth = GetInnerFillWidth();

        fillMaskRect.sizeDelta = new Vector2(Mathf.Round(innerWidth * percent), height);
    }
}