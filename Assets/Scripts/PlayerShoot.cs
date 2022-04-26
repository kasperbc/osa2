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
    public GameObject crossHair;

    Vector2 rotation = Vector2.zero;

    void Start()
    {
        barrel = gunModel.transform.GetChild(0).gameObject;
    }

    public void Aim(Vector3 direction)
    {
        rotation.x += direction.x;
        rotation.y += direction.y;

        rotation.x = Mathf.Clamp(rotation.x, -30, 0);

        gunModel.transform.eulerAngles = rotation * 2.5f;
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
        Invoke(nameof(DeactivateCooldown), 0.5f);

        // Fire the shell
        Vector3 spawnPos = gunModel.transform.position + gunModel.transform.forward * 2;
        
        Vector3 spawnRot = gunModel.transform.eulerAngles;
        spawnRot.x -= 15;

        GameObject spawnedShell = Instantiate(shell, spawnPos, Quaternion.Euler(spawnRot));
        spawnedShell.GetComponent<Rigidbody>().AddForce((spawnPos - gunModel.transform.position) * 20, ForceMode.Impulse);

        // Play animation
        barrel.GetComponent<Animator>().SetTrigger("Fire");
        barrel.GetComponent<ParticleSystem>().Play();

        // Play reload animation
        reloadBar.GetComponent<Animator>().SetTrigger("Reload");

        SoundManager.instance.PlaySound("fire", 0.6f, Random.Range(0.9f, 1.1f), false, false);
    }

    void DeactivateCooldown()
    {
        onCooldown = false;
    }
}
