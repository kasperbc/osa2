using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingGoalIndicator : MonoBehaviour
{
    void Update()
    {
        Vector3 rotation = Vector3.up;

        transform.Rotate(rotation * 30 * Time.deltaTime);
    }
}
