using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;    // How fast the player moves
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

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        if (direction != Vector3.zero)
        {
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 20);
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
