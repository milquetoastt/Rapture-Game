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
    public float manaDrainPerSecond = 1f;
    
}

public class BootSpells : MonoBehaviour
{
    public SpellDataB[] spells;
    public int currentSpell = 0;

    public SpellStats proj;

    private float lastInputX = 1f;
    private float nextCastTime = 0f;
    public Transform firePoint;

    public static BootSpells instance;
    public WindupRing ring;
    [SerializeField] private bool[] unlockedSpells;

    private bool bootIsActive = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadFromSave();
    }

    private void LoadFromSave()
    {
        SaveData data = GameDataManager.instance.CurrentSave;

        unlockedSpells = data.bootUnlocked;
        currentSpell = data.currentBootSpell;
        UIManager.instance.ReassignCooldownImages();
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
        GameDataManager.instance.CurrentSave.bootUnlocked = unlockedSpells;
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
        GameDataManager.instance.CurrentSave.currentBootSpell = currentSpell;
        GameDataManager.instance.SaveGame();
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && !PlayerController.instance.isCasting)
        {
            CastBoots(dir);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && proj != null)
        {
            proj.HoldBoot(dir);

            SpellDataB spell = spells[currentSpell];
            bool stillHasMana = ManaResource.instance.DrainMana(spell.manaDrainPerSecond * Time.unscaledDeltaTime);

            if (!stillHasMana)
            {
                StopBootSpell(dir);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && proj != null)
        {
            StopBootSpell(dir);
            
        }
    }

    public void CastBoots(Vector2 direction)
    {
        if (Time.time < nextCastTime) return;
        if (proj != null) return;
        PlayerController.instance.isCasting = true;
        StartCoroutine(CastAfterWindup(direction));
    }

    private IEnumerator CastAfterWindup(Vector2 direction)
    {
        PlayerController.instance.isCasting = true;
        SpellDataB spell = spells[currentSpell];

        ring.Begin(spell.windUp);
        yield return new WaitForSecondsRealtime(spell.windUp);

        Vector2 spawnPos = (Vector2)firePoint.position + (Vector2)spell.spawnOffset;
        GameObject bootSpellCasting = Instantiate(spell.spellPrefab, spawnPos, Quaternion.identity);

        proj = bootSpellCasting.GetComponent<SpellStats>();
        proj.CastBoot(direction);
        JuiceManager.instance.CastJuice();
        bootIsActive = true;
        
    }

    private void StopBootSpell(Vector2 direction)
    {
        PlayerController.instance.isCasting = false;
        if (proj != null)
        {
            proj.EndBoot(direction);
            Destroy(proj.gameObject);
            proj = null;
        }
        ManaResource.instance.StopManaUse();
        bootIsActive = false;
        ring.End();

        SpellDataB spell = spells[currentSpell];
        nextCastTime = Time.time + spell.fireCooldown;
        UIManager.instance.StartCooldown(3, spell.fireCooldown);
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