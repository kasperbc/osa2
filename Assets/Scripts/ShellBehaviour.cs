using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject explosion;
    public float followSpeed;

    Transform ball;
    void Start()
    {
        GameObject b = GameObject.Find("Ball");
        if (b != null)
        {
            ball = b.transform;
        }
    }

    void Update()
    {
        if (ball != null)
        {
            //SteerTowardsObject(ball, 30);
        }

        Vector3 followDir = Vector3.forward;

        transform.Translate(followDir * followSpeed * Time.deltaTime);
    }

    void SteerTowardsObject(Transform target, float maxAngle)
    {
        Vector3 relativePos = target.position - transform.position;

        Quaternion rotationToObject = Quaternion.LookRotation(relativePos);
        float angleToObject = Quaternion.Angle(rotationToObject, transform.rotation);

        if (angleToObject < maxAngle)
        {
            transform.LookAt(target);
        }
    }
    void SteerTowardsObject(Transform target)
    {
        SteerTowardsObject(target, 360);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bounds")
        {
            return;
        }

        Instantiate(explosion, transform.position, transform.rotation);

        if (other.gameObject.CompareTag("Ball"))
        {
            other.gameObject.GetComponent<BallBehaviour>().KnockBall(transform.position, 1, 1);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Vector3 hitDirection = (transform.position - other.transform.position).normalized;
            hitDirection.y *= 1.5f;

            other.gameObject.GetComponent<Rigidbody>().AddForce(hitDirection * 2, ForceMode.Impulse);
        }

        SoundManager.instance.PlaySound("explosion", 0.15f, Random.Range(0.65f, 0.75f), false, false);

        Destroy(gameObject);
    }
}
