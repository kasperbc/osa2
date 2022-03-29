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
            Vector3 direction = (other.transform.position - transform.position).normalized;

            direction.y = Mathf.Clamp(direction.y, 0, 1);

            other.gameObject.GetComponent<Rigidbody>().AddForce(direction * 30, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
