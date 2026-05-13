using UnityEngine;
using System.Collections;

//TODO: eat cake :)

[System.Serializable]
public class SpellDataM
{
    public string name;
    public GameObject spellPrefab;
    public float fireCooldown = 1.5f;
    public float windUp = 0f;
    public Vector3 spawnOffset;
    public float shakeMagnitude = .1f;
    public float shakeLength = .1f;
}

public class MantleSpells : MonoBehaviour
{
    public SpellDataM[] spells;
    public int currentSpell = 0;
    

    private float lastInputX = 1f;
    private float nextCastTime = 0f;
    public Transform firePoint;
    public static MantleSpells instance;
    public bool wasCasting = false;
    public WindupRing ring;

    [SerializeField] private bool[] unlockedSpells;
    private void Start()
    {
        
        LoadFromSave();
    }

    private void LoadFromSave()
    {
        SaveData data = GameDataManager.instance.CurrentSave;

        unlockedSpells = data.mantleUnlocked;
        currentSpell = data.currentMantleSpell;
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
    void Awake()
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

        if (Input.GetKeyDown(KeyCode.LeftControl) && !PlayerController.instance.isCasting)
        {
            Debug.Log("got mantle input");
            CastMantle(dir);
        }
    }


    public void CastMantle(Vector2 direction)
    {
        if (Time.time < nextCastTime) return;
        StartCoroutine(CastAfterWindup(direction, spells[currentSpell]));
    }

    private IEnumerator CastAfterWindup(Vector2 direction, SpellDataM entry)
    {
        
        PlayerController.instance.isCasting = true;
        SpellDataM spell = spells[currentSpell];
        //pre-cast movement
        if (currentSpell != 0)
        {
            Rigidbody2D rbP = PlayerController.instance.GetComponent<Rigidbody2D>();
            rbP.linearVelocity = (rbP.linearVelocity/2);
        }

        ring.Begin(entry.windUp);
        
        yield return new WaitForSecondsRealtime(entry.windUp);
        Vector2 spawnPos = firePoint.transform.position + spell.spawnOffset;
        GameObject mantleSpellCasting = Instantiate(spell.spellPrefab,spawnPos, Quaternion.identity);
        SpellStats proj = mantleSpellCasting.GetComponent<SpellStats>();
        proj.CastMantle(gameObject, direction);
        PlayerController.instance.isCasting = false;
        ring.End();
        
        JuiceManager.instance.CastJuice();
        
        nextCastTime = Time.time + entry.fireCooldown;
        UIManager.instance.StartCooldown(1, spell.fireCooldown);

    }
    
    public SpellDataM GetCurrentSpellData(int i = -1)
    {
        if (i != -1)
        {
            return spells[i];
        }
        return spells[currentSpell];
    }
}