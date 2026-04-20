using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TreasureChest : Interactable
{
    public GameObject spellPrefab;
    public int typeSpell;
    public int whichSpell;
    public float force;
    public bool rolled = false;

    public override void Interact()
    {
        if (rolled)
        {
            return;
        }
        rolled = true;
        //typeSpell 0=gloves,1=mantle,2=boots,3=hat
        typeSpell = Random.Range(0, 4);
        //whichSpell 0=wind,1=lightning,2=fire,3=water
        whichSpell = Random.Range(0, 4);
        Vector3 position = transform.position + new Vector3(0,1,0);
        GameObject spawned = Instantiate(spellPrefab, position, Quaternion.identity);
        Rigidbody2D rb = spawned.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);   
        SpellDrop pickup = spawned.GetComponent<SpellDrop>();
        pickup.SetSpell(typeSpell, whichSpell);
    }
}
