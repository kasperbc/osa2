using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(Kill), 1);
    }

    void Kill()
    {
        Destroy(gameObject);
    }
}
