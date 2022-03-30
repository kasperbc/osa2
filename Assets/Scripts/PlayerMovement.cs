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
    private float boostMeter;   // How much the player can use the boost ability
    private bool boostDepleted;   // Has the boost been depleted?
    void Start()
    {
        boostMeter = 5;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //
        // BOOST ABILITY
        //

        // Is the player able to boost?
        bool boost = Input.GetKey(KeyCode.LeftShift) && boostMeter > 0 && !boostDepleted;

        // Get the UI boost meter object
        Image boostBar = GameObject.Find("BoostBar").GetComponent<Image>();

        // If the player is boosting, speed up the player
        if (boost)
        {
            horizontal *= 1.5f;
            vertical *= 1.5f;

            // Decrease the boost meter
            boostMeter -= Time.deltaTime;
        }
        else
        {
            // Increase the boost meter if the player isn't boosting
            boostMeter += Time.deltaTime / 1.5f;
        }

        // Limit the boost meter at a max of 5 seconds
        boostMeter = Mathf.Clamp(boostMeter, Mathf.NegativeInfinity, 5);

        // Put boost on cooldown if depleted
        if (boostMeter <= 0 && boostDepleted == false)  DepleteBoost();

        // Is the boost depleted?
        if (boostDepleted == true)
        {
            // If yes, set the UI boost bar to be transparent
            boostBar.color = new Color(1, 0.5f, 0, 0.3f);
            // If the boost meter has been on cooldown enough activate it again
            if (boostMeter > 1)
            {
                boostDepleted = false;
                boostBar.color = new Color(1, 0.5f, 0, 1f);
            }
        }

        // Show the boost meter on the boost bar
        boostBar.fillAmount = boostMeter / 5;
        GameObject.Find("BoostValue").GetComponent<Text>().text = Mathf.Round(boostMeter / 5 * 100).ToString() + "%";

        transform.Translate(vertical * moveSpeed * Time.deltaTime * Vector3.forward);
        transform.Rotate(horizontal * Time.deltaTime * (turnSpeed * Mathf.Clamp(Mathf.Abs(vertical), 0.5f, 1)) * Vector3.up);
    }

    void DepleteBoost()
    {
        boostDepleted = true;

        boostMeter = -3;
    }
}
