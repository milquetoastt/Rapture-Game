using System;
using UnityEngine;
using System.Collections;

//TODO: eat cake :)

[System.Serializable]
public class SpellDataH
{
    public string name;
    public GameObject spellPrefab;
    public float fireCooldown = 1.5f;
    public float windUp = 0f;
    public Vector3 spawnOffset;
}

public class HatSpells : MonoBehaviour
{
    public SpellDataH[] spells;
    public int currentSpell = 0;

    private float lastInputX = 1f;
    private float nextCastTime = 0f;
    public Transform firePoint;

    public static HatSpells instance;
    public WindupRing ring;
    [SerializeField] private bool[] unlockedSpells;
    private void Start()
    {
        LoadFromSave();
    }

    private void LoadFromSave()
    {
        SaveData data = GameDataManager.instance.CurrentSave;

        unlockedSpells = data.hatUnlocked;
        currentSpell = data.currentHatSpell;
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

        if (Input.GetKeyDown(KeyCode.Mouse1) && !PlayerController.instance.isCasting)
        {
            Debug.Log("got hat input");

            Vector3 mouseScreen = Input.mousePosition;
            Vector3 mouseWorld3 = Camera.main.ScreenToWorldPoint(mouseScreen);
            mouseWorld3.z = 0f;
            Vector2 mouseWorld = mouseWorld3;

            CastHat(dir, mouseWorld);
        }
    }

    public void CastHat(Vector2 direction, Vector2 mouseWorld)
    {
        if (Time.time < nextCastTime) return;
        StartCoroutine(CastAfterWindup(direction, mouseWorld, spells[currentSpell]));
    }

    private IEnumerator CastAfterWindup(Vector2 direction, Vector2 mouseWorld, SpellDataH spell)
    {
        PlayerController.instance.isCasting = true;
        ring.Begin(spell.windUp);
        yield return new WaitForSecondsRealtime(spell.windUp);
        ring.End();
        JuiceManager.instance.CastJuice();
        Vector2 spawnPos = (Vector2)firePoint.position + (Vector2)spell.spawnOffset;
        GameObject hatSpellCasting = Instantiate(spell.spellPrefab, spawnPos, Quaternion.identity);

        SpellStats proj = hatSpellCasting.GetComponent<SpellStats>();
        proj.CastHat(mouseWorld);

        nextCastTime = Time.time + spell.fireCooldown;
        UIManager.instance.StartCooldown(2, spell.fireCooldown);
        PlayerController.instance.isCasting = false;
    }
    
    public SpellDataH GetCurrentSpellData(int i = -1)
    {
        if (i != -1)
        {
            return spells[i];
        }
        return spells[currentSpell];
    }
}