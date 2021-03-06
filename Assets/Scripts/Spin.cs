using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 direction;
    [Range(0f, 100f)]
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(direction * speed * Time.deltaTime);
    }
}