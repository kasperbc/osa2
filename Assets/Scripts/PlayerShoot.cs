using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    GameObject gunModel;    // The gun model of the tank
    GameObject barrel;      // The gun barrel
    [SerializeField]
    private GameObject shell;   // The shell that the tank fires
    public Camera cam;                 // The camera that is following the player
    private bool onCooldown;    // Is the tank fire on cooldown/reloading?
    [SerializeField] bool p2;   // Is the tank player 2?
    public GameObject reloadBar;    // The reload UI circle

    void Start()
    {
        barrel = gunModel.transform.GetChild(0).gameObject;
    }

    void MouseAim()
    {
        // Get the mouse position
        Ray mouseWorldRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseWorldRay, out RaycastHit hit))
        {
            // If the mouse hits anything, look towards the mouse
            Vector3 direction = hit.point - transform.position;
            gunModel.transform.rotation = Quaternion.Slerp(gunModel.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 20);
        }
    }

    void KeyboardAim()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
            direction.x = -35;
        if (Input.GetKey(KeyCode.LeftArrow))
            direction.y = -45;
        else if (Input.GetKey(KeyCode.RightArrow))
            direction.y = 45;

        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction.y /= 2;
            direction.x /= 2;
        }

        

        gunModel.transform.rotation = Quaternion.Euler(direction);
    }

    public void Aim(Quaternion direction, bool relativeToOwnRotation)
    {
        if (relativeToOwnRotation)
        {
            Vector3 relativeDirection = transform.rotation.eulerAngles + direction.eulerAngles;
            direction = Quaternion.Euler(relativeDirection);
        }

        gunModel.transform.rotation = Quaternion.Slerp(gunModel.transform.rotation, direction, Time.deltaTime * 20);
    }

    public void Shoot()
    {
        // Check if barrel on cooldown, don't fire if yes
        if (onCooldown)
        {
            return;
        }

        // Activate cooldown
        onCooldown = true;
        Invoke("DeactivateCooldown", 0.5f);

        // Fire the shell
        Vector3 spawnPos = gunModel.transform.position + gunModel.transform.forward * 2;
        GameObject spawnedShell = Instantiate(shell, spawnPos, gunModel.transform.rotation);
        spawnedShell.GetComponent<Rigidbody>().AddForce((spawnPos - gunModel.transform.position) * 20, ForceMode.Impulse);

        // Play animation
        barrel.GetComponent<Animator>().SetTrigger("Fire");
        barrel.GetComponent<ParticleSystem>().Play();

        // Play reload animation
        reloadBar.GetComponent<Animator>().SetTrigger("Reload");
    }

    void DeactivateCooldown()
    {
        onCooldown = false;
    }
}
