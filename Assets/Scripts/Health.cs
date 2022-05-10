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
    [SerializeField] string animationTrigger;
    public bool dead;
    public int damageScore;
    public bool stunnable;
    public string deathSound;
    [Range(0.1f, 4f)]
    public float deathPitch;
    public string spawnSound;
    [Range(0.1f, 4f)]
    public float spawnPitch;
    public string damageSound;
    [Range(0.1f, 4f)]
    public float damagePitch;

    void Start()
    {
        health = maxHealth;

        if (spawnSound != string.Empty)
        {
            SoundManager.instance.PlaySound(spawnSound, 1, spawnPitch, false, false);
        }
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

        SoundManager.instance.PlaySound(damageSound, 1, damagePitch, false, false);

        if (gameObject.CompareTag("Troop") && stunnable)
        {
            StartCoroutine(StunTroop());
        }

        GameManager.instance.AddScore(damageScore);
    }

    IEnumerator StunTroop()
    {
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hit1");

        GetComponent<TroopBehaviour>().enabled = false;

        yield return new WaitForSeconds(0.5f);

        GetComponent<TroopBehaviour>().enabled = true;
    }

    void Die()
    {
        print("I am dead.");

        dead = true;
        
        if (deathSound != string.Empty)
        {
            SoundManager.instance.PlaySound(deathSound, 1, deathPitch, false, false);
        }

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


        if (gameObject.CompareTag("Player") && GameManager.instance.playerCount > 1)
        {
            int pID = GetComponent<PlayerControl>().playerID;

            GameManager.instance.RemovePlayer(pID);
            StartCoroutine(GameManager.instance.DisplayStatus("Player" + (pID + 1) + "has died!"));
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
        {
            healthBar.GetComponent<Image>().fillAmount = health / maxHealth;


            if (!animationTrigger.Equals(""))
            {
                healthBar.GetComponent<Animator>().SetTrigger(animationTrigger);
            }
        }

        float healthString = health;
        healthString = Mathf.Clamp(healthString, 0, Mathf.Infinity);

        if (healthText != null)
            healthText.text = healthString.ToString();
    }
}
