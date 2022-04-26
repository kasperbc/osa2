using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondBehaviour : MonoBehaviour
{
    Health health;
    void Start()
    {
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.dead)
        {

        }
    }
}
