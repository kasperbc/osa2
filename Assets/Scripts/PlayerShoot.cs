using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    GameObject gunModel;    // The gun model of the tank
    [SerializeField]
    private GameObject shell;   // The shell that the tank fires
    // Update is called once per frame
    void Update()
    {
        // Get the mouse position
        Ray mouseWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseWorldRay, out RaycastHit hit))
        {
            // If the mouse hits anything, look towards the mouse
            Vector3 direction = hit.point - transform.position;
            gunModel.transform.rotation = Quaternion.Slerp(gunModel.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 20);
        }

        // Check if fire button pressed
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(shell, gunModel.transform.position, gunModel.transform.rotation);
    }
}
