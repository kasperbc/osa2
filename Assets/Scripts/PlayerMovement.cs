using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;    // How fast the player moves
    [SerializeField]
    private float turnSpeed;    // How fast the player turns
    [SerializeField]
    private GameObject playerModel;  // The player model that's used for rotation
    private Vector2 direction;  // The direction of movement

    private bool isBoosting;    // Is the player boosting?
    private float boostMeter;   // The current boost level
    private bool boostDepleted;   // Has the boost been depleted?

    private Image boostImage; // The boost UI component

    [SerializeField] bool p2;
    void Start()
    {
        boostMeter = 5;
    }

    // Update is called once per frame
    void Update()
    {   
        float horizontal = direction.x;
        float vertical = direction.y;

        // If the player is boosting, speed up the player
        if (isBoosting)
        {
            horizontal *= 1.5f;
            vertical *= 1.5f;

            // Decrease the boost meter
            DecreaseBoost(Time.deltaTime);
        }
        else
        {
            IncreaseBoost(Time.deltaTime / 1.5f);
        }

        // Put boost on cooldown if depleted
        if (boostMeter <= 0 && !boostDepleted)
        {
            DepleteBoost();
        }

        // Move the player
        transform.Translate(vertical * moveSpeed * Time.deltaTime * Vector3.forward);
        transform.Rotate(horizontal * Time.deltaTime * (turnSpeed * Mathf.Clamp(Mathf.Abs(vertical), 0.5f, 1)) * Vector3.up);

        Vector2 barrelTurnDirection = Vector2.zero;
        barrelTurnDirection.y = direction.x * Time.deltaTime * (turnSpeed / 5);

        GetComponent<PlayerShoot>().Aim(barrelTurnDirection);
    }

    public void SetDirection(float horizontal, float vertical)
    {
        direction.x = horizontal;
        direction.y = vertical;
    }

    public void Boost()
    {
        // Boost the player if able to
        isBoosting = boostMeter > 0 && !boostDepleted;
    }

    void DisplayBoostOnUI()
    {
        // Show the boost meter on the boost bar
        boostImage.fillAmount = boostMeter / 5;
        GameObject.Find("BoostValue").GetComponent<Text>().text = Mathf.Round(boostMeter / 5 * 100).ToString() + "%";

        // Is the boost depleted?
        if (boostDepleted)
        {
            // If yes, set the UI boost bar to be transparent
            boostImage.color = new Color(1, 0.5f, 0, 0.3f);
            // If the boost meter has been on cooldown enough activate it again
            if (boostMeter > 1)
            {
                boostDepleted = false;
                boostImage.color = new Color(1, 0.5f, 0, 1f);
            }
        }
    }

    void IncreaseBoost(float amount)
    {
        boostMeter += amount;

        // Limit the boost meter at a max of 5 seconds
        boostMeter = Mathf.Clamp(boostMeter, Mathf.NegativeInfinity, 5);
    }
    void DecreaseBoost(float amount)
    {
        IncreaseBoost(-amount);
    }

    void DepleteBoost()
    {
        boostDepleted = true;

        boostMeter = -3;
    }

    public bool Boosting()
    {
        return isBoosting;
    }
}
