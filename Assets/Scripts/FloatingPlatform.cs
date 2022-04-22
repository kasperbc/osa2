using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    [Range(0f, 10f)]
    public float platformSpeed;
    [Range(0f, 10f)]
    public float waitTime;

    [SerializeField]
    Vector3[] travelPoints = new Vector3[2];
    [SerializeField]
    bool relativeTravelPoints;
    int travelDestination;

    bool onCooldown;
    void Start()
    {
        if (relativeTravelPoints)
        {
            travelPoints[0] += transform.position;
            travelPoints[1] += transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsNextPoint();
    }

    void MoveTowardsNextPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, travelPoints[travelDestination], platformSpeed * Time.deltaTime);

        if (transform.position == travelPoints[travelDestination] && !onCooldown)
        {
            StartCoroutine(SetNextDestination());
        }
    }

    IEnumerator SetNextDestination()
    {
        onCooldown = true;
        
        yield return new WaitForSeconds(waitTime);

        travelDestination++;
        if (travelDestination >= travelPoints.Length)
        {
            travelDestination = 0;
        }

        onCooldown = false;
    }
}
