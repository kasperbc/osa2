using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;    // How fast the player moves
    [SerializeField]
    private float turnSpeed;    // How fast the player turns
    [SerializeField]
    private GameObject playerModel;  // The player model that's used for rotation
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(vertical * moveSpeed * Time.deltaTime * Vector3.forward);
        transform.Rotate(horizontal * Time.deltaTime * turnSpeed * Vector3.up);
    }
}
