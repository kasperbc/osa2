using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public enum Troop { Regular }

    [SerializeField]
    private List<string> waves = new List<string>();
    [SerializeField]
    private List<GameObject> troops = new List<GameObject>();
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
        }

        return null;
    }
}
