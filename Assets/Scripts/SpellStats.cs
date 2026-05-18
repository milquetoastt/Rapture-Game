using UnityEngine;

public class SpellStats : MonoBehaviour
{

    public int damage = 10;
    public float knockback = 0f;
    public float stunTime = .5f;
    public int element;
    //0 is wind, 1 is lightning, 2 is fire, 3 is water
    public int castType;
    //0 is glove, 1 is mantle, 2 is boots, 3 is hat

    public virtual void Fire(Vector2 dir) {
        Debug.Log("BASE SpellStats Fire called");
    }

    public virtual void CastMantle(GameObject caster, Vector2 direction)
    {
        Debug.Log("BASE SpellStats CastMantle called");
    }
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

    public virtual void CastHat(Vector2 mouseWorld)
    {
        Debug.Log("BASE SpellStats CastHat called");
    }
}
