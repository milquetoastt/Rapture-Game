using UnityEngine;
using UnityEngine.UI;

public class Seamstress : Interactable
{
    public bool hasUnlockedASpell = false;

    public override void Interact()
    {
        UIManager.instance.SeamstressSequence(hasUnlockedASpell);
        PlayerController.instance.health = PlayerController.instance.maxHealth;
        //hasUnlockedASpell = true;
    }
}
