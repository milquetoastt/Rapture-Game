using System;
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("cooldown")]
    public CooldownIndicatorUI gloveCooldownUI;
    public CooldownIndicatorUI mantleCooldownUI;
    public CooldownIndicatorUI bootCooldownUI;
    public CooldownIndicatorUI hatCooldownUI;
    [Header("pausemenu")]
    public GameObject pauseHUD;
    
    [Header("seamstress")]
    public bool seamstressHasUnlockedASpell;
    public GameObject seamstressUI;
    public Button[] buttons;
    
    [Header("cooldowns")]
    public Image gloveCooldownImage;
    public Image mantleCooldownImage;
    public Image bootCooldownImage;
    public Image hatCooldownImage;
    public Sprite[] gloveImages;
    public Sprite[] mantleImages;
    public Sprite[] bootImages;
    public Sprite[] hatImages;
    
    [Header("deathsequence")]
    public float slowDuration = 1f;
    public GameObject deathPanel;

    [Header("exit gate")]
    public GameObject exitGateUI;

    [Header("spell drop")]
    public GameObject spellDropHUD;
    public SpellDrop currentSpellDrop;
    public TextMeshProUGUI currentSpellName;
    public TextMeshProUGUI newSpellName;
    public TextMeshProUGUI currentSpellCooldown;
    public TextMeshProUGUI newSpellCooldown;
    public TextMeshProUGUI newSpellDamage;
    public TextMeshProUGUI currentSpellDamage;
    
    
    [Header("wardrobe")] 
    public GameObject wardrobeUI;

    public int currentWardrobeType;
    public GameObject gloveUI;
    public GameObject[] gloveButtons;
    
    public GameObject mantleUI;
    public GameObject[] mantleButtons;
    
    public GameObject hatUI;
    public GameObject[] hatButtons;
    
    public GameObject bootUI;
    public GameObject[] bootButtons;
    
    [Header("level-up")]
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
    public GameObject levelUpHUD;
    

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        pauseHUD.SetActive(false);
        seamstressUI.SetActive(false);
        levelUpHUD.SetActive(false);
        wardrobeUI.SetActive(false);
        gloveUI.SetActive(false);
        mantleUI.SetActive(false);
        hatUI.SetActive(false);
        bootUI.SetActive(false);
        spellDropHUD.SetActive(false);
        exitGateUI.SetActive(false);
    }

    public void ReassignCooldownImages()
    {
        gloveCooldownImage.sprite = gloveImages[GloveSpells.instance.currentSpell];
        mantleCooldownImage.sprite = mantleImages[MantleSpells.instance.currentSpell];
        bootCooldownImage.sprite = bootImages[BootSpells.instance.currentSpell];
        hatCooldownImage.sprite = hatImages[HatSpells.instance.currentSpell];
    }

    public void SeamstressSequence(bool hasUnlockedASpell)
    {
        seamstressUI.SetActive(true);
        PanelSlideUI slide = seamstressUI.GetComponent<PanelSlideUI>();
        seamstressHasUnlockedASpell = hasUnlockedASpell;
        bool[] currentUnlocked = new bool[]
        {
            GloveSpells.instance.IsSpellUnlocked(GloveSpells.instance.currentSpell),
            MantleSpells.instance.IsSpellUnlocked(MantleSpells.instance.currentSpell),
            BootSpells.instance.IsSpellUnlocked(BootSpells.instance.currentSpell),
            HatSpells.instance.IsSpellUnlocked(HatSpells.instance.currentSpell)
        };

        for (int i = 0; i < currentUnlocked.Length; i++)
        {
            TextMeshProUGUI text = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            switch (i)
            {
                case 0:
                    text.text = GloveSpells.instance.GetCurrentSpellData().name;
                    break;

                case 1:
                    text.text = MantleSpells.instance.GetCurrentSpellData().name;
                    break;

                case 2:
                    text.text = BootSpells.instance.GetCurrentSpellData().name;
                    break;

                case 3:
                    text.text = HatSpells.instance.GetCurrentSpellData().name;
                    break;
            }
            //if current is unlocked, gray out buttons[i], uninteractable
            if (currentUnlocked[i] || seamstressHasUnlockedASpell)
            {
                TextMeshProUGUI textUnlockedFlag = text.GetComponentInChildren<TextMeshProUGUI>();
                textUnlockedFlag.enabled = true;
                buttons[i].interactable = false;
                buttons[i].image.color = Color.gray2;
            }
            //if not unlocked, show buttons[i] as interactable
            else if (!currentUnlocked[i])
            {
                buttons[i].interactable = true;
                buttons[i].image.color = Color.white;
            }
        }
        slide.SlideIn();
        
    }

    public void SeamstressUnlock(int which)
    {
        if (seamstressHasUnlockedASpell)
            return;
        else
        {
            switch  (which)
            {
                //gloves mantle boots hat
                case 0:
                    GloveSpells.instance.UnlockSpell(GloveSpells.instance.currentSpell);
                    break;
                case 1:
                    MantleSpells.instance.UnlockSpell(MantleSpells.instance.currentSpell);
                    break;
                case 2:
                    BootSpells.instance.UnlockSpell(BootSpells.instance.currentSpell);
                    break;
                case 3:
                    HatSpells.instance.UnlockSpell(HatSpells.instance.currentSpell);
                    break;
            }
            seamstressHasUnlockedASpell = true;
            FindObjectOfType<Seamstress>().hasUnlockedASpell = true;
            CloseSeamstress();
        }
    }

    public void CloseSeamstress()
    {
        PanelSlideUI slide = seamstressUI.GetComponent<PanelSlideUI>();
        slide.SlideOut();
    }

    public void DieSequence()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        ManaResource.instance.allowTimeControl = false;
        float startScale = Time.timeScale;
        float timer = 0f;

        while (timer < slowDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / slowDuration;

            Time.timeScale = Mathf.Lerp(startScale, 0, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(0.35f);
        deathPanel.SetActive(true);
        PanelSlideUI slide = deathPanel.GetComponent<PanelSlideUI>();
        slide.SlideIn();

    }

    public void DoSceneChange(string sceneName)
    {
        SceneHandler.instance.LoadSceneByName(sceneName);
    }

    public void DoExitGame()
    {
        SceneHandler.instance.QuitGame();
    }

    public void ExitGateSequence()
    {
        exitGateUI.SetActive(true);
        PanelSlideUI slide = exitGateUI.GetComponent<PanelSlideUI>();
        slide.SlideIn();
    }

    public void DoExitGate(bool goToNextDungeonLayer = false)
    {
        SaveData data = GameDataManager.instance.CurrentSave;
        if (data.firstTimeExitGate)
            exitGateUI.GetComponent<PanelSlideUI>().SlideOut();

        if (goToNextDungeonLayer)
        {
            int nextDungeonSceneIndex = PlayerController.instance.currentDungeonLayer + 2;
            SceneHandler.instance.LoadSceneByIndex(null, nextDungeonSceneIndex);
        }
        else
        {
            PlayerController.instance.currentDungeonLayer++;
            SceneHandler.instance.LoadSceneByName("In-BetweenDungeons");
        }
    }

    public void WardrobeSequence(int which)
    {
        currentWardrobeType = which;
        wardrobeUI.SetActive(true);
        gloveUI.SetActive(false);
        mantleUI.SetActive(false);
        hatUI.SetActive(false);
        bootUI.SetActive(false);
        PanelSlideUI slide = wardrobeUI.GetComponent<PanelSlideUI>();
        slide.SlideIn();

        switch (which)
        {
            case 0:
                gloveUI.SetActive(true);
                break;
            case 1:
                mantleUI.SetActive(true);
                break;
            case 2:
                bootUI.SetActive(true);
                break;
            case 3:
                hatUI.SetActive(true);
                break;
        }

        BlockLocked(which);
    }

    private void BlockLocked(int which)
    {
        switch (which)
        {
            case 0:
                HandleSpellUI(gloveButtons, which);
                break;
            case 1:
                HandleSpellUI(mantleButtons, which);
                break;
            case 2:
                HandleSpellUI(bootButtons, which);
                break;

            case 3:
                HandleSpellUI(hatButtons, which);
                break;
        }
    }
    
    private void HandleSpellUI(GameObject[] buttons, int which)
    {
        //Debug.Log("buttons length: " + buttons.Length + ", which: " + which);
        for (int i = 0; i < buttons.Length; i++)
        {
            bool unlocked = false;

            switch (which)
            {
                case 0:
                    unlocked = GloveSpells.instance.IsSpellUnlocked(i);
                    break;
                case 1:
                    unlocked = MantleSpells.instance.IsSpellUnlocked(i);
                    break;
                case 2:
                    unlocked = BootSpells.instance.IsSpellUnlocked(i);
                    break;
                case 3:
                    unlocked = HatSpells.instance.IsSpellUnlocked(i);
                    break;
            }
            
            //Debug.Log("button " + i + " unlocked = " + unlocked);
            
            Button btn = buttons[i].GetComponent<Button>();
            if (btn != null)
                btn.interactable = unlocked;

            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
            {
                if (unlocked)
                {
                    img.color = new Color(1f, 1f, 1f, 0.7f);
                    
                }
                else
                {
                    img.color = Color.red;
                }
            }
        }
    }

    public void CloseWardrobe()
    {
        PanelSlideUI slide = wardrobeUI.GetComponent<PanelSlideUI>();
        slide.SlideOut();
        
    }

    

    public void LevelUpSequence()
    {
        levelUpHUD.SetActive(true);
        PanelSlideUI slide = levelUpHUD.GetComponent<PanelSlideUI>();
        slide.SlideIn();
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
            playerBuffAmount = (PlayerController.instance.maxHealth / 10 + Random.Range(0, PlayerController.instance.currentLevel) * 3);
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
        PanelSlideUI slide = levelUpHUD.GetComponent<PanelSlideUI>();
        slide.SlideOut();
    }

    public void SpellDropSequence()
    {
        spellDropHUD.SetActive(true);
        PanelSlideUI slide = spellDropHUD.GetComponent<PanelSlideUI>();
        slide.SlideIn();
        switch (currentSpellDrop.typeSpell)
        {
            case 0: // gloves
            {
                SpellDataG currentData = GloveSpells.instance.GetCurrentSpellData();
                SpellDataG rolledData = GloveSpells.instance.GetCurrentSpellData(currentSpellDrop.whichSpell);

                currentSpellName.text = currentData.name;
                newSpellName.text = rolledData.name;

                currentSpellCooldown.text = "Spell cooldown is " + currentData.fireCooldown.ToString("0.##");
                newSpellCooldown.text = "Spell cooldown is " + rolledData.fireCooldown.ToString("0.##");

                SpellStats stats = currentData.projectilePrefab.GetComponent<SpellStats>();
                SpellStats newStats = rolledData.projectilePrefab.GetComponent<SpellStats>();
                
                currentSpellDamage.text = "Spell damage is " + stats.damage.ToString();
                newSpellDamage.text ="Spell damage is " +  newStats.damage.ToString();
                break;
            }

            case 1: // mantle
            {
                SpellDataM currentData = MantleSpells.instance.GetCurrentSpellData();
                SpellDataM rolledData = MantleSpells.instance.GetCurrentSpellData(currentSpellDrop.whichSpell);

                currentSpellName.text = currentData.name;
                newSpellName.text = rolledData.name;

                currentSpellCooldown.text = "Spell cooldown is " + currentData.fireCooldown.ToString("0.##");
                newSpellCooldown.text = "Spell cooldown is " + rolledData.fireCooldown.ToString("0.##");

                SpellStats stats = currentData.spellPrefab.GetComponent<SpellStats>();
                SpellStats newStats = rolledData.spellPrefab.GetComponent<SpellStats>();
                currentSpellDamage.text = "Spell damage is " + stats.damage.ToString();
                newSpellDamage.text = "Spell damage is " + newStats.damage.ToString();
                break;
            }

            case 2: // boots
            {
                SpellDataB currentData = BootSpells.instance.GetCurrentSpellData();
                SpellDataB rolledData = BootSpells.instance.GetCurrentSpellData(currentSpellDrop.whichSpell);

                currentSpellName.text = currentData.name;
                newSpellName.text = rolledData.name;

                currentSpellCooldown.text = "Spell cooldown is " + currentData.fireCooldown.ToString("0.##");
                newSpellCooldown.text = "Spell cooldown is " + rolledData.fireCooldown.ToString("0.##");
                
                SpellStats stats = currentData.spellPrefab.GetComponent<SpellStats>();
                SpellStats newStats = rolledData.spellPrefab.GetComponent<SpellStats>();
                currentSpellDamage.text = "Spell damage is " + stats.damage.ToString();
                newSpellDamage.text = "Spell damage is " + newStats.damage.ToString();
                break;
            }

            case 3: // hat
            {
                SpellDataH currentData = HatSpells.instance.GetCurrentSpellData();
                SpellDataH rolledData = HatSpells.instance.GetCurrentSpellData(currentSpellDrop.whichSpell);

                currentSpellName.text = currentData.name;
                newSpellName.text = rolledData.name;

                currentSpellCooldown.text = "Spell cooldown is " + currentData.fireCooldown.ToString("0.##");
                newSpellCooldown.text = "Spell cooldown is " + rolledData.fireCooldown.ToString("0.##");
                break;
            }
        }
        
        
    }

    public void SwapWithDropped()
    {
        if (currentSpellDrop == null)
            return;

        int oldSpellIndex = 0;
        int newSpellIndex = currentSpellDrop.whichSpell;

        switch (currentSpellDrop.typeSpell)
        {
            case 0: // gloves
            {
                oldSpellIndex = GloveSpells.instance.currentSpell;
                GloveSpells.instance.SwapSpells(newSpellIndex, true);
                currentSpellDrop.whichSpell = oldSpellIndex;
                break;
            }

            case 1: // mantle
            {
                oldSpellIndex = MantleSpells.instance.currentSpell;
                MantleSpells.instance.SwapSpells(newSpellIndex, true);
                currentSpellDrop.whichSpell = oldSpellIndex;
                break;
            }

            case 2: // boots
            {
                oldSpellIndex = BootSpells.instance.currentSpell;
                BootSpells.instance.SwapSpells(newSpellIndex, true);
                currentSpellDrop.whichSpell = oldSpellIndex;
                break;
            }

            case 3: // hat
            {
                oldSpellIndex = HatSpells.instance.currentSpell;
                HatSpells.instance.SwapSpells(newSpellIndex, true);
                currentSpellDrop.whichSpell = oldSpellIndex;
                break;
            }
        }

        CloseDropped();
    }   

    public void CloseDropped()
    {
        PanelSlideUI slide = spellDropHUD.GetComponent<PanelSlideUI>();
        ReassignCooldownImages();
        slide.SlideOut();
    }

    public void PauseSequence()
    {
        pauseHUD.SetActive(true);
        PanelSlideUI slide = pauseHUD.GetComponent<PanelSlideUI>();
        slide.SlideIn();
    }
    public void ClosePause()
    {
        PanelSlideUI slide = pauseHUD.GetComponent<PanelSlideUI>();
        slide.SlideOut();
    }
    public void SwapSpellFromUI(int index)
    {
        if (PlayerController.instance == null)
            return;

        switch (currentWardrobeType)
        {
            case 0:
                GloveSpells.instance.SwapSpellsFromUI(index);
                break;

            case 1:
                MantleSpells.instance.SwapSpellsFromUI(index);
                break;

            case 2:
                BootSpells.instance.SwapSpellsFromUI(index);
                break;

            case 3:
                HatSpells.instance.SwapSpellsFromUI(index);
                break;
        }
        ReassignCooldownImages();
        CloseWardrobe();
    }
    
    public void StartCooldown(int which, float cooldown)
    {
        switch (which)
        {
            case 0:
                gloveCooldownUI.StartCooldown(cooldown);
                break;

            case 1:
                mantleCooldownUI.StartCooldown(cooldown);
                break;

            case 2:
                bootCooldownUI.StartCooldown(cooldown);
                break;

            case 3:
                hatCooldownUI.StartCooldown(cooldown);
                break;
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}