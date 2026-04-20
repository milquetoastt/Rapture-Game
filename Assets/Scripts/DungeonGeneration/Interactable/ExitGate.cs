using System;
using UnityEngine;

public class ExitGate : Interactable
{
    public bool firstTime = false;
    public GameObject panelSlideUI;
    private void Start()
    {
        SaveData data = GameDataManager.instance.CurrentSave;
        firstTime = data.firstTimeExitGate;
    }

    public override void Interact()
    {
        if (firstTime)
            SceneHandler.instance.DoPanel(panelSlideUI);
        else
            DoExitGate();
    }

    public void DoExitGate()
    {
        SceneHandler.instance.LoadScene("shoutout evan kice great prof", true);
    }
}
