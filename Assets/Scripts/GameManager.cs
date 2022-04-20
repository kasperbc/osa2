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

    private GameObject player;
    private GameObject playerCamera;
    private GameObject playerVirtualCamera;
    private GameObject reloadBar;
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

        // Load resources
        player = Resources.Load<GameObject>("Prefabs/Player");
        playerCamera = Resources.Load<GameObject>("Prefabs/Player Camera");
        playerVirtualCamera = Resources.Load<GameObject>("Prefabs/Virtual Camera");
        reloadBar = Resources.Load<GameObject>("Prefabs/Reload Bar");

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

        // Setup players
        for (int i = 0; i < playerCount; i++)
        {
            Vector3 spawnPoint = spawnedMap.GetComponent<MapData>().spawnpoints[i];

            SpawnPlayer(i + 1, spawnPoint);
        }
    }

    void SpawnPlayer(int playerNo, Vector3 spawnpoint)
    {
        // Spawn player objects and cameras
        GameObject spawnedPlayer = Instantiate(player);
        GameObject spawnedCam = Instantiate(playerCamera);
        GameObject spawnedVCam = Instantiate(playerVirtualCamera);
        GameObject spawnedRBar = Instantiate(reloadBar, GameObject.Find("Canvas").transform);

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
        spawnedVCam.GetComponent<CinemachineVirtualCamera>().Follow = spawnedPlayer.transform.GetChild(1);

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
        //SetSplitScreenCameras();

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

    public void RemovePlayer(int number)
    {
        GameObject player = GameObject.Find("Player " + number);

        if (player == null)
        {
            Debug.LogError("Could not find Player " + number + ". (RemovePlayer)");
            return;
        }
        if (playerCount <= 0)
        {
            Debug.LogWarning("There are no players loaded.");
            return;
        }

        playerCount--;

        GameObject cam = player.GetComponent<PlayerShoot>().cam.gameObject;

        GameObject vCam = cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;

        Destroy(player);
        Destroy(cam);
        Destroy(vCam);

        print("Removed Player " + number);

        // Re-order other players
        for (int i = number + 1; i <= 4; i++)
        {
            GameObject p = GameObject.Find("Player " + i);

            if (p != null)
            {
                p.name = "Player " + (i - 1);
                GameObject c = p.GetComponent<PlayerShoot>().cam.gameObject;
                c.layer--;
                c.name = "Player " + (i - 1) + " Camera";
                GameObject v = c.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
                v.layer--;
                v.name = "Player " + (i - 1) + " Virtual Camera";
                p.GetComponent<PlayerShoot>().reloadBar.name = "Player " + (i - 1) + " Reload Bar";


                c.GetComponent<Camera>().cullingMask |= 1 << c.layer;
                c.GetComponent<Camera>().cullingMask &= ~(1 << (c.layer + 1));
            }
        }


        SetSplitScreenCameras();
    }

    private MapData GetMapData()
    {
        GameObject map = GameObject.Find("Map");

        if (map != null)
        {
            return GameObject.Find("Map").GetComponent<MapData>();
        }
        else
        {
            Debug.LogError("Unable to get map data.");
            return null;
        } 
    }
    public void RemovePlayer()
    {
        RemovePlayer(playerCount);
    }

    public void AddPlayer()
    {
        if (playerCount >= 4)
        {
            Debug.LogWarning("Player limit reached. Unable to add more.");
            return;
        }
        
        playerCount++;

        SpawnPlayer(playerCount, GetMapData().spawnpoints[playerCount - 1]);
        SetSplitScreenCameras();
    }

    private void SetSplitScreenCameras()
    {
        GameObject[] cameraObjects = GameObject.FindGameObjectsWithTag("PlayerCamera");
        Camera[] cameras = new Camera[4];

        foreach (GameObject c in cameraObjects)
        {
            Camera cam = c.GetComponent<Camera>();

            switch (c.layer)
            {
                case 6:
                    cameras[0] = cam;
                    break;
                case 7:
                    cameras[1] = cam;
                    break;
                case 8:
                    cameras[2] = cam;
                    break;
                case 9:
                    cameras[3] = cam;
                    break;
            }
        }

        switch (playerCount)
        {
            case 1:
                cameras[0].rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                cameras[0].rect = new Rect(0, 0.5f, 1, 0.5f);
                cameras[1].rect = new Rect(0, 0, 1, 0.5f);
                break;
            case 3:
                cameras[0].rect = new Rect(0, 0.5f, 1, 0.5f);
                cameras[1].rect = new Rect(0, 0, 0.5f, 0.5f);
                cameras[2].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
            case 4:
                cameras[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                cameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                cameras[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                cameras[3].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }

    public void UnloadMap()
    {
        GameObject map = GameObject.Find("Map");

        if (map == null)
        {
            Debug.Log("There is no map loaded.");
            return;
        }

        Destroy(map);

        for (int p = 1; p <= playerCount; p++)
        {
            RemovePlayer(p);
        }

        print("Unloaded map.");
    }
}
