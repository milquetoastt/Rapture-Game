using UnityEngine;
using System.Collections;
using Cinemachine;

[System.Serializable]
public class SpellDataG
{
    public float recoil = 10f;
    public float fireCooldown = 1.5f;
    public GameObject projectilePrefab;
    public float spawnOffset = 0f;
    public float windUp = 0f;
}

public class GloveSpells : MonoBehaviour
{
    [Header("Spell Settings")]
    public SpellDataG[] spells;
    public int currentSpell;

    [Header("Post-Fire Recovery")]
    public float postFirePause = 0.5f;

    private float postFireTimer;
    private bool inPostFirePause;

    public Camera cam;
    public Transform firePoint;

    private float nextFireTime;

    private Rigidbody2D rb;
    public ManaResource manaResource;
    private bool gloveCastInProgress = false;
    
    public static GloveSpells instance;
    public CooldownIndicatorUI gloveCooldownUI;

    [SerializeField] private bool[] unlockedSpells;
    public bool IsSpellUnlocked(int index)
    {
        Debug.Log("unlockedSpells length: " + unlockedSpells.Length);
        Debug.Log("spells length: " + spells.Length);
        return unlockedSpells[index];
    }
    public void UnlockSpell(int index)
    {
        if (unlockedSpells == null || index < 0 || index >= unlockedSpells.Length)
        {
            Debug.LogWarning("Invalid spell index: " + index);
            return;
        }

        unlockedSpells[index] = true;
    }

    public void SwapSpells(int i)
    {
        if (unlockedSpells[i] == false)
            return;
        currentSpell = i;
    }

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        instance = this;

        if (manaResource == null)
            manaResource = GetComponent<ManaResource>();

        if (unlockedSpells == null || unlockedSpells.Length != spells.Length)
            unlockedSpells = new bool[spells.Length];

        if (spells.Length > 0)
            UnlockSpell(0);
    }

    public void HandleInput()
    {
        if (UIManager.instance.gameIsPaused)
            return;
        SpellDataG spell = spells[currentSpell];

        if (PlayerController.instance.isCasting && !gloveCastInProgress)
            return;

        bool wantsHold = Input.GetMouseButton(0);

        // Minimum fix: only claim casting when this glove cast actually begins
        if (wantsHold && !gloveCastInProgress && !inPostFirePause && Time.time >= nextFireTime)
        {
            gloveCastInProgress = true;
            PlayerController.instance.isCasting = true;
        }

        if (inPostFirePause)
        {
            postFireTimer -= Time.unscaledDeltaTime;

            if (postFireTimer <= 0f)
            {
                inPostFirePause = false;
                postFireTimer = 0f;
            }
        }

        bool matrixJustEnded = false;

        if (manaResource != null)
        {
            matrixJustEnded = manaResource.UpdateSlowTime(
                wantsHold,
                Time.time >= nextFireTime && !inPostFirePause,
                !inPostFirePause
            );
        }

        if (matrixJustEnded && Time.time >= nextFireTime && manaResource.heldSeconds >= manaResource.maxHoldSeconds)
        {
            nextFireTime = Time.time + spell.fireCooldown;
            gloveCooldownUI.StartCooldown(spell.fireCooldown);
            ShootAtMouse();
            ApplyRecoilFromMouse();
            EnterPostFirePause();
        }

        if (Input.GetMouseButtonUp(0) && Time.time >= nextFireTime)
        {
            StartCoroutine(FireAfterWindup(spell, manaResource.heldSeconds));
        }
    }

    private IEnumerator FireAfterWindup(SpellDataG spell, float heldTime)
    {
        float remainingWindUp = Mathf.Max(0f, spell.windUp - heldTime);

        yield return new WaitForSecondsRealtime(remainingWindUp);

        ShootAtMouse();
        ApplyRecoilFromMouse();
        EnterPostFirePause();

        nextFireTime = Time.time + spell.fireCooldown;
        gloveCooldownUI.StartCooldown(spell.fireCooldown);
    }

    public void ApplyRecoilFromMouse()
    {
        //Debug.Log("rb: " + rb);
        //Debug.Log("Camera.main: " + Camera.main);
        //Debug.Log("spell null? " + (spells[currentSpell] == null));
        SpellDataG spell = spells[currentSpell];

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 awayFromMouse =
            ((Vector2)transform.position - (Vector2)mouseWorld).normalized;

        if (spell.recoil > 0f)
            rb.linearVelocity = Vector2.zero;

        rb.AddForce(awayFromMouse * spell.recoil, ForceMode2D.Impulse);
    }

    private void ShootAtMouse()
    {
        SpellDataG spell = spells[currentSpell];

        Debug.Log("Current spell index: " + currentSpell);
        Debug.Log("Projectile prefab: " + spell.projectilePrefab.name);
        Vector3 mouseScreen = Input.mousePosition;

        // Convert mouse to world point (important in 2D)
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - firePoint.position);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        Vector2 spawnPos = (Vector2)firePoint.position + dir.normalized * spell.spawnOffset;

        GameObject projGO = Instantiate(spell.projectilePrefab, spawnPos, rot);

        SpellStats proj = projGO.GetComponent<SpellStats>();
        proj.Fire(dir);
    }

    void EnterPostFirePause()
    {
        gloveCastInProgress = false;
        PlayerController.instance.isCasting = false;
        inPostFirePause = true;
        postFireTimer = postFirePause;
    }

    public SpellDataG GetCurrentSpellData()
    {
        return spells[currentSpell];
    }
}