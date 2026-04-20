using UnityEngine;

public class Wardrobe : Interactable
{
    public int which;
    public override void Interact()
    {
        UIManager.instance.WardrobeSequence(which);
    }
}
