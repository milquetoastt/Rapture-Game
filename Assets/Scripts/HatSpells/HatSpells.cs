using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

//TODO: eat cake :)

[System.Serializable]
public class SpellDataH
{
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
    public CooldownIndicatorUI hatCooldownUI;
    

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

        if (Input.GetKeyDown(KeyCode.Mouse1))
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
        yield return new WaitForSecondsRealtime(spell.windUp);

        Vector2 spawnPos = (Vector2)firePoint.position + (Vector2)spell.spawnOffset;
        GameObject hatSpellCasting = Instantiate(spell.spellPrefab, spawnPos, Quaternion.identity);

        SpellStats proj = hatSpellCasting.GetComponent<SpellStats>();
        proj.CastHat(mouseWorld);

        nextCastTime = Time.time + spell.fireCooldown;
        hatCooldownUI.StartCooldown(spell.fireCooldown);
        PlayerController.instance.isCasting = false;
    }
}