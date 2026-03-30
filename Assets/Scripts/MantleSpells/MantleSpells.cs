using UnityEngine;
using System.Collections;

//TODO: eat cake :)

[System.Serializable]
public class SpellDataM
{
    public GameObject spellPrefab;
    public float fireCooldown = 1.5f;
    public float windUp = 0f;
    public Vector3 spawnOffset;
}

public class MantleSpells : MonoBehaviour
{
    public SpellDataM[] spells;
    public int currentSpell = 0;
    

    private float lastInputX = 1f;
    private float nextCastTime = 0f;
    public Transform firePoint;
    public CooldownIndicatorUI mantleCooldownUI;
    

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

        if (Input.GetKeyDown(KeyCode.LeftControl))
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
        yield return new WaitForSecondsRealtime(entry.windUp);
        Vector2 spawnPos = firePoint.transform.position + spell.spawnOffset;
        GameObject mantleSpellCasting = Instantiate(spell.spellPrefab,spawnPos, Quaternion.identity);
        
        SpellStats proj = mantleSpellCasting.GetComponent<SpellStats>();
        proj.CastMantle(gameObject, direction);
        
        nextCastTime = Time.time + entry.fireCooldown;
        mantleCooldownUI.StartCooldown(spell.fireCooldown);

    }
}