using UnityEngine;

public class XPBarUI : MonoBehaviour
{
    public RectTransform frameRect;
    public RectTransform fillMaskRect;
    public RectTransform fillRect;

    [Header("sizing")]
    public float baseWidth = 64f;
    public float widthPerXP = 4f;
    public float height = 4f;
    public float fillHeight = 4f;

    [Header("fill")]
    public float leftInset = 2f;
    public float rightInset = 2f;

    private PlayerController player;

    private void Start()
    {
        player = PlayerController.instance;
        float frameWidth = GetTotalFrameWidth();
        frameRect.sizeDelta = new Vector2(frameWidth, height);

        float innerWidth = GetInnerFillWidth();
        fillMaskRect.sizeDelta = new Vector2(innerWidth,height);

        fillMaskRect.anchoredPosition = new Vector2(leftInset, 0f);

        fillRect.sizeDelta = new Vector2(innerWidth, fillHeight);
        fillRect.anchoredPosition = Vector2.zero;
        UpdateFill();
    }

    private void Update()
    {
        UpdateFill();
    }

    float GetTotalFrameWidth()
    {
        return baseWidth; //+ (player.maxHealth * widthPerHealth);
    }

    float GetInnerFillWidth()
    {
        return GetTotalFrameWidth() - leftInset - rightInset;
    }


    void UpdateFill()
    {

        float percent = (float)player.currentXp / player.xpForNextLevel;
        float innerWidth = GetInnerFillWidth();
        
        fillMaskRect.sizeDelta = new Vector2(Mathf.Round(innerWidth * percent), height);
    }
}
