using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArchXploder : EnemyBase
{

    public float timeBetweenMarkers = .75f;
    public int numMarkers = 1;
    public float castExplodeTime = .75f;
    public int numXplosions = 1;
    
    public float castCooldown = 3f;
    public bool isCasting = false;
    private float timer;

    public GameObject marker;
    private List<MarkerXploder> markers = new List<MarkerXploder>();
    
    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (alerted && timer >= castCooldown && !isCasting)
        {
            isCasting = true;
            StartCoroutine(CastSpell());
        }
        CheckForPlayer();
    }

    private IEnumerator CastSpell()
    {
        Debug.Log("CastSpell started");

        for (int i = 0; i < numMarkers; i++)
        {
            yield return new WaitForSeconds(timeBetweenMarkers);

            Debug.Log("spawning marker");

            GameObject markerObj = Instantiate(marker, PlayerController.instance.transform.position, Quaternion.identity);

            MarkerXploder mark = markerObj.GetComponent<MarkerXploder>();

            if (mark == null)
            {
                Debug.LogError("Marker prefab does NOT have MarkerXploder on the root object");
            }
            else
            {
                Debug.Log("marker added");
                markers.Add(mark);
            }
        }
    
        yield return new WaitForSeconds(castExplodeTime);

        Debug.Log("about to detonate markers: " + markers.Count);
    
        foreach (MarkerXploder mark in markers)
        {
            if (mark == null)
            {
                Debug.LogError("marker in list is null");
                continue;
            }

            Debug.Log("calling detonate");
            mark.Detonate();
        }

        markers.Clear();
        timer = 0;
        isCasting = false;
    }
}
