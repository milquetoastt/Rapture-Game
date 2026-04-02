using UnityEngine;

public class ManaBarUI : MonoBehaviour
{

    public RectTransform frameRect;
    public RectTransform fillMaskRect;
    public RectTransform fillRect;

    [Header("sizing")]
    public float baseWidth = 64f;
    public float widthPerMana = 4f;
    public float height = 16f;

    [Header("fill")]
    public float leftInset = 2f;
    public float rightInset = 2f;

    private ManaResource mana;

    private void Start()
    {
        mana = ManaResource.instance;
        UpdateBarSize();
        UpdateFill();
    }

    private void Update()
    {
        UpdateFill();
    }

    float GetTotalFrameWidth()
    {
        return baseWidth + (mana.maxHoldSeconds * widthPerMana);
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
        if (mana == null) return;

        float percent = mana.GetManaPercent();
        float innerWidth = GetInnerFillWidth();

        fillMaskRect.sizeDelta = new Vector2(Mathf.Round(innerWidth * percent), height);
    }
}