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
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
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
