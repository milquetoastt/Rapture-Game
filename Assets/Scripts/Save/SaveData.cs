using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool[] gloveUnlocked;
    public bool[] mantleUnlocked;
    public bool[] hatUnlocked;
    public bool[] bootUnlocked;

    public int currentGloveSpell;
    public int currentMantleSpell;
    public int currentHatSpell;
    public int currentBootSpell;

    public string gloveKey = "test";
    public string mantleKey = "test";
    public string bootKey = "test";
    public string hatKey = "test";
}