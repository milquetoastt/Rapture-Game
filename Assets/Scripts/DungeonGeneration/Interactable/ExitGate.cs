using System;
using UnityEngine;

public class ExitGate : Interactable
{
    public bool firstTime = false;
    public bool tutorial = false;
    public bool goToNextDungeonLayer = false; 
    
    private void Start()
    {
        SaveData data = GameDataManager.instance.CurrentSave;
        firstTime = data.firstTimeExitGate;
    }

    public override void Interact()
    {
        if (tutorial)
        {
            SceneHandler.instance.ReturnToMainMenu();
            return;
        }
        if (firstTime)
            UIManager.instance.ExitGateSequence();
        else
        {
            UIManager.instance.DoExitGate(goToNextDungeonLayer);
        }
    }
}