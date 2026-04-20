using UnityEngine;

public class SpellDrop : Interactable
{   
    public int typeSpell;
    public int whichSpell;

    void Start()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(box, PlayerController.instance.GetComponent<CapsuleCollider2D>(), true);
    }

    public void SetSpell(int type, int which)
    {
        typeSpell = type;
        whichSpell = which;
    }
    
    public override void Interact()
    {
        UIManager.instance.currentSpellDrop = this;
        UIManager.instance.SpellDropSequence();
    }
}
