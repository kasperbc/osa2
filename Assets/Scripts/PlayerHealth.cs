using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public GameObject explosionPrefab;
    public Image healthBar;
    public float health;
    void Start()
    {
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0)
        {
            health = 0;
            Explode();
        }

        healthBar.fillAmount = health / 100;
    }

    void Explode()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        StartCoroutine(GameManager.instance.DisplayStatus("Game Over!"));

        gameObject.SetActive(false);
    }
}
