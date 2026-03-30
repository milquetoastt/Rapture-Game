using System;
using UnityEngine;
using TMPro;
using System.Collections;
using Random = UnityEngine.Random;

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public UpgradeCategory category;
    public UpgradeEffect effect;
    public int amount;
}
[System.Serializable]
public enum UpgradeCategory
{
    Player,
    Glove,
    Mantle,
    Hat,
    Boots
}
[System.Serializable]
public enum UpgradeEffect
{
    MaxHealthUp,
    DoubleJump,
    JumpForceUp,

    GloveDamageUp,
    GloveCooldownDown,

    HatDamageUp,
    HatCooldownDown,

    MantleDamageUp,
    MantleCooldownDown,

    BootsDamageUp,
    BootsCooldownDown
}
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI slot1Description;
    public bool gameIsPaused = false;
    //public TextMeshProUGUI slot2Name;
    //public TextMeshProUGUI slot2Description;
    
    //public TextMeshProUGUI slot3Name;
    //public TextMeshProUGUI slot3Description;

    private int whichBuffType;
    public float playerBuffAmount;
    
    //public List<UpgradeData> spellUpgrades = new List<UpgradeData>();

    private UpgradeData slot1Upgrade;
    private UpgradeData slot2Upgrade;
    private UpgradeData slot3Upgrade;
    public GameObject levelUpHUD;

    public void Awake()
    {
        instance = this;
        
        levelUpHUD.SetActive(false);
    }

    public void LevelUpSequence()
    {
        Time.timeScale = 0;
        gameIsPaused = true;
        ManaResource.instance.allowTimeControl = false;
        levelUpHUD.SetActive(true);
        RollHealthBuffData();
    }

    public void RollHealthBuffData()
    {
        whichBuffType = Random.Range(0, 0);
        //0 is health
        if (whichBuffType == 0)
        {
            // 10% of max health + something between 0 and currentlevel*3
            int playerBuffAmount = (PlayerController.instance.maxHealth / 10 + Random.Range(0, PlayerController.instance.currentLevel) * 3);
            int lowest = PlayerController.instance.maxHealth / 10;
            int highest = PlayerController.instance.maxHealth / 10 + PlayerController.instance.currentLevel * 3;
            slot1Description.text = "Gain " + playerBuffAmount +"("+lowest+" - "+ highest + ") max health.";
        }
        //1 is speed
        //2 is jump strength
        //3 is mana
    }

    public void ChoosePlayerBuff()
    {
        if (whichBuffType == 0)
        {
            Debug.Log("increasing health");
            PlayerController.instance.maxHealth += Mathf.RoundToInt(playerBuffAmount);
            PlayerController.instance.health += Mathf.RoundToInt(playerBuffAmount/2);
        }
        CloseLevelUp();
    }

    public void RollSpellBuffData()
    {
        
    }

    public void CloseLevelUp()
    {
        levelUpHUD.SetActive(false);
        
        
        
        Time.timeScale = 1;
        gameIsPaused = false;
        ManaResource.instance.allowTimeControl = true;
    }
}
