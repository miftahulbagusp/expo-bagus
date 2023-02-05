using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using AuthenticationValues = Fusion.Photon.Realtime.AuthenticationValues;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance;
    private NetworkRunner _runner;

    // Network properties
    [HideInInspector] public string roomName;
    
    public int SpawnLocation { get; set; } = -1;
    public string CountPlayersOnline => _runner.SessionInfo.PlayerCount.ToString();
    public string GetPing => $"{_runner.GetPlayerRtt(_runner.LocalPlayer)*1000:N0} ms";

    public Action<NetworkRunner, PlayerRef> OnPlayerJoined;
    public Action<NetworkRunner, PlayerRef> OnPlayerLeft;
    public Action<NetworkRunner, NetworkInput> OnInput;
    public Action<NetworkRunner, PlayerRef, NetworkInput> OnInputMissing;
    public Action<NetworkRunner, ShutdownReason> OnShutdown;
    public Action<NetworkRunner> OnConnectedToServer;
    public Action<NetworkRunner> OnDisconnectedFromServer;
    public Action<NetworkRunner, NetworkRunnerCallbackArgs.ConnectRequest, byte[]> OnConnectRequest;
    public Action<NetworkRunner, NetAddress, NetConnectFailedReason> OnConnectFailed;
    public Action<NetworkRunner, SimulationMessagePtr> OnUserSimulationMessage;
    public Action<NetworkRunner, List<SessionInfo>> OnSessionListUpdated;
    public Action<NetworkRunner, Dictionary<string, object>> OnCustomAuthenticationResponse;
    public Action<NetworkRunner, HostMigrationToken> OnHostMigration;
    public Action<NetworkRunner, PlayerRef, ArraySegment<byte>> OnReliableDataReceived;
    public Action<NetworkRunner> OnSceneLoadDone;
    public Action<NetworkRunner> OnSceneLoadStart;
    
    [Header("Debug Mode")]
    [SerializeField] [Tooltip("Press Numpad 9 to Disconnect")] private bool enableDisconnectShortcut;
    [SerializeField] private bool useCustomRoomNamePrefix;
    [SerializeField] private string customRoomNamePrefix = "test_";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!_runner) _runner = gameObject.AddComponent<NetworkRunner>();
    }

    private void Start()
    {
        OnConnectedToServer += delegate { Debug.Log("Connected to server."); };
        OnDisconnectedFromServer += delegate(NetworkRunner networkRunner) { networkRunner.Shutdown(); };
        OnShutdown += delegate(NetworkRunner networkRunner, ShutdownReason reason) { networkRunner.SetActiveScene(0); };
        OnSceneLoadStart += delegate { Debug.Log("Scene load start."); };
        OnSceneLoadDone += delegate { Debug.Log("Scene load done."); };
    }

    private void Update()
    {
        if (enableDisconnectShortcut)
        {
            if (Keyboard.current.numpad9Key.isPressed)
            {
                _runner.Disconnect(_runner.LocalPlayer);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (loadSceneMode == LoadSceneMode.Single)
        {
            if (scene.buildIndex != 0)
            {
                Debug.Log("Scene Loaded");

                RoomManager.Instance.Init();
            }
        }
    }

    public async void JoinRoom()
    {
        AuthenticationValues authenticationValues = new AuthenticationValues {UserId = Guid.NewGuid().ToString()};
        _runner.ProvideInput = true;

        string targetRoomName = useCustomRoomNamePrefix ? customRoomNamePrefix + roomName : roomName;
        
        Debug.Log("Connecting to Session: " + targetRoomName);
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            AuthValues = authenticationValues
        };

        var task = await _runner.StartGame(startGameArgs);

        if (task.Ok)
        {
            _runner.SetActiveScene(1);

            // switch (roomName)
            // {
                // case StringValue.RANS_ISLAND:
                //     _runner.SetActiveScene(1);
                //     break;
                //
                // case StringValue.SBM_ITB:
                //     _runner.SetActiveScene(2);
                //     break;
                //
                // case StringValue.RANSVERSE_HOUSE:
                //     _runner.SetActiveScene(3);
                //     break;
                //
                // case StringValue.PEMPROV_DKI:
                //     _runner.SetActiveScene(4);
                //     break;
                //
                // case StringValue.KAABAVERSE:
                //     _runner.SetActiveScene(5);
                //     break;
            // }
        }
        else
        {
            Debug.Log("Start game failed");
        }

    }
    
    public void LeaveRoom()
    {
        _runner.Disconnect(_runner.LocalPlayer);

        SceneManager.LoadSceneAsync(0);
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { OnPlayerJoined?.Invoke(runner, player); }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { OnPlayerLeft?.Invoke(runner,player); }
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { OnInput?.Invoke(runner, input); }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { OnInputMissing?.Invoke(runner, player, input); }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { OnShutdown?.Invoke(runner, shutdownReason); }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { OnConnectedToServer?.Invoke(runner); }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { OnDisconnectedFromServer?.Invoke(runner); }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { OnConnectRequest?.Invoke(runner, request, token); }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { OnConnectFailed?.Invoke(runner, remoteAddress, reason); }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { OnUserSimulationMessage?.Invoke(runner, message); }
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { OnSessionListUpdated?.Invoke(runner, sessionList); }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { OnCustomAuthenticationResponse?.Invoke(runner, data); }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { OnHostMigration?.Invoke(runner, hostMigrationToken); }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { OnReliableDataReceived?.Invoke(runner, player, data); }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { OnSceneLoadDone?.Invoke(runner); }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { OnSceneLoadStart?.Invoke(runner); }
}