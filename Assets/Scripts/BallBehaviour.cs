using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float knockback;    // How much force is added to the ball on "kick"
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float kb = 1;
            if (collision.gameObject.GetComponent<PlayerMovement>().Boosting())
            {
                kb = 2;
            }

            KnockBall(collision.transform.position, kb, 0.5f);
        }
    }

    public void KnockBall(Vector3 from, float hitMultiplier, float verticalMultiplier)
    {
        Vector3 hitDirection = (transform.position - from).normalized;
        hitDirection.y *= verticalMultiplier;

        rb.AddForce(hitDirection * knockback * hitMultiplier, ForceMode.Impulse);
    }

    public void KnockBall(Vector3 from)
    {
        KnockBall(from, 1, 1);
    }
}
