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
    public GameObject reloadBar;    // The reload UI circle
    public GameObject crossHair;

    public int damage;
    public float damageMultiplier = 1;

    public float reloadTime;    // The time it takes for the player to reload
    public float reloadMultiplier = 1;

    public float bulletSpeed;
    public float bulletSpeedMultiplier = 1;

    public int pierceCount;

    public int bulletCount;

    public float bulletSize;

    public int[] levels;

    Vector2 rotation = Vector2.zero;

    void Start()
    {
        barrel = gunModel.transform.GetChild(0).gameObject;

        reloadTime = 0.75f;
        bulletSpeed = 20;
        damage = 20;
        pierceCount = 0;
        bulletCount = 1;
        bulletSize = 1;
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
        Invoke(nameof(DeactivateCooldown), reloadTime * reloadMultiplier);

        for (int i = 0; i < bulletCount; i++)
        {
            // Fire the shell
            Vector3 spawnPos = gunModel.transform.position + gunModel.transform.forward * 2;
        
            Vector3 spawnRot = gunModel.transform.eulerAngles;
            spawnRot.x -= 15 + ((bulletSize - 1) * 15);

            if (i % 2 == 0)
            {
                spawnRot.y -= 30 * i;
            }
            else
            {
                spawnRot.y += 30 * i;
            }

            GameObject spawnedShell = Instantiate(shell, spawnPos, Quaternion.Euler(spawnRot));
            spawnedShell.GetComponent<Rigidbody>().AddForce((spawnPos - gunModel.transform.position) * (bulletSpeed * bulletSpeedMultiplier), ForceMode.Impulse);
            spawnedShell.GetComponent<ShellBehaviour>().damage = damage * damageMultiplier;
            spawnedShell.GetComponent<ShellBehaviour>().pierces = pierceCount;
            spawnedShell.transform.localScale *= bulletSize;
        }

        // Play animation
        barrel.GetComponent<Animator>().SetTrigger("Fire");
        barrel.GetComponent<ParticleSystem>().Play();

        // Play reload animation
        reloadBar.GetComponent<Animator>().SetTrigger("Reload");
        reloadBar.GetComponent<Animator>().SetFloat("ReloadSpeed", 1 / (reloadTime * reloadMultiplier));

        float pitch = 1;
        pitch = Mathf.Clamp(pitch, 0.5f, 1f);
        

        SoundManager.instance.PlaySound("fire", 0.6f, Random.Range(pitch - 0.1f, pitch + 0.1f), false, false);
    }

    void DeactivateCooldown()
    {
        onCooldown = false;
    }
}
