using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Range(1, 4)]
    public int playerCount = 1;
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

        LoadMap("Lobby");
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

    public void LoadMap(string mapName)
    {
        UnloadMap();

        // Spawn in map
        GameObject map = Resources.Load<GameObject>("Maps/" + mapName);

        if (map == null)
        {
            Debug.LogError("Map " + mapName + " could not be found.");
            return;
        }

        GameObject spawnedMap = Instantiate(map);
        spawnedMap.name = "Map";

        // Load resources
        GameObject player = Resources.Load<GameObject>("Prefabs/Player");
        GameObject playerCam = Resources.Load<GameObject>("Prefabs/Player Camera");
        GameObject virtualCam = Resources.Load<GameObject>("Prefabs/Virtual Camera");
        GameObject reloadBar = Resources.Load<GameObject>("Prefabs/Reload Bar");

        // Setup players
        for (int i = 0; i < playerCount; i++)
        {
            Vector3 spawnPoint = spawnedMap.GetComponent<MapData>().spawnpoints[i];

            SpawnPlayer(player, playerCam, virtualCam, reloadBar, i + 1, spawnPoint);
        }
    }

    void SpawnPlayer(GameObject playerPrefab, GameObject camPrefab, GameObject vCamPrefab, GameObject reloadBarPrefab, int playerNo, Vector3 spawnpoint)
    {
        // Spawn player objects and cameras
        GameObject spawnedPlayer = Instantiate(playerPrefab);
        GameObject spawnedCam = Instantiate(camPrefab);
        GameObject spawnedVCam = Instantiate(vCamPrefab);
        GameObject spawnedRBar = Instantiate(reloadBarPrefab, GameObject.Find("Canvas").transform);

        // Set names
        spawnedPlayer.name = "Player " + playerNo;
        spawnedCam.name = "Player " + playerNo + " Camera";
        spawnedVCam.name = "Player " + playerNo + " Virtual Camera";
        spawnedRBar.name = "Player " + playerNo + " Reload Bar";

        PlayerShoot shootComponent = spawnedPlayer.GetComponent<PlayerShoot>();
        Camera cam = spawnedCam.GetComponent<Camera>();

        // Destroy duplicate audio listeners
        if (playerNo > 1)
        {
            Destroy(spawnedCam.GetComponent<AudioListener>());
        }

        // Set camera layers to link them to each other
        int camLayer = 6 + (playerNo - 1);
        spawnedCam.layer = camLayer;
        spawnedVCam.layer = camLayer;
        cam.cullingMask |= 1 << camLayer;


        // Link player to camera
        shootComponent.cam = spawnedCam.GetComponent<Camera>();

        // Link camera to player
        spawnedVCam.GetComponent<CinemachineVirtualCamera>().Follow = spawnedPlayer.transform.GetChild(0);

        // Link player to UI components
        shootComponent.reloadBar = spawnedRBar;

        // Setup splitscreen
        switch (playerCount)
        {
            case 1:
                cam.rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                if (playerNo == 1)
                    cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                else
                    cam.rect = new Rect(0, 0, 1, 0.5f);
                break;
            case 3:
                if (playerNo == 1)
                    cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                if (playerNo == 2)
                    cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                if (playerNo == 3)
                    cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
            case 4:
                if (playerNo == 1)
                    cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                if (playerNo == 2)
                    cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                if (playerNo == 3)
                    cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                if (playerNo == 4)
                    cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }

        // Set reload bar location
        Vector2 barPos = Vector2.zero;
        barPos.x = (cam.rect.x * 4 - 1 + ((cam.rect.width - 0.5f) * 2)) * 200;
        barPos.y = (cam.rect.y * 4 - 1 + ((cam.rect.height - 0.5f) * 2)) * 120;
        spawnedRBar.GetComponent<RectTransform>().localPosition = barPos;

        // Spawn player at spawnpoint
        spawnedPlayer.transform.position = spawnpoint;

        // Set camera bounds
        spawnedVCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = GameObject.Find("Bounds").GetComponent<Collider>();

        // Set player controls
        PlayerControl control = spawnedPlayer.GetComponent<PlayerControl>();
        if (playerNo == 1)
        {
            control.controlMethod = PlayerControl.ControlMethod.MouseAndKeyboard;
        }
        else
        {
            control.controlMethod = PlayerControl.ControlMethod.PS4;
            control.controllerPort = playerNo - 1;
        }

        // Set player color
        string color = "Green";
        switch (playerNo)
        {
            case 1:
                color = "Green";
                break;
            case 2:
                color = "Blue";
                break;
            case 3:
                color = "Red";
                break;
            case 4:
                color = "Yellow";
                break;
        }

        spawnedPlayer.transform.GetChild(0).GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Tank/" + color + "Base");
        spawnedPlayer.transform.GetChild(1).GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Tank/" + color + "Barrel");
        spawnedPlayer.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Tank/" + color + "Barrel");

        print("Spawned Player " + playerNo);
    }

    public void RemovePlayer(int number, bool removeCamera)
    {
        GameObject player = GameObject.Find("Player " + number);

        if (player == null)
        {
            Debug.LogError("Could not find Player " + number + ". (RemovePlayer)");
            return;
        }

        GameObject cam = player.GetComponent<PlayerShoot>().cam.gameObject;

        GameObject vCam = cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;

        Destroy(player);
        if (removeCamera)
        {
            Destroy(cam);
        }
        else
        {
            Destroy(cam.GetComponent<CinemachineBrain>());
        }
        Destroy(vCam);

        print("Removed Player " + number);
    }
    public void RemovePlayer(int number)
    {
        RemovePlayer(number, false);
    }

    public void UnloadMap()
    {
        GameObject map = GameObject.Find("Map");

        if (map == null)
        {
            Debug.LogWarning("There is no map loaded.");
            return;
        }

        Destroy(map);

        for (int p = 1; p <= playerCount; p++)
        {
            RemovePlayer(p, true);
        }

        print("Unloaded map.");
    }
}
