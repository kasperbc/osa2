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
    [SerializeField] float attackSpeed;
    [SerializeField] float damage;
    [SerializeField] bool targetsPlayer;
    [SerializeField] float playerDetectionRange;
    void Start()
    {
        mode = BehaviourMode.Idle;

        StartCoroutine(Emerge());

        diamond = GameObject.Find("Diamond");
    }

    void Update()
    {
        if (mode == BehaviourMode.Moving)
        {
            SetTarget();

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

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            mode = BehaviourMode.Attacking;
            StartCoroutine(Attack());
        }
    }

    void SetTarget()
    {
        if (targetsPlayer)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            GameObject nearestPlayer = players[0];
            foreach (GameObject player in players)
            {
                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

                if (distanceToPlayer <= playerDetectionRange)
                {
                    if (distanceToPlayer < Vector3.Distance(nearestPlayer.transform.position, transform.position))
                        nearestPlayer = player;
                }
            }

            if (nearestPlayer != null)
            {
                target = nearestPlayer.transform;
                return;
            }
        }
        
        target = diamond.transform;
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackSpeed);

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            target.GetComponent<Health>().TakeDamage(damage);
        }
        else
        {
            mode = BehaviourMode.Moving;
            yield return null;
        }

        StartCoroutine(Attack());
    }

    IEnumerator Emerge()
    {
        GetComponent<Collider>().isTrigger = true;

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0;

        Instantiate(spawnParticle, spawnPos, transform.rotation);

        yield return new WaitForSeconds(2);

        GetComponent<Collider>().isTrigger = false;

        gameObject.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        mode = BehaviourMode.Moving;
    }
}
