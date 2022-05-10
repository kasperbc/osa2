using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] int treeCount;
    [SerializeField] float mapSize;
    [SerializeField] int randomPosMaxAttempts;
    [SerializeField] float treeMinDistance;
    void Start()
    {
        GameObject treePrefab = Resources.Load<GameObject>("Prefabs/Tree");

        for (int i = 0; i < treeCount; i++)
        {
            Vector3 spawnPos = GetRandomPos();
            Vector3 spawnRot = Vector3.zero;

            spawnRot.y = Random.Range(0, 359f);

            GameObject spawnedTree = Instantiate(treePrefab, spawnPos, Quaternion.Euler(spawnRot));

            spawnedTree.transform.localScale *= Random.Range(0.6f, 1.4f);
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

            Collider[] colliders = Physics.OverlapSphere(transform.position, treeMinDistance);
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
