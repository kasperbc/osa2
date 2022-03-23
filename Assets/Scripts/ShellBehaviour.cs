using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : MonoBehaviour
{
    public float moveSpeed = 10;

    void Update()
    {
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
    }
}
