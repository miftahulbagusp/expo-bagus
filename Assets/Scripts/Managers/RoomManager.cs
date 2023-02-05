using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance { get; private set; }

    public List<GameObject> playersInRoom {get; private set;} = new List<GameObject>();
    
    [Header("Controller")]
    [SerializeField] private NonVRPlayerController nonVRController;

    [Header("Player Spawn Points")] 
    public Transform spawnLocation;
    public Transform randomSpawnLocation;
    
    [Header("Room Setting")] 
    public float defaultPlayerSize = 1f;
    public AvatarExpressionData avatarExpressionData;

    [SerializeField] private AvatarSpawner avatarSpawner;
    enum GameState
    {
        Starting,
        Running,
    }
    
    [Networked] private GameState _gameState { get; set; }
    public bool GameIsRunning => _gameState == GameState.Running;

    private void Awake()
    {
        Instance = this;
        
        Application.targetFrameRate = 60;
        
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public override void Spawned()
    {
        Debug.Log("Room Spawner");
        
        if (Object.HasStateAuthority == false) return;
    
        if (_gameState != GameState.Starting)
        {

        }
    
        // Initialize the game state on the master client
        _gameState = GameState.Starting;
    }

    public override void FixedUpdateNetwork()
    {
        switch (_gameState)
        {
            case GameState.Starting:
                UpdateStartingDisplay();
                break;
            case GameState.Running:
                UpdateRunningDisplay();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Called every joining the room
    /// </summary>
    public void Init()
    {
        Debug.Log("Room Init");
        Vector3 initPosition;
        Vector2 initRotation;
        
        if (NetworkManager.Instance.SpawnLocation  > -1)
        {
            initPosition = spawnLocation.GetChild(NetworkManager.Instance.SpawnLocation).position;
            initRotation = spawnLocation.GetChild(NetworkManager.Instance.SpawnLocation).eulerAngles;
            NetworkManager.Instance.SpawnLocation = -1;
        }
        else
        {
            List<Transform> targetSpawnLocation = new List<Transform>();
            for (int i = 0; i < randomSpawnLocation.childCount; i++)
            {
                if (randomSpawnLocation.GetChild(i).gameObject.activeSelf)
                {
                    targetSpawnLocation.Add(randomSpawnLocation.GetChild(i));
                }

            }
            
            int randomIndex = Random.Range(0, targetSpawnLocation.Count - 1);
            initPosition = targetSpawnLocation[randomIndex].position;
            initRotation = targetSpawnLocation[randomIndex].eulerAngles;
        }

        // Setup player initial position
        nonVRController.transform.position = initPosition;
        nonVRController.avatarPivot.eulerAngles = new Vector3(0, initRotation.y, 0);
        nonVRController.cameraPivot.localEulerAngles = new Vector3(0, initRotation.y, 0);
        nonVRController.gameObject.SetActive(true);

        // Instantiate player avatar
        InputManager.Instance.enableAvatarMove = true;
    }

    private void UpdateStartingDisplay()
    {
        // --- Master client
        if (Object.HasStateAuthority == false) return;

        // Starts the avatar spawners once the game start delay has expired
        RPC_SpawnReadyPlayers();

        // Switches to the Running GameState and sets the time to the length of a game session
        _gameState = GameState.Running;
    }
    
    private void UpdateRunningDisplay()
    {
        //TODO: get in room player
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SpawnReadyPlayers()
    {
        avatarSpawner.InstantiateNetworkAvatar(Runner.LocalPlayer);
    }
}