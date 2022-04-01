using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private static int[] scores = new int[2];
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameObject.Find("Score").GetComponent<Text>().text = "Player 1: " + scores[1] + "\nPlayer 2: " + scores[0] + "\n\nFirst to 5 wins";
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void AddScore(int score)
    {
        scores[score]++;

        if (score == 0)
        {
            StartCoroutine(DisplayStatus("Player 2 scored!"));
        }
        if (score == 1)
        {
            StartCoroutine(DisplayStatus("Player 1 scored!"));
        }

        GameObject.Find("Ball").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        GameObject.Find("Score").GetComponent<Text>().text = "Player 1: " + scores[1] + "\nPlayer 2: " + scores[0] + "\n\nFirst to 5 wins";

        if (scores[0] >= 5)
        {
            StartCoroutine(DisplayStatus("Player 2 wins!"));
        }
        else if (scores[1] >= 5)
        {
            StartCoroutine(DisplayStatus("Player 1 wins!"));
        }
        else
        {
            Invoke("ReloadScene", 5);
        }
    }

    public IEnumerator DisplayStatus(string status)
    {
        GameObject.Find("Status").GetComponent<Text>().text = status;

        yield return new WaitForSeconds(5);

        GameObject.Find("Status").GetComponent<Text>().text = string.Empty;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetScore()
    {
        scores[0] = 0;
        scores[1] = 0;

        ReloadScene();
    }
}
