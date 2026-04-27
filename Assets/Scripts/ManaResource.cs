using UnityEngine;

public class ManaResource : MonoBehaviour
{
    public static ManaResource instance;

    [Header("mana")]
    public float maxMana = 5f;
    public float currentManaUsed = 0f;
    public float rechargeRate = 1f;

    [Header("recharge delay")]
    public float normalRechargeDelay = 0.25f;
    public float emptyRechargeDelay = 3f;

    [Header("slow time")]
    public float slowTimeScale = 0.2f;
    public float timeScaleLerpSpeed = 12f;
    public bool allowTimeControl = true;

    public bool isHolding;

    private float rechargeTimer;
    private float defaultFixedDeltaTime;
    private bool wasHolding;

    public float heldSeconds => currentManaUsed;
    public float maxHoldSeconds => maxMana;

    private void Awake()
    {
        instance = this;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        RechargeMana();
    }

    public float GetManaPercent()
    {
        return 1f - (currentManaUsed / maxMana);
    }

    public bool HasMana()
    {
        return currentManaUsed < maxMana;
    }

    public bool DrainMana(float amount)
    {
        if (currentManaUsed >= maxMana)
        {
            currentManaUsed = maxMana;
            StopManaUse();
            rechargeTimer = emptyRechargeDelay;
            return false;
        }

        isHolding = true;
        currentManaUsed += amount;

        if (currentManaUsed >= maxMana)
        {
            currentManaUsed = maxMana;
            isHolding = false;
            rechargeTimer = emptyRechargeDelay;
            return false;
        }

        rechargeTimer = normalRechargeDelay;
        return true;
    }

    public void StopManaUse()
    {
        if (!isHolding) return;

        isHolding = false;

        if (currentManaUsed >= maxMana)
            rechargeTimer = emptyRechargeDelay;
        else
            rechargeTimer = normalRechargeDelay;
    }

    private void RechargeMana()
    {
        if (isHolding) return;
        if (currentManaUsed <= 0f) return;

        if (rechargeTimer > 0f)
        {
            rechargeTimer -= Time.unscaledDeltaTime;
            return;
        }

        currentManaUsed -= rechargeRate * Time.unscaledDeltaTime;
        currentManaUsed = Mathf.Max(currentManaUsed, 0f);
    }

    public bool UpdateSlowTime(bool wantsHold, bool canHold)
    {
        if (!allowTimeControl)
        {
            isHolding = false;
            wasHolding = false;
            return false;
        }

        bool actuallyHolding = wantsHold && canHold && HasMana();

        if (actuallyHolding)
        {
            DrainMana(Time.unscaledDeltaTime);
        }
        else
        {
            StopManaUse();
        }

        float targetScale = isHolding ? slowTimeScale : 1f;

        Time.timeScale = Mathf.Lerp(
            Time.timeScale,
            targetScale,
            Time.unscaledDeltaTime * timeScaleLerpSpeed
        );

        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

        bool manaJustEnded = wasHolding && !isHolding;
        wasHolding = isHolding;

        return manaJustEnded;
    }
}