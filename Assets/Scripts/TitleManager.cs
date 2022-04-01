using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void LoadPractice()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadTwoPlayer()
    {
        SceneManager.LoadScene("SampleScene2Player");
    }
}
