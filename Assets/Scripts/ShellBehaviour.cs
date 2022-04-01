using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject explosion;
    public float followSpeed;
    private void Update()
    {
        transform.LookAt(GameObject.Find("Ball").transform);

        Vector3 followDir = Vector3.forward;

        transform.Translate(followDir * followSpeed * Time.deltaTime);
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

        Destroy(gameObject);
    }
}
