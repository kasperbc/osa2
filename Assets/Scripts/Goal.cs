using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public GameObject scoreCounter;
    private int score;
    public bool practice;
    public int goalID;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            score++;

            scoreCounter.GetComponent<TMPro.TextMeshPro>().SetText(score.ToString());
        }
    }
}
