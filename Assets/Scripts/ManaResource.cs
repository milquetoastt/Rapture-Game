using UnityEngine;
using System.Collections;
using Cinemachine;

public class ManaResource : MonoBehaviour
{
    public static ManaResource instance;
    public float slowTimeScale = .2f;
    public float maxHoldSeconds = 5f;
    public float timeScaleLerpSpeed = 12f;
    public float rechargeRate = 1f;
    public bool allowTimeControl = true;
    public float heldSeconds;
    public bool isHolding;
    private float defaultFixedDeltaTime;

    private bool wasHolding;

    private void Awake()
    {
        //on start, remember real time
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        instance = this;
    }

    public float GetManaPercent()
    {
        return (maxHoldSeconds - heldSeconds) / maxHoldSeconds;
    }
    
    public bool UpdateSlowTime(bool wantsHold, bool canHold, bool canRecharge)
    {
        if (!allowTimeControl)
        {
            isHolding = false;
            wasHolding = false;
            return false;
        }
        //wants to hold, mana not empty, and allowed to hold (off cooldown)
        if (wantsHold && heldSeconds < maxHoldSeconds && canHold)
        {
            isHolding = true;
            heldSeconds += Time.unscaledDeltaTime; // real-time countdown
        }
        else
        {
            isHolding = false;
        }

        if (!wantsHold && heldSeconds > 0f && canRecharge)
        {
            heldSeconds -= rechargeRate * Time.unscaledDeltaTime;
            heldSeconds = Mathf.Max(heldSeconds, 0f);
        }
        float targetScale = isHolding ? slowTimeScale : 1f;
        //smooth timescale changes
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetScale, Time.unscaledDeltaTime * timeScaleLerpSpeed);
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

        bool matrixJustEnded = wasHolding && !isHolding;
        wasHolding = isHolding;

        return matrixJustEnded;
    }
    
    public bool TryDrainMana(float amount)
    {
        if (heldSeconds >= maxHoldSeconds)
        {
            heldSeconds = maxHoldSeconds;
            return false;
        }

        heldSeconds += amount;

        if (heldSeconds >= maxHoldSeconds)
        {
            heldSeconds = maxHoldSeconds;
            return false;
        }

        return true;
    }
    
    public bool DrainManaOnly(float amount)
    {
        if (heldSeconds >= maxHoldSeconds)
        {
            heldSeconds = maxHoldSeconds;
            isHolding = false;
            return false;
        }

        isHolding = true;
        heldSeconds += amount;
        heldSeconds = Mathf.Min(heldSeconds, maxHoldSeconds);

        if (heldSeconds >= maxHoldSeconds)
        {
            isHolding = false;
            return false;
        }

        return true;
    }
    public void StopManaHold()
    {
        isHolding = false;
        wasHolding = false;
    }
}