using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpellDataB
{
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
        }
    }


    public void CastBoots(Vector2 direction)
    {
        if (Time.time < nextCastTime) return;
        StartCoroutine(CastAfterWindup(direction));
    }

    private IEnumerator CastAfterWindup(Vector2 direction)
    {
        PlayerController.instance.isCasting = true;
        SpellDataB spell = spells[currentSpell];
        //pre-cast movement would go here if there is any idk i'm writing this before i've even started the first boots spell

        yield return new WaitForSecondsRealtime(spell.windUp);
        Vector2 spawnPos = firePoint.transform.position + spell.spawnOffset;
        GameObject bootSpellCasting = Instantiate(spell.spellPrefab,spawnPos, Quaternion.identity);
        
        proj = bootSpellCasting.GetComponent<SpellStats>();
        proj.CastBoot(direction);
        
        nextCastTime = Time.time + spell.fireCooldown;
        bootCooldownUI.StartCooldown(spell.fireCooldown);
        PlayerController.instance.isCasting = false;
    }
    
    public SpellDataB GetCurrentSpellData()
    {
        return spells[currentSpell];
    }
}