using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public enum Troop { Regular, Aggressive, Boss, Fast, Bomber, PlayerBomber }

    [SerializeField]
    private List<string> waves = new List<string>();
    [SerializeField]
    private List<GameObject> troops = new List<GameObject>();
    public bool waveInProgression;
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

    void Update()
    {
        if (waveInProgression)
        {
            GetWaveCompletion();
        }
    }

    public int GetWaveCount()
    {
        return waves.Count;
    }

    public string[] ReadWave(int wave)
    {
        string[] waveData = waves[wave].Split('-');

        List<string> result = new List<string>();

        foreach (string w in waveData)
        {
            if (w == "")
            {
                continue;
            }
            
            result.Add(w);
        }

        return result.ToArray();
    }

    public GameObject GetTroop(Troop troopType)
    {
        switch (troopType)
        {
            case Troop.Regular:
                return troops[0];
            case Troop.Aggressive:
                return troops[1];
            case Troop.Fast:
                return troops[2];
            case Troop.Bomber:
                return troops[3];
            case Troop.PlayerBomber:
                return troops[4];
            case Troop.Boss:
                return troops[5];
        }

        return null;
    }

    private void GetWaveCompletion()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");

        if (troops.Length == 0)
        {
            waveInProgression = false;

            StartCoroutine(GameManager.instance.StartWave());
        }
    }
}
