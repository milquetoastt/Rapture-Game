using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpellDataB
{
    public string name;
    public GameObject spellPrefab;
    public float fireCooldown = 1.5f;
    public float windUp = 0f;
    public Vector3 spawnOffset;
}

public class BootSpells : MonoBehaviour
{
    public SpellDataB[] spells;
    public int currentSpell = 0;
    
    public SpellStats proj;
    

    private float lastInputX = 1f;
    private float nextCastTime = 0f;
    public Transform firePoint;

    public CooldownIndicatorUI bootCooldownUI;
    public static BootSpells instance;
    public WindupRing ring;
    [SerializeField] private bool[] unlockedSpells;

    private void Start()
    {
        LoadFromSave();
    }
    private void LoadFromSave()
    {
        SaveData data = GameDataManager.instance.CurrentSave;

        unlockedSpells = data.bootUnlocked;
        currentSpell = data.currentBootSpell;
    }
    public bool IsSpellUnlocked(int index)
    {
        if (unlockedSpells == null || index < 0 || index >= unlockedSpells.Length)
            return false;

        return unlockedSpells[index];
    }
    public void UnlockSpell(int index)
    {
        if (unlockedSpells == null || index < 0 || index >= unlockedSpells.Length)
        {
            Debug.LogWarning("invalid spell index: " + index);
            return;
        }

        unlockedSpells[index] = true;
        GameDataManager.instance.CurrentSave.gloveUnlocked = unlockedSpells;
        GameDataManager.instance.SaveGame();
    }
    public void SwapSpellsFromUI(int i)
    {
        SwapSpells(i, false);
    }
    public void SwapSpells(int i, bool fromChest = false)
    {
        if (i < 0 || i >= spells.Length)
            return;

        if (!fromChest && !unlockedSpells[i])
            return;

        currentSpell = i;
        GameDataManager.instance.CurrentSave.currentMantleSpell = currentSpell;
        GameDataManager.instance.SaveGame();
    }
    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (UIManager.instance.gameIsPaused)
            return;
        float inputX = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(inputX) > 0.01f)
        {
            lastInputX = Mathf.Sign(inputX);
        }

        Vector2 dir = new Vector2(lastInputX, 0f);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("got boots input down");
            //start windup
            CastBoots(dir);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && proj != null)
        {
            proj.HoldBoot(dir);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && proj != null)
        {
            proj.EndBoot(dir);
            Destroy(proj);
            ring.End();
            SpellDataB spell = spells[currentSpell];
            nextCastTime = Time.time + spell.fireCooldown;
            bootCooldownUI.StartCooldown(spell.fireCooldown);
            
        }
    }


    public void CastBoots(Vector2 direction)
    {
        if (Time.time < nextCastTime) return;
        if (proj != null) return;

        StartCoroutine(CastAfterWindup(direction));
    }

    private IEnumerator CastAfterWindup(Vector2 direction)
    {
        PlayerController.instance.isCasting = true;
        SpellDataB spell = spells[currentSpell];

        ring.Begin(spell.windUp);
        yield return new WaitForSecondsRealtime(spell.windUp);

        Vector2 spawnPos = firePoint.transform.position + spell.spawnOffset;
        GameObject bootSpellCasting = Instantiate(spell.spellPrefab, spawnPos, Quaternion.identity);

        proj = bootSpellCasting.GetComponent<SpellStats>();
        proj.CastBoot(direction);
        PlayerController.instance.isCasting = false;
    }
    
    public SpellDataB GetCurrentSpellData(int i = -1)
    {
        if (i != -1)
        {
            return spells[i];
        }
        return spells[currentSpell];
    }
}