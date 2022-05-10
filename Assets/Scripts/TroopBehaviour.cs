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
    public float moveSpeed;
    Transform target;
    [SerializeField] float moveAnimMultiplier = 1;

    [Header("Combat")]
    [SerializeField] float attackRange;
    [SerializeField] float attackSpeed;
    [SerializeField] float damage;
    [SerializeField] bool targetsPlayer;
    [SerializeField] float playerDetectionRange;
    [SerializeField] bool dieAfterAttack;
    
    private Animator skeletonAnim;

    bool footstepActive;
    void Start()
    {
        skeletonAnim = transform.GetChild(0).GetComponent<Animator>();

        mode = BehaviourMode.Idle;

        StartCoroutine(Emerge());

        diamond = GameObject.Find("Diamond");

        GetComponent<Health>().deathPitch += Random.Range(-0.2f, 0.2f);
    }

    void Update()
    {
        if (mode == BehaviourMode.Moving)
        {
            SetTarget();
            MoveTowardsTarget();

            transform.GetChild(0).transform.position = transform.position;
            transform.GetChild(0).transform.rotation = transform.rotation;
        }

    }

    void MoveTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;

        Quaternion lookRot = Quaternion.LookRotation(direction);

        skeletonAnim.SetFloat("speedh", moveSpeed);
        skeletonAnim.SetFloat("moveSpeed", moveAnimMultiplier);

        Vector3 lookRotEuler = lookRot.eulerAngles;
        lookRotEuler.x = 0;

        if (!footstepActive)
        {
            StartCoroutine(Footstep());
        }

        Physics.Raycast(transform.position, direction, out RaycastHit hit);

        Debug.DrawRay(transform.position, direction);

        if (hit.collider.CompareTag("Tree"))
        {
            lookRotEuler.y += 90;
        }

        lookRot = Quaternion.Euler(lookRotEuler);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 1 * Time.deltaTime);

        transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            mode = BehaviourMode.Attacking;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Footstep()
    {
        footstepActive = true;

        yield return new WaitForSeconds(moveAnimMultiplier / 2);

        float volume = (1 - (GetNearestPlayerDistance() - 20)) / 30;

        SoundManager.instance.PlaySound("kick", volume, 1, false, false);

        if (footstepActive)
        {
            StartCoroutine(Footstep());
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
        footstepActive = false;
        StopCoroutine(Footstep());

        if (target == null)
        {
            mode = BehaviourMode.Moving;
            yield return null;
        }

        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            skeletonAnim.SetTrigger("Attack1h1");
            skeletonAnim.SetFloat("attackSpeed", 1 / attackSpeed);

            float volume = (1 - (GetNearestPlayerDistance() - 15)) / 30;

            SoundManager.instance.PlaySound("clang", volume, Random.Range(1, 1.2f) * (1 / attackSpeed), false, false);

            target.GetComponent<Health>().TakeDamage(damage);

            if (WaveManager.instance.thorns)
            {
                GetComponent<Health>().TakeDamage(damage / 2);
            }

            if (dieAfterAttack)
            {
                GetComponent<Health>().TakeDamage(Mathf.Infinity);
            }
        }
        else
        {
            mode = BehaviourMode.Moving;
            yield return null;
        }

        yield return new WaitForSeconds(attackSpeed);

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

    float GetNearestPlayerDistance()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        float closestDistance = Mathf.Infinity;
        foreach (GameObject p in players)
        {
            float distanceToPlayer = Vector3.Distance(p.transform.position, transform.position);

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
            }
        }

        return closestDistance;
    }
}
