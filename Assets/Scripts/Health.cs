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
    public TMPro.TextMeshProUGUI healthText;
    [SerializeField] GameObject dyingParticle;
    [SerializeField] bool destroyOnDeath;
    [SerializeField] bool hideOnDeath;
    public bool dead;
    public int damageScore;
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (dead == true)
        {
            return;
        }
        
        health -= amount;

        UpdateHealthUI();

        if (health < 0)
        {
            health = 0;
            Die();
        }

        GameManager.instance.AddScore(damageScore);
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

    public void AddMaxHealth(float value)
    {
        maxHealth += value;
    }

    public void FullHeal()
    {
        health = maxHealth;

        UpdateHealthUI();
    }

    public void Heal(float value)
    {
        health += value;

        health = Mathf.Clamp(health, 0, maxHealth);

        UpdateHealthUI();
    }

    public void MultiplyMaxHealth(float value)
    {
        maxHealth *= value;
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.GetComponent<Image>().fillAmount = health / maxHealth;

        if (healthText != null)
            healthText.text = health + "/" + maxHealth;
    }
}
