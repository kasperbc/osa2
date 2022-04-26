using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopBehaviour : MonoBehaviour
{
    public enum BehaviourMode { Idle, Moving, Attacking }
    GameObject diamond;
    public BehaviourMode mode;

    [Header("Spawning")]
    [SerializeField] GameObject spawnParticle;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    Transform target;

    [Header("Combat")]
    [SerializeField] float attackRange;
    void Start()
    {
        mode = BehaviourMode.Idle;

        StartCoroutine(Emerge());

        diamond = GameObject.Find("Diamond");
        target = diamond.transform;
    }

    void Update()
    {
        if (mode == BehaviourMode.Moving)
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;

        Quaternion lookRot = Quaternion.LookRotation(direction);

        Vector3 lookRotEuler = lookRot.eulerAngles;
        lookRotEuler.x = 0;
        lookRot = Quaternion.Euler(lookRotEuler);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 1 * Time.deltaTime);

        transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
    }

    IEnumerator Emerge()
    {
        GetComponent<Collider>().isTrigger = true;

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0;

        Instantiate(spawnParticle, spawnPos, transform.rotation);

        yield return new WaitForSeconds(2);

        GetComponent<Collider>().isTrigger = false;

        mode = BehaviourMode.Moving;
    }
}
