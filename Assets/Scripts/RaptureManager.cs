using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaptureManager : MonoBehaviour
{
    [Header("wind stats")]
    public int windSlashes = 3;
    public int windDamage = 4;
    public float windSlashStunTime = .3f;
    
    [Header("fire stats")]
    public int fireDamage = 2;
    public float fireDuration = 2f;
    public float tickRate = .5f;
    
    [Header("lightning stats")]
    public int lightningDamage = 4;
    public float lightningRadius = 15f;
    
    [Header("water stats")]
    public int waterDamage = 4;
    public float waterStunTime = 2f;
    
    public static RaptureManager instance;
    
    public void Start()
    {
        instance = this;
    }
    
    public void DoRapture(int spellElement, EnemyBase enemy)
    {
        Debug.Log("called DoRapture");
        //wind element = 0
        if (spellElement == 0)
            StartCoroutine(WindRapture(enemy));
        
        //fire element = 1
        else if (spellElement == 1)
            StartCoroutine(FireRapture(enemy));
        
        //lightning element = 2
        else if (spellElement == 2)
            StartCoroutine(LightningRapture(enemy));

        // water element = 3
        else
            StartCoroutine(WaterRapture(enemy));
    }
    
    private IEnumerator WindRapture(EnemyBase enemy)
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < windSlashes; i++)
        {
            Debug.Log("inrapture wind loop");
            if(enemy.canBeStunned)
                enemy.StunCoroutine(windSlashStunTime);
            enemy.TakeHit(windDamage,0.0f, Vector3.zero);
            //JuiceManager.instance.doWindRapture slash juice
            
            yield return new WaitForSeconds(windSlashStunTime);
        }
    }
    
    private IEnumerator FireRapture(EnemyBase enemy)
    {
        //DOT 
        yield return new WaitForSeconds(.2f);
        float timer = 0f;
        float tickRate = .5f;
        
        while (timer < fireDuration)
        {
            //do damage
            if (enemy == null)
                yield break;
            enemy.TakeHit(fireDamage, 0.0f, Vector3.zero);
            yield return new WaitForSeconds(tickRate);
            timer += tickRate;
        }
    }

    private IEnumerator LightningRapture(EnemyBase enemy)
    {
        //look in a radius around enemyposition, if there's an enemy in that area, zap it
        yield return new WaitForSeconds(.2f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, lightningRadius);

        List<EnemyBase> enemies = new List<EnemyBase>();

        foreach (Collider2D hit in hits)
        {
            EnemyBase nme = hit.GetComponent<EnemyBase>();
            if (nme == null) continue;
            //if (nme == enemy) continue;
            enemies.Add(nme);
        }
        //loop through enemies, do hit
        if(enemies.Count > 0)  
            Debug.Log("atleast1enemy");
        foreach (EnemyBase guy in enemies)
        {
            if(guy.canBeStunned)
                guy.StunCoroutine(.1f);
            guy.TakeHit(lightningDamage,0.0f, Vector3.zero);
            //JuiceManager.instance.doLightningRapture slash juice
        }
    }

    private IEnumerator WaterRapture(EnemyBase enemy)
    {
        // deal a chunk of damage
        // stun the enemy (if they can be stunned)
        yield return new WaitForSeconds(.2f);
        if(enemy.canBeStunned)
            enemy.StunCoroutine(waterStunTime);
        enemy.TakeHit(waterDamage,0.0f, Vector3.zero);
    }
    
    
}
