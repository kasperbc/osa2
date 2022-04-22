using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public Vector3[] spawnpoints = new Vector3[4];

    public List<Vector3> troopSpawnpoints = new List<Vector3>();
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnpoints[0], 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(spawnpoints[1], 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spawnpoints[2], 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(spawnpoints[3], 0.5f);

        Gizmos.color = Color.black;
        foreach (var point in troopSpawnpoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }
    }
}
