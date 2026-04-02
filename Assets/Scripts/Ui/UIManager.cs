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
    public TextMeshProUGUI spell1Name;
    public TextMeshProUGUI spell1Description;
    public TextMeshProUGUI spell1Synopsis;
    public TextMeshProUGUI spell2Name;
    public TextMeshProUGUI spell2Description;
    public TextMeshProUGUI spell2Synopsis;

    private int whichBuffType;
    private int spellBuffType1;
    private int spellBuffType2;
    private int spellToBeBuffed1;
    private int spellToBeBuffed2;
    
    public float spell1BuffAmount;
    public float spell2BuffAmount;
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
        
        RollSpellBuffData();
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
        //0 is damage, 1 is cooldown
        spellBuffType1 = Random.Range(0, 2);
        spellBuffType2 = Random.Range(0, 2);
        //0 is glove, 1 is mantle
        spellToBeBuffed1 = Random.Range(0, 2);
        //0 is boots, 1 is hat
        spellToBeBuffed2 = Random.Range(0,2);

        //glove
        if (spellToBeBuffed1 == 0)
        {
            SpellDataG spellData = GloveSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.projectilePrefab.GetComponent<SpellStats>();
            //damage
            if (spellBuffType1 == 0)
            {
                spell1Name.text = "Glove Damage";
                //10 % of current damage + something between 0 and currentLevel*3
                int lowest = Mathf.RoundToInt(spellStats.damage * 0.1f);
                int highest = PlayerController.instance.currentLevel * 3;
                spell1BuffAmount = Mathf.RoundToInt(Random.Range(lowest, highest));
                spell1Description.text = $"Glove Spell deals an additional {spell1BuffAmount} damage ({lowest}–{highest}).";
                spell1Synopsis.text = "Current damage: " + spellStats.damage;
            }
            //cooldown 
            else if (spellBuffType1 == 1)
            {
                spell1Name.text = "Glove Cooldown";
                // 5% of current damage + something between 5% of 0 and 2
                float lowest = spellData.fireCooldown * 0.05f;
                float highest = 0.05f * 2;
                spell1BuffAmount = lowest + Random.Range(0, 2 * 0.05f);
                spell1Description.text = $"Glove spell cooldown is reduced by {spell1BuffAmount} seconds ({lowest}-{highest}).";
                spell1Synopsis.text = "Current cooldown: " + spellData.fireCooldown;
            }
        }

        //mantle
        else if (spellToBeBuffed1 == 1)
        {
            SpellDataM spellData = MantleSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.spellPrefab.GetComponent<SpellStats>();
            //damage
            if (spellBuffType1 == 0)
            {
                spell1Name.text = "Mantle Damage";
                //10 % of current damage + something between 0 and currentLevel*3
                int lowest = Mathf.RoundToInt(spellStats.damage * 0.1f);
                int highest = PlayerController.instance.currentLevel * 3;
                spell1BuffAmount = Mathf.RoundToInt(Random.Range(lowest, highest));
                spell1Description.text = $"Mantle Spell deals an additional {spell1BuffAmount} damage ({lowest}–{highest}).";
                spell1Synopsis.text = "Current damage: " + spellStats.damage;
            }

            //cooldown
            if (spellBuffType1 == 1)
            {
                spell1Name.text = "Mantle Cooldown";
                // 5% of current damage + something between 5% of 0 and 2
                float lowest = spellData.fireCooldown * 0.05f;
                float highest = 0.05f * 2;
                spell1BuffAmount = lowest + Random.Range(0, 2 * 0.05f);
                spell1Description.text = $"Mantle spell cooldown is reduced by {spell1BuffAmount} seconds ({lowest}-{highest}).";
                spell1Synopsis.text = "Current cooldown: " + spellData.fireCooldown;
            }
        }

        //boots
        if (spellToBeBuffed2 == 0)
        {
            SpellDataB spellData = BootSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.spellPrefab.GetComponent<SpellStats>();

            // damage
            if (spellBuffType2 == 0)
            {
                spell2Name.text = "Boot Damage";

                int lowest = Mathf.RoundToInt(spellStats.damage * 0.1f);
                int highest = PlayerController.instance.currentLevel * 3;
                spell2BuffAmount = Mathf.RoundToInt(Random.Range(lowest, highest));
                spell2Description.text = $"Boot Spell deals an additional {spell2BuffAmount} damage ({lowest}–{highest}).";
                spell2Synopsis.text = "Current damage: " + spellStats.damage;
            }
            // cooldown
            else if (spellBuffType2 == 1)
            {
                spell2Name.text = "Boot Cooldown";
                float lowest = spellData.fireCooldown * 0.05f;
                float highest = 0.05f * 2f;
                spell2BuffAmount = lowest + Random.Range(0f, 2f * 0.05f);
                spell2Description.text = $"Boot spell cooldown is reduced by {spell2BuffAmount} seconds ({lowest}-{highest}).";
                spell2Synopsis.text = "Current cooldown: " + spellData.fireCooldown;
            }
        }
        
        //hat
        else if (spellToBeBuffed2 == 1)
        {
            SpellDataH spellData = HatSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.spellPrefab.GetComponent<SpellStats>();

            // cooldown
            {
                spell2Name.text = "Hat Cooldown";
                float lowest = spellData.fireCooldown * 0.05f;
                float highest = 0.05f * 2f;
                spell2BuffAmount = lowest + Random.Range(0f, 2f * 0.05f);
                spell2Description.text = $"Hat spell cooldown is reduced by {spell2BuffAmount} seconds ({lowest}-{highest}).";
                spell2Synopsis.text = "Current cooldown: " + spellData.fireCooldown;
            }
        }
    }

    public void ChooseSpellBuff1()
    {
        //glove
        if (spellToBeBuffed1 == 0)
        {
            SpellDataG spellData = GloveSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.projectilePrefab.GetComponent<SpellStats>();
            if (spellBuffType1 == 0)
            {
                Debug.Log("buffing glove damage");
                spellStats.damage += (int)spell1BuffAmount;
            }
            //cooldown
            else if (spellBuffType1 == 1)
            {
                Debug.Log("lowering glove cooldown");
                spellData.fireCooldown -= spell1BuffAmount;
            }
        }
        
        //mantle
        if (spellToBeBuffed1 == 1)
        {
            SpellDataM spellData = MantleSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.spellPrefab.GetComponent<SpellStats>();
            if (spellBuffType1 == 0)
            {
                Debug.Log("buffing mantle damage");
                spellStats.damage += (int)spell1BuffAmount;
            }
            //cooldown
            else if (spellBuffType1 == 1)
            {
                Debug.Log("lowering mantle cooldown");
                spellData.fireCooldown -= spell1BuffAmount;
            }
        }
        
        CloseLevelUp();
    }

    public void ChooseSpellBuff2()
    {
        //boot
        if (spellToBeBuffed2 == 0)
        {
            SpellDataB spellData = BootSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.spellPrefab.GetComponent<SpellStats>();
            if (spellBuffType2 == 0)
            {
                Debug.Log("buffing boot damage");
                spellStats.damage += (int)spell2BuffAmount;
            }
            //cooldown
            else if (spellBuffType2 == 1)
            {
                Debug.Log("lowering boot cooldown");
                spellData.fireCooldown -= spell2BuffAmount;
            }
        }
        
        //hat
        if (spellToBeBuffed2 == 1)
        {
            SpellDataH spellData = HatSpells.instance.GetCurrentSpellData();
            SpellStats spellStats = spellData.spellPrefab.GetComponent<SpellStats>();


            Debug.Log("lowering hat cooldown");
            spellData.fireCooldown -= spell2BuffAmount;
        }
        
        CloseLevelUp();
    }

    public void CloseLevelUp()
    {
        levelUpHUD.SetActive(false);
        
        Time.timeScale = 1;
        gameIsPaused = false;
        ManaResource.instance.allowTimeControl = true;
    }
}
