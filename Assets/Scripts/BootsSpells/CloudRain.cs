using System;
using UnityEngine;

public class CloudRain : SpellStats
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void TriggerEnter2D(Collider2D collision)
    {
        //TODO: delete only on touchign ground
        Destroy(gameObject);
    }
}
