using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private Player _playerPrefab;
    private Dictionary<PlayerRef, Player> _spawnedCharacters = new();
    public static List<Player> Players = new();

    public static SetInputData CurrentInput;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log("OnPlayerJoined");
        if (runner.IsServer || runner.IsSharedModeMasterClient)
        {
            var player = runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
            _spawnedCharacters.Add(playerRef, player);
            Players.Add(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log("OnPlayerLeft");
        if ((runner.IsServer || runner.IsSharedModeMasterClient) && _spawnedCharacters.TryGetValue(playerRef, out Player player))
        {
            Players.Remove(player);
            _spawnedCharacters.Remove(playerRef);
            runner.Despawn(player.Object);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CurrentInput.FocusSetId = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            CurrentInput.FocusSetId = 1;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(CurrentInput);
        Debug.Log($"{CurrentInput.FocusSetId}\n" +
            $"{CurrentInput.CreateSetId}\n" +
            $"{CurrentInput.DeleteSetId}\n");
        CurrentInput = new SetInputData(-1, -1, -1);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
}