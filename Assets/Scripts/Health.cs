using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float health;
    public GameObject healthBar;
    [SerializeField] GameObject dyingParticle;
    [SerializeField] bool destroyOnDeath;
    [SerializeField] bool hideOnDeath;
    public bool dead;
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        GameObject damageTextPrefab = Resources.Load<GameObject>("Prefabs/DamageText");

        Vector3 spawnPos = transform.position;
        spawnPos.y += 2;

        GameObject damageText = Instantiate(damageTextPrefab, spawnPos, transform.rotation);
        damageText.GetComponent<TextMeshPro>().text = amount.ToString();

        if (dead == true)
        {
            return;
        }
        
        health -= amount;

        if (healthBar != null)
            healthBar.GetComponent<Image>().fillAmount = health / maxHealth;

        if (health < 0)
        {
            health = 0;
            Die();
        }
    }

    void Die()
    {
        print("I am dead.");

        dead = true;
        
        if (dyingParticle != null)
        {
            Instantiate(dyingParticle, transform.position, transform.rotation);
        }

        if (hideOnDeath)
        {
            GetComponent<Renderer>().enabled = false;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
