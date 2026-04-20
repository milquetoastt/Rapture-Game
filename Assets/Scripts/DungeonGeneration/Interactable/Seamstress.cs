using UnityEngine;

public class Seamstress : Interactable
{
    public GameObject panel;
    public override void Interact()
    {
        SceneHandler.instance.DoPanel(panel);
        
    }
}
