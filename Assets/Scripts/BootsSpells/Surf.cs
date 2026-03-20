using UnityEngine;

public class Surf : SpellStats
{
    public virtual void CastBoot(Vector2 direction)
    {
        Debug.Log("BASE SpellStats CastBoot called");
    }

    public virtual void HoldBoot(Vector2 direction)
    {
        Debug.Log("BASE SpellStats HoldBoot called");
    }
    
    public virtual void EndBoot(Vector2 direction)
    {
        Debug.Log("BASE SpellStats EndBoot called");
    }
}
