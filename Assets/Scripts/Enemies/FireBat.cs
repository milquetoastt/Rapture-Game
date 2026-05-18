using UnityEngine;
using System.Collections;

public class FireBat : EnemyBase
{
    public float castCooldown = 6f;
    public bool isCasting = false;
    public float chargeTime = 2f;
    private float timer;
    void Update()
    {
        CheckForPlayer();
        timer += Time.deltaTime;
        if (alerted && timer >= castCooldown && !isCasting)
        {
            isCasting = true;
            StartCoroutine(CastSpell());
        }
    }

    private IEnumerator CastSpell()
    {
        //get into position in 3 seconds
        
        //lock position and charge for chargeTime
        Vector2 loc =  PlayerController.instance.transform.position;
        yield return new WaitForSeconds(chargeTime);
        
    }
}
