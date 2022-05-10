using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    [SerializeField] int count;
    [SerializeField] float mapSize;
    [SerializeField] int randomPosMaxAttempts;
    [SerializeField] float minDistance;
    [SerializeField] float randomSize;
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetRandomPos();
            Vector3 spawnRot = Vector3.zero;

            spawnRot.y = Random.Range(0, 359f);

            int prefabIndex = Random.Range(0, prefabs.Length - 1);

            GameObject spawnedTree = Instantiate(prefabs[prefabIndex], spawnPos, Quaternion.Euler(spawnRot));

            spawnedTree.transform.localScale *= Random.Range(1 - randomSize, 1 + randomSize);
        }
    }

    Vector3 GetRandomPos()
    {
        Vector3 randomPos = Vector3.zero;

        for (int i = 0; i < randomPosMaxAttempts; i++)
        {
            randomPos.x = Random.Range(-mapSize, mapSize);
            randomPos.z = Random.Range(-mapSize, mapSize);

            if (Mathf.Abs(randomPos.x) < 10 && Mathf.Abs(randomPos.z) < 10)
            {
                continue;
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, minDistance);
            foreach (Collider c in colliders)
            {
                if (c.CompareTag("Tree"))
                {
                    continue;
                }
            }

            break;
        }


        return randomPos;
    }
}
