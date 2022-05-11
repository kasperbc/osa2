using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Range(1, 4)]
    public int playerCount = 1;

    private GameObject player;
    private GameObject playerCamera;
    private GameObject playerVirtualCamera;
    private GameObject playerCanvas;

    private bool cursorLocked;
    private bool joinable;
    private bool[] portsInUse = new bool[4];

    private int wave = 0;

    private List<GameObject> lobbyUI = new List<GameObject>();
    private List<GameObject> invasionUI = new List<GameObject>();

    private bool debugMode;

    private GameObject gameOverPanel;
    private int score;
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
        playerCanvas = Resources.Load<GameObject>("Prefabs/Player Canvas");

        LoadMap("Lobby");

        ToggleMouseLock();

        joinable = true;

        SoundManager.instance.PlaySound("whitenoise", 0.2f, 1, true, false);
        SoundManager.instance.StopSound("wavemusic");

        lobbyUI.Add(GameObject.Find("EnterStart"));

        invasionUI.Add(GameObject.Find("DiamondHealthBar"));
        invasionUI.Add(GameObject.Find("Troops"));

        SetInvasionUI(false);

        if (Application.isEditor)
        {
            debugMode = true;
        }

        gameOverPanel = GameObject.Find("GameOver");
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (joinable)
        {
            ListenForPlayerJoins();
            ListenForStart();
        }

        ListenForPlayerLeaves();

        if (debugMode && Input.GetKeyDown(KeyCode.P))
        {
            GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");

            foreach (GameObject t in troops)
            {
                Destroy(t);
            }

            StopCoroutine(SpawnWave());
        }
    }

    void ListenForStart()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartInvasionGame();
        }
    }

    void ListenForPlayerJoins()
    {
        // Listen to input from controllers 1-4
        for (int i = 1; i <= 4; i++)
        {
            if (portsInUse[i - 1])
            {
                continue;
            }

            if (Input.GetKeyDown((KeyCode)331 + (20 * i)))
            {
                AddPlayer(i);
            }
        }
    }

    void ListenForPlayerLeaves()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 1; i <= players.Length; i++)
        {
            int port = players[i - 1].GetComponent<PlayerControl>().controllerPort;

            if (Input.GetKeyDown((KeyCode)338 + (20 * port)) && port != 0)
            {
                print(port);

                RemovePlayer(i);
            }
        }
    }

    public void ToggleMouseLock()
    {
        cursorLocked = !cursorLocked;

        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public IEnumerator DisplayStatus(string status)
    {
        GameObject.Find("Status").GetComponent<TextMeshProUGUI>().text = status;

        yield return new WaitForSeconds(5);

        GameObject.Find("Status").GetComponent<TextMeshProUGUI>().text = string.Empty;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMap(string mapName)
    {
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
        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("Player");

        if (existingPlayers.Length == playerCount)
        {
            for (int i = 0; i < playerCount; i++)
            {
                Vector3 spawnPoint = spawnedMap.GetComponent<MapData>().spawnpoints[i];

                existingPlayers[i].transform.position = spawnPoint; 
            }
        }
        else
        {
            for (int i = 0; i < playerCount; i++)
            {
                Vector3 spawnPoint = spawnedMap.GetComponent<MapData>().spawnpoints[i];

                if (GameObject.Find("Player " + (i + 1)) != null)
                {
                    continue;
                }

                SpawnPlayer(i + 1, spawnPoint, 0);
            }
        }
    }

    void SpawnPlayer(int playerNo, Vector3 spawnpoint, int controllerPort)
    {
        // Spawn player objects and cameras
        GameObject spawnedPlayer = Instantiate(player);
        GameObject spawnedCam = Instantiate(playerCamera);
        GameObject spawnedVCam = Instantiate(playerVirtualCamera);

        GameObject spawnedCanvas = Instantiate(playerCanvas);

        // Set names
        spawnedPlayer.name = "Player " + playerNo;
        spawnedCam.name = "Player " + playerNo + " Camera";
        spawnedVCam.name = "Player " + playerNo + " Virtual Camera";
        spawnedCanvas.name = "Player " + playerNo + " Canvas";

        PlayerShoot shootComponent = spawnedPlayer.GetComponent<PlayerShoot>();
        Camera cam = spawnedCam.GetComponent<Camera>();
        UpgradeManager upgradeComponent = spawnedPlayer.GetComponent<UpgradeManager>();

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

        // Link canvas to camera
        spawnedCanvas.GetComponent<Canvas>().worldCamera = spawnedCam.GetComponent<Camera>();
        spawnedCanvas.GetComponent<Canvas>().planeDistance = 0.35f;

        // Link player to camera
        shootComponent.cam = spawnedCam.GetComponent<Camera>();

        // Link camera to player
        spawnedVCam.GetComponent<CinemachineVirtualCamera>().Follow = spawnedPlayer.transform.GetChild(1);

        // Link player to UI components
        shootComponent.reloadBar = spawnedCanvas.transform.GetChild(2).gameObject;
        shootComponent.crossHair = spawnedCanvas.transform.GetChild(1).gameObject;
        spawnedPlayer.GetComponent<Health>().healthBar = spawnedCanvas.transform.GetChild(0).gameObject;
        spawnedPlayer.GetComponent<Health>().healthText = spawnedCanvas.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        // Link player to upgrade panel
        upgradeComponent.upgradePanel = spawnedCanvas.transform.GetChild(3).gameObject;

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

        // Spawn player at spawnpoint
        spawnedPlayer.transform.position = spawnpoint;

        // Set camera bounds
        spawnedVCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = GameObject.Find("Bounds").GetComponent<Collider>();

        // Set player controls
        PlayerControl control = spawnedPlayer.GetComponent<PlayerControl>();

        if (controllerPort == 0)
        {
            if (playerNo == 1)
            {
                control.controlMethod = PlayerControl.ControlMethod.MouseAndKeyboard;
            }
            else
            {
                control.controlMethod = PlayerControl.ControlMethod.PS4;
                control.controllerPort = playerNo - 1;
            }
        }
        else
        {
            control.controlMethod = PlayerControl.ControlMethod.PS4;
            control.controllerPort = controllerPort;
        }

        control.playerID = playerNo;

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

        int port = player.GetComponent<PlayerControl>().controllerPort;
        if (port != 0)
        {
            portsInUse[port - 1] = false;
        }

        GameObject cam = player.GetComponent<PlayerShoot>().cam.gameObject;
        GameObject vCam = cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
        GameObject rBar = player.GetComponent<PlayerShoot>().reloadBar;
        GameObject cHair = player.GetComponent<PlayerShoot>().crossHair;
        GameObject hBar = player.GetComponent<Health>().healthBar;


        Destroy(player);
        Destroy(cam);
        Destroy(vCam);
        Destroy(rBar);
        Destroy(cHair);
        Destroy(hBar);

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

    public void AddPlayer(int controllerPort)
    {
        if (playerCount >= 4)
        {
            Debug.LogWarning("Player limit reached. Unable to add more.");
            return;
        }
        
        playerCount++;

        if (controllerPort != 0)
        {
            portsInUse[controllerPort - 1] = true;
        }

        SpawnPlayer(playerCount, GetMapData().spawnpoints[playerCount - 1], controllerPort);
        SetSplitScreenCameras();

        StartCoroutine(DisplayStatus("Player " + playerCount + " has joined!"));
    }
    public void AddPlayer()
    {
        AddPlayer(0);
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

    public void UnloadMap(bool removePlayers)
    {
        GameObject map = GameObject.Find("Map");

        if (map == null)
        {
            Debug.Log("There is no map loaded.");
            return;
        }

        Destroy(map);

        if (removePlayers)
        {
            for (int p = 1; p <= playerCount; p++)
            {
                RemovePlayer(p);
            }
        }

        print("Unloaded map.");
    }
    public void UnloadMap()
    {
        UnloadMap(true);
    }
    
    public void StartInvasionGame()
    {
        UnloadMap(false);

        joinable = false;

        LoadMap("Diamond");

        Instantiate(Resources.Load<GameObject>("Prefabs/WaveManager"));

        SoundManager.instance.PlaySound("wavemusic", 0.1f, 1, true, true);
        SoundManager.instance.StopSound("whitenoise");

        EnableFog();

        SetLobbyUI(false);
        SetInvasionUI(true);

        GameObject.Find("Diamond").GetComponent<Health>().healthBar = GameObject.Find("DiamondHealthBarFill");
        GameObject.Find("Diamond").GetComponent<Health>().healthText = GameObject.Find("DiamondHealthText").GetComponent<TextMeshProUGUI>();

        StartCoroutine(StartWave());
    }

    public IEnumerator StartWave()
    {
        wave++;

        if (WaveManager.instance != null)
        {
            if (WaveManager.instance.GetWaveCount() < wave)
            {
                StartCoroutine(EndInvasionGame());

                yield break;
            }
        }

        StartCoroutine(DisplayStatus("Wave " + wave + " incoming!"));

        yield return new WaitForSeconds(5);

        WaveManager.instance.waveInProgression = true;

        StartCoroutine(SpawnWave());
    }

    public IEnumerator EndInvasionGame()
    {
        StartCoroutine(DisplayStatus("All waves complete! You win!"));

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator SpawnWave()
    {
        string[] waveData = WaveManager.instance.ReadWave(wave - 1);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            GameObject vCam = player.GetComponent<PlayerShoot>().cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;

            vCam.GetComponent<Animator>().SetTrigger("Shake");
        }

        SoundManager.instance.PlaySound("rumble");

        foreach (string wave in waveData)
        {
            if (int.TryParse(wave, out int seconds))
            {
                yield return new WaitForSeconds(seconds);

                continue;
            }

            char[] waveChars = wave.ToCharArray();
            char firstChar = waveChars[0];

            switch (waveChars[0])
            {
                case 'r':
                    SpawnTroop(WaveManager.Troop.Regular, wave);
                    break;
                case 'a':
                    SpawnTroop(WaveManager.Troop.Aggressive, wave);
                    break;
                case 'b':
                    SpawnTroop(WaveManager.Troop.Boss, wave);
                    break;
                case 'f':
                    SpawnTroop(WaveManager.Troop.Fast, wave);
                    break;
                case 's':
                    SpawnTroop(WaveManager.Troop.Bomber, wave);
                    break;
                case 'p':
                    SpawnTroop(WaveManager.Troop.PlayerBomber, wave);
                    break;
            }
        }
    }

    void SpawnTroop(WaveManager.Troop troopType, string wave)
    {
        char[] waveChars = wave.ToCharArray();

        string countString = string.Empty;

        foreach (char waveChar in waveChars)
        {
            if (char.IsDigit(waveChar))
            {
                countString += waveChar;
            }
        }

        int count = int.Parse(countString);

        for (int i = 0; i < count; i++)
        {
            GameObject troopPrefab = WaveManager.instance.GetTroop(troopType);

            float spawnRot = Random.Range(0, 359);
            float spawnDistance = Random.Range(30, 50);

            Quaternion rotation = Quaternion.Euler(0, spawnRot, 0);

            GameObject troop = Instantiate(troopPrefab, new Vector3(0, -3, 0), rotation);

            TroopBehaviour troopBehaviour = troop.GetComponent<TroopBehaviour>();
            Health troopHealth = troop.GetComponent<Health>();

            troopBehaviour.moveSpeed *= WaveManager.instance.enemySpeedModifier;
            troopHealth.MultiplyMaxHealth(0.05f * GetWave() + 1);

            troop.transform.Translate(troop.transform.forward * spawnDistance);
        }
    }

    int GetWave()
    {
        return wave;
    }

    void EnableFog()
    {
        RenderSettings.fog = true;

        RenderSettings.fogDensity = 0.05f;
    }

    void DisableFog()
    {
        RenderSettings.fog = false;
    }

    void SetLobbyUI(bool value)
    {
        foreach (GameObject o in lobbyUI)
        {
            o.SetActive(value);
        }
    }
    
    void SetInvasionUI(bool value)
    {
        foreach (GameObject o in invasionUI)
        {
            o.SetActive(value);
        }
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);

        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");

        foreach (GameObject t in troops)
        {
            Destroy(t);
        }

        gameOverPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Score: " + score;

        ToggleMouseLock();

        Destroy(WaveManager.instance);
    }

    public void AddScore(int value)
    {
        score += value;

        if (score < 0)
            score = 0;
    }
}
